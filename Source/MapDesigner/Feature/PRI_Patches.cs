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
        static bool Prefix(IntVec3 c, Map map, RiverMaker river, ref TerrainDef __result)
        {
            if(MapDesignerMod.mod.settings.selectedFeature != MapDesignerSettings.Features.RoundIsland)
            {
                return true;
            }

            MapGenFloatGrid priGrid = MapGenerator.FloatGridNamed("ZMD_PRI");
                
            if(priGrid[c] < 0.1f)
            {
                __result = TerrainDefOf.WaterOceanShallow;
                return false;
            }
            if (priGrid[c] < 1.1f)
            {
                if ((river?.TerrainAt(c, true).IsRiver) != true)
                {
                    __result = TerrainDefOf.Sand;
                    return false;
                }
            }
            return true;
        }
    }

    


}
