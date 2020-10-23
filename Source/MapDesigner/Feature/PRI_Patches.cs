using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using MapDesigner;

namespace MapDesigner.Feature
{
    [HarmonyPatch(typeof(RimWorld.GenStep_Terrain), "TerrainFrom")]
    static class PRI_TerrainFrom
    {
        static bool Prefix(IntVec3 c, Map map, RiverMaker river, float fertility, ref TerrainDef __result)
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
            if (fertility < -994f)
            {
                if (river != null)
                {
                    if (river?.TerrainAt(c, true) == null)
                    {
                        __result = TerrainDefOf.Sand;
                        return false;
                    }
                }
                else
                {
                    __result = TerrainDefOf.Sand;
                    return false;
                }
            }
            return true;
            
        }
    }

}
