using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using static MapDesigner.MapDesignerSettings;
using Verse.Noise;
using RimWorld.Planet;

namespace MapDesigner.Patches
{
    /// <summary>
    /// Coast direction
    /// </summary>
    

    [HarmonyPatch(typeof(RimWorld.Planet.World), "CoastAngleAt")]
    static class CoastDirPatch
    {
        static void Postfix(PlanetTile tile, ref float? __result)
        {
            if(MapDesignerMod.mod.settings.coastDir == CoastDirection.Vanilla)
            {
                return;
            }
            if(__result == null)
            {
                return;
            }
            switch(MapDesignerMod.mod.settings.coastDir)
            {
                case CoastDirection.North:
                    __result = 270;
                    break;
                case CoastDirection.East:
                    __result = 180;
                    break;
                case CoastDirection.South:
                    __result = 90;
                    break;
                case CoastDirection.West:
                    __result = 0;
                    break;
            }
        }
    }
    

    /// <summary>
    /// Coast terrain
    /// </summary>
    /// 
    /* TODO: RESTORE BEACH TERRAIN
     * 
     * 
    [HarmonyPatch(typeof(BeachMaker), "BeachTerrainAt")]
    static class CoastTerrainPatch
    {
        static bool Prefix(IntVec3 loc, BiomeDef biome, ModuleBase ___beachNoise, ref TerrainDef __result)
        {
            if (___beachNoise == null)
            {
                return true;
            }
            if(MapDesignerMod.mod.settings.beachTerr == "Vanilla")
            {
                return true;
            }

            float value = ___beachNoise.GetValue(loc);
            if(value <1f && value >= 0.45f)
            {
                __result = TerrainDef.Named(MapDesignerMod.mod.settings.beachTerr);
                return false;
            }

            return true;

        }
    }

    */


}
