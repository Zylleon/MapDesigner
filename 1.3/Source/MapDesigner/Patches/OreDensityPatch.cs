using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MapDesigner.Patches
{
    [HarmonyPatch(typeof(RimWorld.GenStep_ScatterLumpsMineable))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ScatterLumpsMineable.Generate))]
    internal static class OreDensityPatch
    {
        static bool Prefix(ref GenStep_ScatterLumpsMineable __instance)
        {
            float densityOre = MapDesignerMod.mod.settings.densityOre;
            if (densityOre > 1f)
            {
                densityOre *= densityOre;
            }

            __instance.countPer10kCellsRange.min *= densityOre;
            __instance.countPer10kCellsRange.max *= densityOre;

            return true;
        }
    }



    //// Counts ores and puts the number in the debug log.
    //// Useful for checking ore spawns
    //[HarmonyPatch(typeof(GenStep_FindPlayerStartSpot), nameof(GenStep_FindPlayerStartSpot.Generate))]
    //internal static class CouuntMinableSteel
    //{
    //    internal static void Postfix(Map map, GenStepParams parms)
    //    {
    //        int countSteel = map.spawnedThings.Count(thing => thing.def == ThingDefOf.MineableSteel);
    //        Log.Message(String.Format("{0} steel", countSteel));
    //        int countGold = map.spawnedThings.Count(thing => thing.def == ThingDefOf.MineableGold);
    //        Log.Message(String.Format("{0} gold", countGold));
    //    }
    //}

}
