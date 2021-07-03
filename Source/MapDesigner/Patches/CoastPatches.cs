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

namespace MapDesigner.Patches
{
    /// <summary>
    /// Coast direction
    /// </summary>
    [HarmonyPatch(typeof(RimWorld.Planet.World), "CoastDirectionAt")]
    static class CoastDirPatch
    {
        static void Postfix(int tileID, ref Rot4 __result)
        {
            if(MapDesignerMod.mod.settings.coastDir == CoastDirection.Vanilla)
            {
                return;
            }
            if(__result == Rot4.Invalid)
            {
                return;
            }
            switch(MapDesignerMod.mod.settings.coastDir)
            {
                case CoastDirection.North:
                    __result = Rot4.North;
                    break;
                case CoastDirection.East:
                    __result = Rot4.East;
                    break;
                case CoastDirection.South:
                    __result = Rot4.South;
                    break;
                case CoastDirection.West:
                    __result = Rot4.West;
                    break;
            }
        }
    }


    /// <summary>
    /// Coast terrain
    /// </summary>
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


}
