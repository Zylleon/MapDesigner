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
        }

        public static float GetRiverDirection()
        {
            return 0f;
        }
    

        public static TerrainDefault StretchTerrains(TerrainDefault input, Map map)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

            List<TerrainThreshold> oldThreshes = map.Biome.terrainsByFertility;
            List<TerrainThreshold> newThreshes = new List<TerrainThreshold>();

            // Find the boundaries
            MapGenFloatGrid fertility = MapGenerator.Fertility;

            float minMapFert = 999f;
            float maxMapFert = -999f;
            foreach (IntVec3 current in map.AllCells)
            {
                maxMapFert = Math.Max(maxMapFert, fertility[current]);
                minMapFert = Math.Min(minMapFert, fertility[current]);
            }

            float rangeSize = maxMapFert - minMapFert;

            List<TBF> listTbf = new List<TBF>();

            oldThreshes = oldThreshes.OrderBy(t => t.terrain.fertility).ToList();

            for (int i = 0; i < oldThreshes.Count(); i++)
            {
                TBF item = new TBF()
                {
                    fertRank = i,
                    thresh = oldThreshes[i],
                    size = Math.Min(oldThreshes[i].max, maxMapFert) - Math.Max(oldThreshes[i].min, minMapFert)
                };
                listTbf.Add(item);
            }


            // the actual adjustments
            listTbf.Where(t => !t.thresh.terrain.IsWater).First().size /= settings.terrainFert;
            listTbf.Where(t => !t.thresh.terrain.IsWater).Last().size *= settings.terrainFert;

            float rangeSizeNew = listTbf.Sum(t => t.size);
            float ratio = rangeSize / rangeSizeNew;

            // make new threshes
            // it's still sorted, go in order
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
                newThreshes.Add(thresh);
            }

            // reset to original range, hopefully
            foreach (TerrainThreshold t in newThreshes)
            {
                t.min += minMapFert;
                t.max += minMapFert;
            }

            // stretch to cover full range. ideally this doesn't matter, but it's safer
            newThreshes = newThreshes.OrderBy(t => t.min).ToList();

            newThreshes[0].min = -999f;
            newThreshes.Last().max = 999f;


            foreach (TerrainThreshold t in newThreshes)
            {
                Log.Message(String.Format("Terrain: {0} | min={1}, max={2}", t.terrain.defName, t.min, t.max));
            }
            //map.Biome.terrainsByFertility = newThreshes;

            input.terrainsByFertility = newThreshes;

            return input;
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
