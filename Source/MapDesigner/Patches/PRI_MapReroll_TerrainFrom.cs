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
            if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.None)
            {
                return true;
            }

            else if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.RoundIsland)
            {
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

            else if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.Lake)
            {
                if (fertility < -998f)         // deep water
                {
                    if (MapDesignerMod.mod.settings.flagLakeSalty)
                    {
                        __result = TerrainDefOf.WaterOceanDeep;
                    }
                    else
                    {
                        __result = TerrainDefOf.WaterDeep;
                    }
                    return false;
                }
                if (fertility < -985f)         // shallow water
                {
                    if (MapDesignerMod.mod.settings.flagLakeSalty)
                    {
                        __result = TerrainDefOf.WaterOceanShallow;
                    }
                    else
                    {
                        __result = TerrainDefOf.WaterShallow;
                    }
                    return false;
                }
                else if (fertility < -975f)    // beach
                {
                    __result = TerrainDefOf.Sand;
                    __result = TerrainDef.Named(MapDesignerMod.mod.settings.lakeShore);
                    return false;
                }
                return true;
            }

            return true;
        }
    }
}
