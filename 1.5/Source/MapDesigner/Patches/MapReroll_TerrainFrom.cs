using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
//using MapReroll;


namespace MapDesigner.Patches
{
    static class MapReroll_TerrainFrom
    {
        static bool Prefix(IntVec3 c, Map map, float fertility, ref TerrainDef __result)
        {
            if (fertility < -1000)
            {
                __result = Feature.Feature_TerrainFrom.TerrainFromValue(fertility);
                return false;
            }
            return true;
        }
    }
}
