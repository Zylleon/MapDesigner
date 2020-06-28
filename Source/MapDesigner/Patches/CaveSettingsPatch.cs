using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MapDesigner.Patches
{

    [HarmonyPatch(typeof(RimWorld.Planet.World))]
    [HarmonyPatch(nameof(RimWorld.Planet.World.HasCaves))]
    static class CaveSettingsPatch
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
