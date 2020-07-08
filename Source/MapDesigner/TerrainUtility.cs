using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static TerrainDefault StretchTerrainFertility(TerrainDefault old, float min, float max)
        {
            oldTerrain = old;
            minMapFert = min;
            maxMapFert = max;

            newTerrain = new TerrainDefault()
            {
                terrainsByFertility = oldTerrain.terrainsByFertility,
                terrainPatchMakers = oldTerrain.terrainPatchMakers
            };

            List<TerrainThreshold> oldTerrainsByFertility = oldTerrain.terrainsByFertility;

            List<TerrainPatchMaker> newPatchMakers = oldTerrain.terrainPatchMakers;
            
            //float rangeSize = maxMapFert - minMapFert;

            List<TBF> listTbf = new List<TBF>();

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

            if (listTbf.Where(t => !t.thresh.terrain.IsWater).Count() >= 2)
            {
                // the actual adjustments
                changeTbf = true;
                FertChangeTbf(listTbf);
            }

            // TerrainPatchMaker adjustments?

            //List<TerrainDef> patchTerrains = newPatchMakers.SelectMany(t => t.thresholds.Select(tt => tt.terrain)).Distinct().ToList();
            List<TerrainDef> patchTerrains = new List<TerrainDef>();

            // NOPE
            //newTerrain.terrainPatchMakers = newPatchMakers;

            foreach (TerrainPatchMaker p in newPatchMakers)
            {
                foreach (TerrainThreshold t in p.thresholds)
                {
                    patchTerrains.Add(t.terrain);
                }
            }
            patchTerrains = patchTerrains.Where(p => !p.IsWater).Distinct().ToList();

            // check that list has at least 2 elements !IsWater
            if (patchTerrains.Count() >= 2)
            {
                changePatchMakers = true;
                FertChangePatchMakers(patchTerrains);
            }


            // if changeTbf and changePatchMakers are both false, make alterhate adjustments


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
            for (int i = 0; i<oldTbf.Count(); i++)
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

            TerrainDef minFert = patchTerrains.First();
            TerrainDef maxFert = patchTerrains.Last();
            Log.Message(String.Format("Min: {0}.... Max: {1}", minFert.defName, maxFert.defName));

            for(int index = 0; index < newTerrain.terrainPatchMakers.Count; index++)
            {
                Log.Message("Doing patchmaker # " + index);
                TerrainPatchMaker p = newTerrain.terrainPatchMakers[index];

                // find patchmakers that use those terrains
                if(p.thresholds.Any(t => t.terrain == minFert || t.terrain == maxFert))
                {
                    // for each valid patchmaker:
                    // sort terrains by min
                    p.thresholds.Sort((x, y) => x.min.CompareTo(y.min));

                    // Make new list:
                    List<TBF> listTbf = new List<TBF>();
                    float current = -999f;

                    //while (i < p.thresholds.Count())
                    for (int i = 0; i < p.thresholds.Count; i++)
                    {
                        // if current == terrain min, add terrain to list
                        // set current = terrain max
                        // if terrain is the minFert or maxFert, change size appropriately
                        // increment index
                        TBF tbf = new TBF();
                        TBF placeholder = new TBF();
                        if (current != p.thresholds[i].min)
                        {
                            //placeholder when needed
                            Log.Message("Placeholder");
                            placeholder.size = Math.Min(p.thresholds[i + 1].min, maxAllowable) - Math.Max(current, minAllowable);
                            current = p.thresholds[i + 1].min;
                            listTbf.Add(placeholder);
                        }
                        

                        // real thing
                        
                        current = p.thresholds[i].max;

                        tbf.thresh = p.thresholds[i];
                        tbf.size = Math.Min(p.thresholds[i].max, maxAllowable) - Math.Max(p.thresholds[i].min, minAllowable);
                        if (tbf.thresh.terrain == minFert)
                        {
                            tbf.size /= settings.terrainFert;
                        }
                        else if (tbf.thresh.terrain == maxFert)
                        {
                            tbf.size *= settings.terrainFert;
                        }
                        Log.Message(String.Format("Terrain: {0} | Size: {1}", tbf.thresh.terrain, tbf.size));

                        listTbf.Add(tbf);
                        //i++;
                    }

                    // add extra blank at end if needed
                    if(current < maxAllowable)
                    {
                        listTbf.Add(new TBF() { size = maxAllowable - current });
                    }

                    // shift list to size, probably -1 to 1
                    float rangeSize = maxAllowable - minAllowable;
                    float rangeSizeNew = listTbf.Sum(t => t.size);
                    float ratio = rangeSize / rangeSizeNew;
                    Log.Message("listTbf count: " + listTbf.Count());
                    Log.Message("RangeSize " + rangeSize);
                    Log.Message("rangeSizeNew " + rangeSizeNew);
                    Log.Message("Ratio: " + ratio);
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
                            //curValue = thresh.max;
                            newThreshes.Add(thresh);
                        }
                        curValue += ratio * tbf.size;
                    }
                    Log.Message("Made new threshes - success");

                    foreach (TerrainThreshold t in newThreshes)
                    {
                        t.min += minAllowable;
                        t.max += minAllowable;
                    }

                    // stretch to cover full range
                    newThreshes.Sort((x, y) => x.min.CompareTo(y.min));
                    
                    if(newThreshes[0].min == minAllowable)
                    {
                        newThreshes[0].min = -999f;
                    }
                    if (newThreshes.Last().max == maxAllowable)
                    {
                        newThreshes.Last().max = 999f;
                    }
                    //put it back in the original
                    newTerrain.terrainPatchMakers[index].thresholds = newThreshes;
                }

                foreach(TerrainThreshold t in newTerrain.terrainPatchMakers[index].thresholds)
                {
                    Log.Message(String.Format("{0} : {1} - {2}", t.terrain.defName, t.min, t.max));
                }
            }
     

        }




    }
}
