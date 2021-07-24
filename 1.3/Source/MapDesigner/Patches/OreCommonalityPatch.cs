using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;


namespace MapDesigner.Patches
{
    //[HarmonyPatch(typeof(RimWorld.GenStep_ScatterLumpsMineable), "ChooseThingDef")]
    //internal static class OreCommonalityPatch
    //{

        //static bool Prefix(GenStep_ScatterLumpsMineable __instance, ref ThingDef __result)
        //{
        //    Log.Message("Running prefix");
        //    if (__instance.forcedDefToScatter != null)
        //    {
        //        Log.Message("Forced def to scatter");
        //        __result = __instance.forcedDefToScatter;
        //        return false;

        //    }

        //    Log.Message("No forced scatterables");

        //    __result =  DefDatabase<ThingDef>.AllDefs.RandomElementByWeightWithFallback(delegate (ThingDef d)
        //    {
        //        if (d.building == null)
        //        {
        //            return 0f;
        //        }

        //        //return (d.building.mineableThing != null && d.building.mineableThing.BaseMarketValue > __instance.maxValue) ? 0f : d.building.mineableScatterCommonality;

        //        Log.Message(String.Format("Def: {0} } commonality {1}", d.defName, MapDesignerMod.mod.settings.oreCommonality[d.defName]));

        //        return (d.building.mineableThing != null && d.building.mineableThing.BaseMarketValue > __instance.maxValue) ? 0f : (d.building.mineableScatterCommonality * MapDesignerMod.mod.settings.oreCommonality[d.defName]);

        //    });

        //    return false;
        //}
    //}



    //// Counts steel and puts the number in the debug log.
    //// Useful for checking ore spawns
    //[HarmonyPatch(typeof(GenStep_FindPlayerStartSpot), nameof(GenStep_FindPlayerStartSpot.Generate))]
    //internal static class CouuntMinableSteel
    //{
    //    internal static void Postfix(Map map, GenStepParams parms)
    //    {
    //        int steelCount = map.spawnedThings.Count(thing => thing.def == ThingDefOf.MineableSteel);
    //        Log.Message(String.Format("Steel: {0}", steelCount));

    //        int goldCount = map.spawnedThings.Count(thing => thing.def == ThingDefOf.MineableGold);
    //        Log.Message(String.Format("Gold : {0}", goldCount));
    //    }
    //}



}
