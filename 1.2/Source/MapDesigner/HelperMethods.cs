﻿using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MapDesigner
{
    public static class HelperMethods
    {
        public static float GetHillSize()
        {
            return MapDesignerMod.mod.settings.hillSize;
        }

        public static float GetHillSmoothness()
        {
            return MapDesignerMod.mod.settings.hillSmoothness;
        }


        public static void InitBiomeDefaults()
        {
            Log.Message("[Map Designer] Finding biomes...");
            MapDesignerSettings settings = MapDesignerMod.mod.settings;

            // Biomes
            Dictionary<string, BiomeDefault>  biomeDefaults = new Dictionary<string, BiomeDefault>();
            try
            {
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
                settings.biomeDefaults = biomeDefaults;
            }
            catch
            {
                Log.Message("[Map Designer] Could not initialize biome defaults");
            }


            // Densities
            Dictionary<string, FloatRange> densityDefaults = new Dictionary<string, FloatRange>();
            try
            {
                GenStepDef step = DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple");
                densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

                step = DefDatabase<GenStepDef>.GetNamed("ScatterShrines");
                densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

                step = DefDatabase<GenStepDef>.GetNamed("SteamGeysers");
                densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

                settings.densityDefaults = densityDefaults;
            }
            catch
            {
                Log.Message("[Map Designer] Could not initialize density defaults");
            }

            // Rocks
            try
            {
                if (settings.allowedRocks.EnumerableNullOrEmpty())
                {
                    settings.allowedRocks = new Dictionary<string, bool>();
                }
                List<ThingDef> list = GetRockList();
                foreach (ThingDef rock in list)
                {
                    if (!settings.allowedRocks.ContainsKey(rock.defName))
                    {
                        settings.allowedRocks.Add(rock.defName, true);
                    }
                }
                settings.rockTypeRange.max = Math.Min(list.Count, settings.rockTypeRange.max);
                settings.rockTypeRange.min = Math.Min(list.Count, settings.rockTypeRange.min);
            }
            catch
            {
                Log.Message("[Map Designer] Could not initialize rock types");
                settings.rockTypeRange.max = 3;
                settings.rockTypeRange.min = 2;
            }

            // Ore
            try
            {
                if (settings.oreCommonality.EnumerableNullOrEmpty())
                {
                    settings.oreCommonality = new Dictionary<string, float>();
                }
                List<ThingDef> list = GetMineableList();
                foreach (ThingDef ore in list)
                {
                    if (!settings.oreCommonality.ContainsKey(ore.defName))
                    {
                        settings.oreCommonality.Add(ore.defName, 1f);
                    }
                }
               
            }
            catch
            {
                Log.Message("[Map Designer] Could not initialize ore types");
            }

        }


        public static void ApplyBiomeSettings()
        {
            Log.Message("[Map Designer] Applying settings");
            MapDesignerSettings settings = MapDesignerMod.mod.settings;

            // densities
            Dictionary<string, BiomeDefault> biomeDefaults = settings.biomeDefaults;

            float densityPlant = settings.densityPlant;
            float densityAnimal = settings.densityAnimal;
            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                try
                {
                    biome.animalDensity = biomeDefaults[biome.defName].animalDensity * densityAnimal;
                    biome.plantDensity = biomeDefaults[biome.defName].plantDensity * densityPlant;

                    if (biome.plantDensity > 1f)
                    {
                        biome.wildPlantRegrowDays = biomeDefaults[biome.defName].wildPlantRegrowDays / biome.plantDensity;
                        biome.plantDensity = 1f;
                    }
                }
                catch
                {
                    Log.Message("[Map Designer] ERROR applying plant and animal settings to " + biome.defName);
                }
            }

            Dictionary<string, FloatRange> densityDefaults = settings.densityDefaults;

            // ruins
            try
            {
                float densityRuins = settings.densityRuins;
                if (densityRuins > 1)
                {
                    densityRuins = (float)Math.Pow(densityRuins, 3);
                }
                (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterRuinsSimple"].min * densityRuins;
                (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterRuinsSimple"].max * densityRuins;
            }
            catch
            {
                Log.Message("[Map Designer] ERROR with settings: ruins");
            }

            // ancient dangers
            try
            {
                float densityDanger = settings.densityDanger;
                if (densityDanger > 1)
                {
                    densityDanger = (float)Math.Pow(densityDanger, 4);
                }
                (DefDatabase<GenStepDef>.GetNamed("ScatterShrines").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterShrines"].min * densityDanger;
                (DefDatabase<GenStepDef>.GetNamed("ScatterShrines").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterShrines"].max * densityDanger;
            }
            catch
            {
                Log.Message("[Map Designer] ERROR with settings: ancient dangers");
            }

            // geysers
            try
            {
                float densityGeyser = settings.densityGeyser;
                if (densityGeyser > 1)
                {
                    densityGeyser = (float)Math.Pow(densityGeyser, 2);
                }
                (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["SteamGeysers"].min * densityGeyser;
                (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["SteamGeysers"].max * densityGeyser;
            }
            catch
            {
                Log.Message("[Map Designer] ERROR with settings: geysers");
            }

            // rivers
            float widthOnMap = 6;
            foreach (RiverDef river in DefDatabase<RiverDef>.AllDefs)
            {
                try
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
                catch
                {
                    Log.Message("[Map Designer] ERROR with settings: river width : " + river.defName);
                }
            }

            // terrain
            if (Math.Abs(settings.terrainFert - 1f) > 0.05 || Math.Abs(settings.terrainWater - 1f) > 0.05)
            {
                Log.Message(String.Format("[Map Designer] Terrain settings: fertility: {0} | Water {1}", Math.Round(100 * settings.terrainFert), Math.Round(100 * settings.terrainWater)));
                foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
                {
                    if (!biome.terrainsByFertility.NullOrEmpty())
                    {
                        //Log.Message("Doing adjustments for " + biome.defName);
                        try
                        {
                       
                            TerrainDefault newTerrain;
                            float minFertBound = -0.2f;
                            float maxFertBound = 1.2f;

                            if (ModsConfig.IsActive("BiomesTeam.BiomesCore"))
                            {
                                maxFertBound = HelperMethods.GetMaxFertByBiome(biome);
                            }
                            newTerrain = TerrainUtility.StretchTerrainFertility(biomeDefaults[biome.defName].terrain, minFertBound, maxFertBound);


                            biome.terrainsByFertility = newTerrain.terrainsByFertility;
                            biome.terrainPatchMakers = newTerrain.terrainPatchMakers;
                        }

                        catch (Exception e)
                        {
                            Log.Message("[Map Designer] ERROR with settings: terrain : " + biome.defName);
                            Log.Message(e.Message);

                            TerrainDefault dictEntry = settings.biomeDefaults[biome.defName].terrain;
                            Log.Message("--terrainsByFertility");
                            foreach (TerrainThreshold t in dictEntry.terrainsByFertility)
                            {
                                Log.Message(String.Format("- {0} .... {1} | {2}", t.terrain.defName, Math.Round(t.min, 2), Math.Round(t.max, 2)));
                            }
                            Log.Message("--terrainPatchMakers");
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
            else
            {
                Log.Message("[Map Designer] Terrain settings: Default");
                foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
                {
                    biome.terrainsByFertility = biomeDefaults[biome.defName].terrain.terrainsByFertility;
                    biome.terrainPatchMakers = biomeDefaults[biome.defName].terrain.terrainPatchMakers;
                }
            }
        }

        public static float GetRiverDirection(float angle)
        {
            if (MapDesignerMod.mod.settings.flagRiverDir)
            {
                angle = MapDesignerMod.mod.settings.riverDir;
            }
            return angle;
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


        public static string GetDisplayValue(float input, float power = 1)
        {
            if (input > 1)
            {
                input = (float)Math.Pow(input, power);
            }

            string output = "";
            if (input < 2)
            {
                output = String.Format("{0:0.0}", input);
            }
            else
            {
                output = String.Format("{0:0}", input);
            }
            return output;

        }


        public static List<ThingDef> GetRockList()
        {
            List<ThingDef> rockList = (from d in DefDatabase<ThingDef>.AllDefs
                                       where d.category == ThingCategory.Building && d.building.isNaturalRock && !d.building.isResourceRock && !d.IsSmoothed
                                       select d).ToList<ThingDef>();
            return rockList;
        }

        public static List<ThingDef> GetMineableList()
        {
            List<ThingDef> oreList = (from d in DefDatabase<ThingDef>.AllDefs
                                       where d.category == ThingCategory.Building && d.building.isResourceRock && d.building.mineableThing != null
                                       select d).ToList<ThingDef>();
            return oreList;
        }

        public static void ApplyMapRerollPatches()
        {
            MethodInfo targetmethod = AccessTools.Method(typeof(MapReroll.MapPreviewGenerator), "TerrainFrom");
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(Patches.MapReroll_TerrainFrom), "Prefix");
            new Harmony("zylle.MapDesigner_RerollCompat").Patch(targetmethod, prefixmethod);
        }


        public static float GetMaxFertByBiome(BiomeDef biome)
        {
            if (!biome.HasModExtension<BiomesMap>())
            {
                return 1.2f;
            }
            if (biome.GetModExtension<BiomesMap>().isOasis)
            {
                return 17f;
            }
            if (biome.GetModExtension<BiomesMap>().isIsland)
            {
                return 17f;
            }

            return 1.2f;
        }
    }
}
