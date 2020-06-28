using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;


namespace MapDesigner.Patches
{
    [HarmonyPatch(typeof(RimWorld.RiverMaker))]
    [HarmonyPatch(nameof(RimWorld.RiverMaker.TerrainAt))]
    static class RiverBankPatch
    {
        static void Postfix(ref TerrainDef __result, ModuleBase ___generator, float ___surfaceLevel, IntVec3 loc, bool recordForValidation = false)
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
            }
        }
    }


}
