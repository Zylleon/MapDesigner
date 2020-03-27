using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Verse;
using HarmonyLib;
using RimWorld;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld.Planet;


namespace MapDesigner
{
    [StaticConstructorOnStartup]
    public static class MapDesigner
    {
        static MapDesigner()
        {
            new Harmony("zylle.MapDesigner").PatchAll();

            HelperMethods.InitBiomeDefaults();
            HelperMethods.ApplyBiomeSettings();
        }
    }


    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    internal static class MountainSettingsPatch
    {
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


        static void Postfix(Map map, GenStepParams parms)
        {
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            float hillAmount = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillAmount;

            foreach (IntVec3 current in map.AllCells)
            {
                elevation[current] *= hillAmount;
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

}
