using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using MapReroll;


namespace MapDesigner.Patches
{
    static class PRI_MapReroll_TerrainFrom
    {
        static bool Prefix(IntVec3 c, Map map, float fertility, ref TerrainDef __result)
        {
            if (MapDesignerMod.mod.settings.selectedFeature != MapDesignerSettings.Features.RoundIsland)
            {
                return true;
            }

            if (fertility < -998f)
            {
                __result = TerrainDefOf.WaterOceanShallow;
                return false;
            }

            else if (fertility < -994f)
            {
                __result = TerrainDefOf.Sand;
                return false;
            }

            return true;
        }
    }
}
