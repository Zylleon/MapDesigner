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
        }
    }


    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    internal static class MountainSettingsPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //CodeInstruction[] codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            //foreach (CodeInstruction instruction in codeInstructions)
            //{
            //    yield return instruction;
            //}
            //int endIndex = -1;
            //int lacunIndex = -1;
            //int freqIndex = -1;

            //MethodInfo noise = AccessTools.Method(type: typeof(Scatterer_WaterBiomeFix), name: nameof(Scatterer_WaterBiomeFix.AllowedInWaterBiome));
            //ConstructorInfo noise = AccessTools.Constructor(type: typeof(Verse.Noise.Perlin));

            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Ldc_R8)  
                {
                    //Log.Message("Found float code at line " + i);
                    //Log.Message("Value: " + codes[i].operand);
                    //if (freqIndex == -1 && (float)codes[i].operand == 0.021f)
                    //{
                    //    freqIndex = i;
                    //    Log.Message("Freq index: " + freqIndex);
                    //    codes[i] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSize)));
                    //}

                    //if (lacunIndex == -1 &&(float)codes[i].operand == 2f)
                    //{
                    //    lacunIndex = i;
                    //    Log.Message("Lacun index: " + lacunIndex);
                    //}

                    //if ((float)codes[i].operand == 2f)
                    //{

                    //}

                }
            }

            codes[8] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSize)));

            codes[9] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSmoothness)));

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

            //var world = Traverse.Create(__instance);
            //WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
            //if (!worldGrid[tile].biome.HasModExtension<ZMDBiomeModExtension>())
            //{
            //    return true;
            //}
            //bool? hasCaves = worldGrid[tile].biome.GetModExtension<ZMDBiomeModExtension>().biomeMapSettings?.caves;
            //if (hasCaves == true)
            //{
            //    __result = true;
            //    return false;
            //}
            //if (hasCaves == false)
            //{
            //    __result = true;
            //    return false;
            //}
            //return true;
        }
    }


    

}
