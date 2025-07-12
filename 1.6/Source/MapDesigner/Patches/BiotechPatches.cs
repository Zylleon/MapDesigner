using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDesigner.Patches
{
    //[HarmonyPatch(typeof(RimWorld.GenStep_PoluxTrees))]
    //[HarmonyPatch(nameof(RimWorld.GenStep_PoluxTrees.DesiredTreeCountForMap))]
    static class PoluxTreesPatch
    {
        static void Postfix(ref int __result)
        {
            float poluxCount = MapDesignerMod.mod.settings.densityPoluxTrees;
            __result *= (int)poluxCount;
        }
    }


    //[HarmonyPatch(typeof(Verse.PollutionUtility))]
    //[HarmonyPatch(nameof(Verse.PollutionUtility.PolluteMapToPercent))]
    static class PollutionLevelPatch
    {
        static void Prefix(ref float mapPollutionPercent)
        {
            float pollutePct = (float)Math.Pow(MapDesignerMod.mod.settings.pollutionLevel, 3);
            mapPollutionPercent *= pollutePct;
            //__result *= (int)poluxCount;
        }
    }
}
