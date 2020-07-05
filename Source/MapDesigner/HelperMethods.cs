using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MapDesigner
{

    public static class HelperMethods
    {
        public static float GetHillSize()
        {
            return LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSize;
        }

        public static float GetHillSmoothness()
        {
            return LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSmoothness;
        }


        public static void InitBiomeDefaults()
        {
            Dictionary<string, BiomeDefault>  biomeDefaults = new Dictionary<string, BiomeDefault>();

            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                BiomeDefault biodef = new BiomeDefault();
                biodef.animalDensity = biome.animalDensity;
                biodef.plantDensity = biome.plantDensity;
                biodef.wildPlantRegrowDays = biome.wildPlantRegrowDays;
                biodef.terrain = new TerrainDefault()
                {
                    terrainsByFertility = biome.terrainsByFertility,
                    terrainPatchMakers = biome.terrainPatchMakers
                };

                biomeDefaults.Add(biome.defName, biodef);
            }

            LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().biomeDefaults = biomeDefaults;

            Dictionary<string, FloatRange> densityDefaults = new Dictionary<string, FloatRange>();

            GenStepDef step = DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple");
            densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

            step = DefDatabase<GenStepDef>.GetNamed("ScatterShrines");
            densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

            step = DefDatabase<GenStepDef>.GetNamed("SteamGeysers");
            densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

            LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityDefaults = densityDefaults;
        }


        public static void ApplyBiomeSettings()
        {
            // densities
            Dictionary<string, BiomeDefault> biomeDefaults = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().biomeDefaults;

            float densityPlant = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityPlant;
            float densityAnimal = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityAnimal;

            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                biome.animalDensity = biomeDefaults[biome.defName].animalDensity * densityAnimal;

                biome.plantDensity = biomeDefaults[biome.defName].plantDensity * densityPlant;

                if (biome.plantDensity > 1f)
                {
                    biome.wildPlantRegrowDays = biomeDefaults[biome.defName].wildPlantRegrowDays / biome.plantDensity;
                    biome.plantDensity = 1f;
                }
            }

            // ruins
            Dictionary<string, FloatRange> densityDefaults = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityDefaults;
            float densityRuins = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityRuins;
            if (densityRuins > 1)
            {
                densityRuins = (float)Math.Pow(densityRuins, 3);
            }
            (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterRuinsSimple"].min * densityRuins;
            (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterRuinsSimple"].max * densityRuins;

            // ancient dangers
            float densityDanger = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityDanger;
            if (densityDanger > 1)
            {
                densityDanger = (float)Math.Pow(densityDanger, 4);
            }
            (DefDatabase<GenStepDef>.GetNamed("ScatterShrines").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterShrines"].min * densityDanger;
            (DefDatabase<GenStepDef>.GetNamed("ScatterShrines").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterShrines"].max * densityDanger;


            // geysers
            float densityGeyser = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityGeyser;
            if (densityGeyser > 1)
            {
                densityGeyser = (float)Math.Pow(densityGeyser, 2);
            }

            (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["SteamGeysers"].min * densityGeyser;
            (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["SteamGeysers"].max * densityGeyser;


            // rivers
            float sizeRiver = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().sizeRiver;
            float widthOnMap = 6;
            foreach (RiverDef river in DefDatabase<RiverDef>.AllDefs)
            {
               switch(river.defName)
               {
                    case "HugeRiver":
                        widthOnMap = 30f;
                        break;
                    case "LargeRiver":
                        widthOnMap = 14f;
                        break;
                    case "River":
                        widthOnMap = 6f;
                        break;
                    case "Creek":
                        widthOnMap = 4f;
                        break;
               }

                river.widthOnMap = widthOnMap * sizeRiver;
            }

            // terrain
            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                if (!biome.terrainsByFertility.NullOrEmpty())
                {
                    TerrainDefault newTerrain;

                    if (biome.defName.Contains("Island"))
                    {
                        newTerrain = StretchTerrains(biomeDefaults[biome.defName].terrain, -.20f, 17.0f);
                    }
                    else
                    {
                        newTerrain = StretchTerrains(biomeDefaults[biome.defName].terrain, -.20f, 1.20f);
                    }
                    biome.terrainsByFertility = newTerrain.terrainsByFertility;
                    //biome.terrainPatchMakers = newTerrain.terrainPatchMakers;
                    if (biome.defName.Contains("Arid"))
                    {
                        Log.Message("Arid Shrubland");
                        foreach (TerrainThreshold t in newTerrain.terrainsByFertility)
                        {
                            Log.Message(String.Format("Terrain: {0} | min={1}, max={2}", t.terrain.defName, t.min, t.max));
                        }
                    }
                }
            }

        }

        public static float GetRiverDirection()
        {
            return 0f;
        }
    

        public static TerrainDefault StretchTerrains(TerrainDefault oldTerrain, float minMapFert, float maxMapFert)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

            bool changeTbf = false;
            bool changePatchMakers = false;

            List<TerrainThreshold> oldTbf = oldTerrain.terrainsByFertility;
            List<TerrainThreshold> newTbf = new List<TerrainThreshold>();

            List<TerrainPatchMaker> newPatchMakers = oldTerrain.terrainPatchMakers;
            float rangeSize = maxMapFert - minMapFert;

            List<TBF> listTbf = new List<TBF>();

            for (int i = 0; i < oldTbf.Count(); i++)
            {
                TBF item = new TBF()
                {
                    fertRank = i,
                    thresh = oldTbf[i],
                    size = Math.Min(oldTbf[i].max, maxMapFert) - Math.Max(oldTbf[i].min, minMapFert)
                };
                listTbf.Add(item);
            }

            if (listTbf.Where(t => !t.thresh.terrain.IsWater).Count() >= 2)
            {
                changeTbf = true;
                // the actual adjustments
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
                //newTbf = newTbf.OrderBy(t => t.min).ToList();
                newTbf.Sort((x, y) => x.min.CompareTo(y.min));
                newTbf[0].min = -999f;
                newTbf.Last().max = 999f;

                // adjust patchmaker min max
                newTbf.Sort((x, y) => x.min.CompareTo(y.min));
                oldTbf.Sort((x, y) => x.min.CompareTo(y.min));

                SimpleCurve curve = new SimpleCurve();
                for(int i = 0; i < oldTbf.Count(); i++)
                {
                    curve.Add(new CurvePoint(oldTbf[i].min, newTbf[i].min));
                }
                curve.Add(new CurvePoint(oldTbf.Last().max, newTbf.Last().max));

                foreach(TerrainPatchMaker patchMaker in newPatchMakers)
                {
                    patchMaker.minFertility = curve.Evaluate(patchMaker.minFertility);
                    patchMaker.maxFertility = curve.Evaluate(patchMaker.maxFertility);
                }

            }

            // TerrainPatchMaker adjustments?








            TerrainDefault newTerrain = new TerrainDefault()
            {
                terrainsByFertility = newTbf,
                terrainPatchMakers = newPatchMakers
            };

            return newTerrain;
        }


        public static float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);
            dist = (float)Math.Sqrt(xDist + zDist);

            return dist;
        }

        public static List<IntVec3> GenCircle(Map map, IntVec3 center, float radius)
        {
            List<IntVec3> circle = new List<IntVec3>();

            foreach (IntVec3 current in map.AllCells)
            {
                if (DistanceBetweenPoints(center, current) < radius)
                {
                    circle.Add(current);
                }
            }

            return circle;
        }

    }
}
