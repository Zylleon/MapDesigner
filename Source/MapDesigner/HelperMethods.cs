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
                    terrainsByFertility = new List<TerrainThreshold>(biome.terrainsByFertility),
                    terrainPatchMakers = new List<TerrainPatchMaker>(biome.terrainPatchMakers)
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
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

            // densities
            Dictionary<string, BiomeDefault> biomeDefaults = settings.biomeDefaults;

            float densityPlant = settings.densityPlant;
            float densityAnimal = settings.densityAnimal;

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
            Dictionary<string, FloatRange> densityDefaults = settings.densityDefaults;
            float densityRuins = settings.densityRuins;
            if (densityRuins > 1)
            {
                densityRuins = (float)Math.Pow(densityRuins, 3);
            }
            (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterRuinsSimple"].min * densityRuins;
            (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterRuinsSimple"].max * densityRuins;

            // ancient dangers
            float densityDanger = settings.densityDanger;
            if (densityDanger > 1)
            {
                densityDanger = (float)Math.Pow(densityDanger, 4);
            }
            (DefDatabase<GenStepDef>.GetNamed("ScatterShrines").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterShrines"].min * densityDanger;
            (DefDatabase<GenStepDef>.GetNamed("ScatterShrines").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterShrines"].max * densityDanger;


            // geysers
            float densityGeyser = settings.densityGeyser;
            if (densityGeyser > 1)
            {
                densityGeyser = (float)Math.Pow(densityGeyser, 2);
            }

            (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["SteamGeysers"].min * densityGeyser;
            (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["SteamGeysers"].max * densityGeyser;


            // rivers
            float widthOnMap = 6;
            foreach (RiverDef river in DefDatabase<RiverDef>.AllDefs)
            {
                switch (river.defName)
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

                river.widthOnMap = widthOnMap * settings.sizeRiver;
            }

            // terrain
            if (Math.Abs(settings.terrainFert - 1f) > 0.1 || Math.Abs(settings.terrainWater - 1f) > 0.1)
            {
                Log.Message(String.Format("Map Designer Terrain settings: fertility: {0} | Water {1}", Math.Round(100 * settings.terrainFert), Math.Round(100 * settings.terrainWater)));
                foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
                {
                    if (!biome.terrainsByFertility.NullOrEmpty())
                    {
                        try
                        {
                            TerrainDefault newTerrain;
                            if (biome.defName.Contains("BiomesIsland"))
                            {
                                newTerrain = TerrainUtility.StretchTerrainFertility(biomeDefaults[biome.defName].terrain, -.20f, 17.0f);
                            }
                            else
                            {
                                newTerrain = TerrainUtility.StretchTerrainFertility(biomeDefaults[biome.defName].terrain, -.20f, 1.20f);
                            }
                            biome.terrainsByFertility = newTerrain.terrainsByFertility;
                            biome.terrainPatchMakers = newTerrain.terrainPatchMakers;
                        }

                        catch (Exception e)
                        {
                            Log.Message("ERROR applying terrain settings to " + biome.defName);
                            Log.Message(e.Message);

                            TerrainDefault dictEntry = settings.biomeDefaults[biome.defName].terrain;
                            Log.Message("Terrains by fertility");
                            foreach (TerrainThreshold t in dictEntry.terrainsByFertility)
                            {
                                Log.Message(String.Format("- {0} .... {1} | {2}", t.terrain.defName, Math.Round(t.min, 2), Math.Round(t.max, 2)));
                            }
                            for (int i = 0; i < dictEntry.terrainPatchMakers.Count(); i++)
                            {
                                TerrainPatchMaker p = dictEntry.terrainPatchMakers[i];
                                Log.Message(String.Format("Patchmaker #{0} | min {1} | max {2}", i, p.minFertility, p.maxFertility));
                                foreach (TerrainThreshold t in p.thresholds)
                                {
                                    Log.Message(String.Format("--- {0} | {1} | {2}", t.terrain.defName, Math.Round(t.min, 2), Math.Round(t.max, 2)));
                                }
                            }
                        }
                    }
                }
            }

        }

        public static float GetRiverDirection()
        {
            return 0f;
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
