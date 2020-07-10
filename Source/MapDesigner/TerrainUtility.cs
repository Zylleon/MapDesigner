using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MapDesigner
{
    public static class TerrainUtility
    {

        private static TerrainDefault oldTerrain;
        private static TerrainDefault newTerrain;
        private static bool changeTbf = false;
        private static bool changePatchMakers = false;
        private static float minMapFert = -0.20f;
        private static float maxMapFert = 1.20f;
        private static string biomeName;

        public static TerrainDefault StretchTerrainFertility(TerrainDefault old, float min, float max, string biome)
        {
            oldTerrain = old;
            minMapFert = min;
            maxMapFert = max;
            biomeName = biome;

            newTerrain = new TerrainDefault();
            newTerrain.terrainsByFertility = new List<TerrainThreshold>(oldTerrain.terrainsByFertility);

            newTerrain.terrainPatchMakers = new List<TerrainPatchMaker>();
            for (int i = 0; i < oldTerrain.terrainPatchMakers.Count(); i++)
            {
                TerrainPatchMaker p = new TerrainPatchMaker();
                p.maxFertility = oldTerrain.terrainPatchMakers[i].maxFertility;
                p.minFertility = oldTerrain.terrainPatchMakers[i].minFertility;
                p.perlinFrequency = oldTerrain.terrainPatchMakers[i].perlinFrequency;
                p.perlinLacunarity = oldTerrain.terrainPatchMakers[i].perlinLacunarity;
                p.perlinOctaves = oldTerrain.terrainPatchMakers[i].perlinOctaves;
                p.perlinPersistence = oldTerrain.terrainPatchMakers[i].perlinPersistence;
                p.thresholds = new List<TerrainThreshold>(oldTerrain.terrainPatchMakers[i].thresholds);

                newTerrain.terrainPatchMakers.Add(p);
            }


            List<TerrainThreshold> oldTerrainsByFertility = oldTerrain.terrainsByFertility;

            List<TBF> listTbf = new List<TBF>();

            // convert to TBFs
            for (int i = 0; i < oldTerrainsByFertility.Count(); i++)
            {
                TBF item = new TBF()
                {
                    fertRank = i,
                    thresh = oldTerrainsByFertility[i],
                    size = Math.Min(oldTerrainsByFertility[i].max, maxMapFert) - Math.Max(oldTerrainsByFertility[i].min, minMapFert)
                };
                listTbf.Add(item);
            }

            // the actual adjustments
            if (listTbf.Where(t => !t.thresh.terrain.IsWater).Count() >= 2)
            {
                changeTbf = true;
                FertChangeTbf(listTbf);
            }

            // TerrainPatchMaker adjustments
            List<TerrainDef> patchTerrains = new List<TerrainDef>();

            foreach (TerrainPatchMaker p in newTerrain.terrainPatchMakers)
            {
                foreach (TerrainThreshold t in p.thresholds)
                {
                    patchTerrains.Add(t.terrain);
                }
            }
            patchTerrains = patchTerrains.Where(p => !p.IsWater).Distinct().ToList();

            // check that there are at least 2 nonwater terrains to compare
            if (patchTerrains.Count() >= 2)
            {
                changePatchMakers = true;
                FertChangePatchMakers(patchTerrains);
            }


            // TODO: if changeTbf and changePatchMakers are both false, make alterhate adjustments
            // Which biomes would this even affect?

            return newTerrain;
        }



        private static void FertChangeTbf(List<TBF> listTbf)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();
            List<TerrainThreshold> newTbf = new List<TerrainThreshold>();
            float rangeSize = maxMapFert - minMapFert;
            List<TerrainThreshold> oldTbf = oldTerrain.terrainsByFertility;

            // the actual adjustments
            listTbf.Sort((x, y) => x.thresh.terrain.fertility.CompareTo(y.thresh.terrain.fertility));
            listTbf.Where(t => !t.thresh.terrain.IsWater).First().size /= settings.terrainFert;
            listTbf.Where(t => !t.thresh.terrain.IsWater).Last().size *= settings.terrainFert;

            float rangeSizeNew = listTbf.Sum(t => t.size);
            float ratio = rangeSize / rangeSizeNew;

            // make new threshes
            listTbf = listTbf.OrderBy(t => t.thresh.min).ToList();

            float curValue = 0f;
            foreach (TBF tbf in listTbf)
            {
                TerrainThreshold thresh = new TerrainThreshold()
                {
                    terrain = tbf.thresh.terrain,
                    min = curValue,
                    max = curValue + ratio * tbf.size
                };
            curValue = thresh.max;
                newTbf.Add(thresh);
            }

            // reset to original active range
            foreach (TerrainThreshold t in newTbf)
            {
                t.min += minMapFert;
                t.max += minMapFert;
            }

            // stretch to cover full range
            newTbf.Sort((x, y) => x.min.CompareTo(y.min));
            newTbf[0].min = -999f;
            newTbf.Last().max = 999f;

            // adjust patchmaker min max
            newTbf.Sort((x, y) => x.min.CompareTo(y.min));
            oldTbf.Sort((x, y) => x.min.CompareTo(y.min));

            SimpleCurve curve = new SimpleCurve();
            for (int i = 0; i < oldTbf.Count(); i++)
            {
                curve.Add(new CurvePoint(oldTbf[i].min, newTbf[i].min));
            }
            curve.Add(new CurvePoint(oldTbf.Last().max, newTbf.Last().max));

            foreach (TerrainPatchMaker patchMaker in newTerrain.terrainPatchMakers)
            {
                patchMaker.minFertility = curve.Evaluate(patchMaker.minFertility);
                patchMaker.maxFertility = curve.Evaluate(patchMaker.maxFertility);
            }

            newTerrain.terrainsByFertility = newTbf;
        }



        private static void FertChangePatchMakers(List<TerrainDef> patchTerrains)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();
            float minAllowable = -2f;
            float maxAllowable = 2f;

            // find highest and lowest fert terrains overall
            patchTerrains.Sort((x, y) => x.fertility.CompareTo(y.fertility));

            //TerrainDef minFert = patchTerrains.First();
            TerrainDef maxFert = patchTerrains.Last();



            for (int index = 0; index < newTerrain.terrainPatchMakers.Count; index++)
            {
                TerrainPatchMaker p = newTerrain.terrainPatchMakers[index];

                // find patchmakers that use those terrains
                if (p.thresholds.Any(t => t.terrain == maxFert))                    
                {
                    // sort terrains by min
                    p.thresholds.Sort((x, y) => x.min.CompareTo(y.min));

                    // Make new list:
                    List<TBF> listTbf = new List<TBF>();
                    float current = -999f;

                    for (int i = 0; i < p.thresholds.Count; i++)
                    {
                        // if current == terrain min, add terrain to list
                        // set current = terrain max
                        // if terrain is the minFert or maxFert, change size appropriately
                        TBF tbf = new TBF();
                        TBF placeholder = new TBF();
                        if (!Mathf.Approximately(current, p.thresholds[i].min))
                        {
                            //placeholder when needed
                            placeholder.size = Math.Min(p.thresholds[i].min, maxAllowable) - Math.Max(current, minAllowable);
                            current = p.thresholds[i].min;
                            listTbf.Add(placeholder);
                        }
                        
                        // real thing
                        current = p.thresholds[i].max;

                        tbf.thresh = p.thresholds[i];
                        tbf.size = Math.Min(p.thresholds[i].max, maxAllowable) - Math.Max(p.thresholds[i].min, minAllowable);

                        if (tbf.thresh.terrain == maxFert)
                        {
                            tbf.size *= settings.terrainFert;
                        }
                        
                        listTbf.Add(tbf);
                    }

                    // add extra placeholder at end if needed
                    if(current < maxAllowable)
                    {
                        listTbf.Add(new TBF() { size = maxAllowable - current });
                    }

                    // shift list to size
                    float rangeSize = maxAllowable - minAllowable;
                    float rangeSizeNew = listTbf.Sum(t => t.size);
                    float ratio = rangeSize / rangeSizeNew;

                    // make new threshes
                    List<TerrainThreshold> newThreshes = new List<TerrainThreshold>();
                    float curValue = 0f;
                    foreach (TBF tbf in listTbf)
                    {
                        if(tbf.thresh != null)
                        {
                            TerrainThreshold thresh = new TerrainThreshold()
                            {
                                terrain = tbf.thresh.terrain,
                                min = curValue,
                                max = curValue + ratio * tbf.size
                            };
                            newThreshes.Add(thresh);
                            curValue = thresh.max;
                        }
                        else
                        {
                            curValue += ratio * tbf.size;
                        }
                    }

                    foreach (TerrainThreshold t in newThreshes)
                    {
                        t.min += minAllowable;
                        t.max += minAllowable;
                    }

                    // stretch to cover full range
                    newThreshes.Sort((x, y) => x.min.CompareTo(y.min));
                    
                    if(Mathf.Approximately(newThreshes[0].min, minAllowable))
                    {
                        newThreshes[0].min = -999f;
                    }
                    if (Mathf.Approximately(newThreshes.Last().max, maxAllowable))
                    {
                        newThreshes.Last().max = 999f;
                    }

                    //output
                    newTerrain.terrainPatchMakers[index].thresholds = newThreshes;



                }

            }
     

        }


    }
}
