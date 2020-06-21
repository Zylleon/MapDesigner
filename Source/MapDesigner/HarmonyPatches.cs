using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.Noise;
using UnityEngine;

namespace MapDesigner
{
    [StaticConstructorOnStartup]
    public static class MapDesigner
    {
        static MapDesigner()
        {
            Harmony harmony = new Harmony("zylle.MapDesigner");

            harmony.PatchAll();


            //MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_Terrain), "GenerateRiver");
            //HarmonyMethod transpiler = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("RiverDirectionTranspiler"));
            //harmony.Patch(targetmethod, null, null, transpiler);

            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_Terrain), "TerrainFrom");
            HarmonyMethod postfix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("RiverBeachPostfix"));
            harmony.Patch(targetmethod, null, postfix);

            HelperMethods.InitBiomeDefaults();
            HelperMethods.ApplyBiomeSettings();
        }
    }


    static class HarmonyPatches
    {

        public static IEnumerable<CodeInstruction> RiverDirectionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            int startIndex = -1;
            int dirIndex = -1;
            MethodInfo test = AccessTools.Method(typeof(WorldGrid), "GetHeadingFromTo", new Type[] { typeof(Int32), typeof(Int32) });


            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ret && startIndex == -1)
                {
                    Log.Message("start on line " + (i + 1));

                    startIndex = i + 1;
                }

                if (codes[i].opcode == OpCodes.Callvirt)
                {
                    //Log.Message("Line " + i + " ..... operand " + codes[i].operand);
                    if ((MethodInfo)codes[i].operand == test)
                    {
                        Log.Message("FOUND IT ON LINE " + i);
                        dirIndex = i;
                    }
                }
            }

            //codes[startIndex].opcode = OpCodes.Nop;
            //codes[dirIndex] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetRiverDirection)));

            Log.Message("Start index is " + startIndex);
            Log.Message("dir index is " + dirIndex);

            if (startIndex > -1 && dirIndex > -1)
            {
                // we cannot remove the first code of our range since some jump actually jumps to
                // it, so we replace it with a no-op instead of fixing that jump (easier).
                //codes[startIndex].opcode = OpCodes.Nop;
                //codes.RemoveRange(startIndex + 1, endIndex - startIndex - 1);

                codes[startIndex].opcode = OpCodes.Ldarg_1;
                codes[dirIndex] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetRiverDirection)));

                codes.RemoveRange(startIndex + 1, dirIndex - startIndex - 2);

            }

            return codes.AsEnumerable();
        }


        /// <summary>
        /// GenStep_Terrain.TerrainFrom
        /// lets river beaches override coast beaches
        /// </summary>
        /// <returns></returns>
        public static void RiverBeachPostfix(IntVec3 c, Map map, float elevation, float fertility, RiverMaker river, bool preferSolid, ref TerrainDef __result)
        {
            if (!MapDesignerSettings.flagRiverBeach)
            {
                return;
            }

            if (river.TerrainAt(c, true) != null && BeachMaker.BeachTerrainAt(c, map.Biome)?.IsWater != true)
            {
                __result = river.TerrainAt(c, true);
            }
        }

    }


        [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    internal static class MountainSettingsPatch
    {
        /// <summary>
        /// Mountain size and smoothness
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            int hillSizeIndex = -1;
            int hillSmoothnessIndex = -1;
            float result = -1f;

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R8)
                {
                    if (float.TryParse(codes[i].operand.ToString(), out result))
                    {
                        if (hillSmoothnessIndex == -1 && result == 2f)
                        {
                            hillSmoothnessIndex = i;
                        }
                        if (hillSizeIndex == -1 && result == 0.021f)
                        {
                            hillSizeIndex = i;
                        }
                    }
                }
                if (hillSizeIndex != -1 && hillSmoothnessIndex != -1)
                {
                    break;
                }
            }
            //codes[8] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSize)));
            //codes[9] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSmoothness)));
            codes[hillSizeIndex] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSize)));
            codes[hillSmoothnessIndex] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSmoothness)));

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Mountain amount
        /// Natural hill distribution
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        static void Postfix(Map map, GenStepParams parms)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();
            // hill size
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            float hillAmount = settings.hillAmount;

            foreach (IntVec3 current in map.AllCells)
            {
                elevation[current] *= hillAmount;
            }

            // natural distribution
            if (MapDesignerSettings.flagHillClumping)
            {
                float hillSize = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSize;

                if (hillSize > 0.022f)       // smaller than vanilla only, else skip this step
                {
                    float clumpSize = Rand.Range(0.01f, Math.Min(0.04f, hillSize));
                    float clumpStrength = Rand.Range(0.3f, 0.7f);

                    ModuleBase hillClumping = new Perlin(clumpSize, 0.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Low);

                    foreach (IntVec3 current in map.AllCells)
                    {
                        elevation[current] += clumpStrength * hillClumping.GetValue(current);
                    }

                }

            }


            // pushes hills away from center
            if (MapDesignerSettings.flagHillRadial)
            {
                IntVec3 center = map.Center;
                int size = map.Size.x / 2;
                float centerSize = settings.hillRadialSize * size;
                float multiplier = 1.2f * settings.hillRadialAmt / size;
                //Log.Message("Pushing hills with value " + settings.hillRadialAmt);
                foreach (IntVec3 current in map.AllCells)
                {
                    float distance = (float)Math.Sqrt(Math.Pow(current.x - center.x, 2) + Math.Pow(current.z - center.z, 2));
                    elevation[current] *= (1f + (settings.hillRadialAmt * (distance - centerSize) / size));
                }
            }
        }
    }


    [HarmonyPatch(typeof(RimWorld.GenStep_ScatterLumpsMineable))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ScatterLumpsMineable.Generate))]
    internal static class OreDensityPatch
    {
        static bool Prefix(ref GenStep_ScatterLumpsMineable  __instance)
        {
            float densityOre = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityOre;
            if(densityOre > 1f)
            {
                densityOre *= densityOre;
            }

            __instance.countPer10kCellsRange.min *= densityOre;
            __instance.countPer10kCellsRange.max *= densityOre;

            return true;
        }
    }


    [HarmonyPatch(typeof(RimWorld.Planet.World))]
    [HarmonyPatch(nameof(RimWorld.Planet.World.HasCaves))]
    static class BiomeMapSettings_Caves
    {
        static bool Prefix(int tile, ref bool __result, ref World __instance)
        {
            if (!MapDesignerSettings.flagCaves)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(RimWorld.Planet.World))]
    [HarmonyPatch(nameof(RimWorld.Planet.World.NaturalRockTypesIn))]
    internal static class RockTypesPatch
    {
        static void Finalizer(int tile, ref IEnumerable<ThingDef> __result)
        {
            Rand.PushState();
            Rand.Seed = tile;

            IntRange rockTypeRange = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().rockTypeRange;

            int num = Rand.RangeInclusive(rockTypeRange.min, rockTypeRange.max);

            //List<ThingDef> rocks = new List<ThingDef>();

            List<ThingDef> rocks = __result.ToList();

            // If it's a modded biome with special stone, check that the user allows this to change before continuing
            if (__result.Count() == 1 && !MapDesignerSettings.flagBiomeRocks)
            {
                Rand.PopState();

                return;
            }

            // shorten if the list is already long enough
            if (__result.Count() >= num)
            {
                __result = __result.Take(num);
            }
            else
            {
                //Rand.PushState();
                //Rand.Seed = tile;
                List<ThingDef> list = (from d in DefDatabase<ThingDef>.AllDefs
                                       where d.category == ThingCategory.Building && d.building.isNaturalRock && !d.building.isResourceRock && !d.IsSmoothed && !rocks.Contains(d)
                                       select d).ToList<ThingDef>();
                while (rocks.Count() < num && list.Count() > 0)
                {
                    ThingDef item = list.RandomElement<ThingDef>();
                    list.Remove(item);
                    rocks.Add(item);
                }


                __result = rocks.ToList();
            }

            //// correctly counts rock type on original map
            //if (__result.Count() > 1)
            //{
            //    test.Add(ThingDefOf.Sandstone);
            //}
            //else
            //{
            //    test.Add(ThingDefOf.Granite);
            //}

            __result = rocks.AsEnumerable();
            Rand.PopState();

        }
    }


    [HarmonyPatch(typeof(RimWorld.GenStep_AnimaTrees))]
    [HarmonyPatch(nameof(RimWorld.GenStep_AnimaTrees.DesiredTreeCountForMap))]
    static class AnimaTreePatch
    {
        static void Postfix(ref int __result)
        {
            float animaCount = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().animaCount;
            __result *= (int)animaCount;
        }
    }


    [HarmonyPatch(typeof(RimWorld.RiverMaker))]
    [HarmonyPatch(nameof(RimWorld.RiverMaker.TerrainAt))]
    static class FertileRivers
    {
        static void Postfix(ref TerrainDef __result, /*ref RiverMaker __instance,*/ ModuleBase ___generator, float ___surfaceLevel, IntVec3 loc, bool recordForValidation = false )
        {
            if (!MapDesignerSettings.flagRiverBeach)
            {
                return;
            }

            float value = ___generator.GetValue(loc);
            float num = ___surfaceLevel - Mathf.Abs(value);
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

            if (num < 0 && num > 0 - settings.riverBeachSize)
            {
                __result = TerrainDef.Named(settings.riverShore);

                //__result = TerrainDef.Named("SoilRich");
            }
        }
    }






    /*
    // TEST RIVER GENSTEPS
   [HarmonyPatch(typeof(RimWorld.GenStep_Terrain))]
   [HarmonyPatch(nameof(RimWorld.GenStep_Terrain.Generate))]
   static class TestRiverPatch
   {
       static bool Prefix(Map map, GenStepParams parms)
       {
           (new ZMD_GenStep_Terrain()).Generate(map, parms);
           return false;
       }
   }
   */


}
