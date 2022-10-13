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
        private static float tbfFert = -1f;
        private static float patchFert = -1f;
        private static float minMapFert = -0.20f;
        private static float maxMapFert = 1.20f;


        public static TerrainDefault StretchTerrainFertility(TerrainDefault old, float min, float max)
        {
            oldTerrain = old;
            minMapFert = min;
            maxMapFert = max;

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
                    thresh = oldTerrainsByFertility[i],
                    size = Math.Min(oldTerrainsByFertility[i].max, maxMapFert) - Math.Max(oldTerrainsByFertility[i].min, minMapFert)
                };
                listTbf.Add(item);
            }

            // the actual adjustments
            if (listTbf.Count() >= 1)
            {
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

            // check that there are terrains
            if (patchTerrains.Count() >= 1)
            {
                FertChangePatchMakers(patchTerrains);
            }
            
            return newTerrain;
        }


        private static void FertChangeTbf(List<TBF> listTbf)
        {
            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            List<TerrainThreshold> newTbf = new List<TerrainThreshold>();
            float rangeSize = maxMapFert - minMapFert;
            List<TerrainThreshold> oldTbf = oldTerrain.terrainsByFertility;

            // the actual adjustments
            listTbf.Sort((x, y) => x.thresh.terrain.fertility.CompareTo(y.thresh.terrain.fertility));
            float minFert = -1;
            float maxFert = -1;
            if (listTbf.Any(t => !t.thresh.terrain.IsWater))
            {
                if (listTbf.Where(t => !t.thresh.terrain.IsWater).Max(t => t.thresh.terrain.fertility) != listTbf.Where(t => !t.thresh.terrain.IsWater).Min(t => t.thresh.terrain.fertility))
                {
                    minFert = listTbf.Where(t => !t.thresh.terrain.IsWater).Min(t => t.thresh.terrain.fertility);
                    maxFert = listTbf.Where(t => !t.thresh.terrain.IsWater).Max(t => t.thresh.terrain.fertility);
                }
            }
            for (int i = 0; i < listTbf.Count(); i++)
            {
                //if(listTbf[i].thresh.terrain.IsWater)
                //{
                //    Log.Message(String.Format("{0} is water", listTbf[i].thresh.terrain.defName));
                //}
                //else
                //{
                //    Log.Message(String.Format("----------{0} is NOT water", listTbf[i].thresh.terrain.defName));
                //}

                //fert
                if (listTbf[i].thresh.terrain.fertility == maxFert && !listTbf[i].thresh.terrain.IsWater)
                {
                    listTbf[i].size *= settings.terrainFert;
                    //Log.Message(String.Format("stretching {0}", listTbf[i].thresh.terrain.defName));

                }
                else if (listTbf[i].thresh.terrain.fertility == minFert && !listTbf[i].thresh.terrain.IsWater)
                {
                    listTbf[i].size /= settings.terrainFert;
                    //Log.Message(String.Format("---- SHRINKING {0}", listTbf[i].thresh.terrain.defName));

                }
                //water
                if (listTbf[i].thresh.terrain.IsWater)
                {
                    listTbf[i].size *= settings.terrainWater;
                }
                else if (settings.flagTerrainWater && !listTbf[i].thresh.terrain.affordances.Contains(TerrainAffordanceDefOf.Heavy))
                {
                    listTbf[i].size *= settings.terrainWater;
                }
            }

            listTbf = listTbf.OrderBy(t => t.thresh.min).ToList();

            newTbf = SquishTerrain(listTbf, minMapFert, maxMapFert);

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
            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            float minAllowable = -1.5f;
            float maxAllowable = 1.5f;

            // find highest fert terrain overall
            patchTerrains.Sort((x, y) => x.fertility.CompareTo(y.fertility));

            float maxFert = patchTerrains.Max(t => t.fertility);

            for (int index = 0; index < newTerrain.terrainPatchMakers.Count; index++)
            {
                TerrainPatchMaker p = newTerrain.terrainPatchMakers[index];

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

                    if (tbf.thresh.terrain.fertility == maxFert)
                    {
                        tbf.size *= settings.terrainFert;
                    }

                    if (tbf.thresh.terrain.IsWater)
                    {
                        tbf.size *= settings.terrainWater;
                    }
                    else if (settings.flagTerrainWater && !tbf.thresh.terrain.affordances.Contains(TerrainAffordanceDefOf.Heavy))
                    {
                        tbf.size *= settings.terrainWater;
                    }
                    listTbf.Add(tbf);
                }

                // add extra placeholder at end if needed
                if(current < maxAllowable)
                {
                    listTbf.Add(new TBF() { size = maxAllowable - current });
                }
                newTerrain.terrainPatchMakers[index].thresholds = SquishTerrain(listTbf, minAllowable, maxAllowable);

                }


        }


        private static List<TerrainThreshold> SquishTerrain(List<TBF> listTbf, float min, float max)
        {
            float rangeSize = max - min;

            float rangeSizeNew = listTbf.Sum(t => t.size);
            float ratio = rangeSize / rangeSizeNew;

            List<TerrainThreshold> newTbf = new List<TerrainThreshold>();

            // make new threshes
            // assumes that list Tbf is pre-sorted by t => t.thresh.min
            // can't sort here because of blank tbfs (no thresh) from patchmaker shift
            float curValue = 0f;

            foreach (TBF tbf in listTbf)
            {
                if (tbf.thresh != null)
                {
                    TerrainThreshold thresh = new TerrainThreshold()
                    {
                        terrain = tbf.thresh.terrain,
                        min = curValue,
                        max = curValue + ratio * tbf.size
                    };
                    newTbf.Add(thresh);
                    curValue = thresh.max;
                }
                else
                {
                    curValue += ratio * tbf.size;
                }
            }

            // reset to original active range
            foreach (TerrainThreshold t in newTbf)
            {
                t.min += min;
                t.max += min;
            }

            // stretch to cover full range
            newTbf.Sort((x, y) => x.min.CompareTo(y.min));
            if(Mathf.Approximately(newTbf[0].min, min))
            {
                newTbf[0].min = -999f;
            }
            if (Mathf.Approximately(newTbf.Last().max, max))
            {
                newTbf.Last().max = 999f;
            }

            return newTbf;
        }

    }
}
