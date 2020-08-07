using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;


namespace MapDesigner.Patches
{
    [HarmonyPatch(typeof(RimWorld.RiverMaker))]
    [HarmonyPatch(nameof(RimWorld.RiverMaker.TerrainAt))]
    static class RiverBankPatch
    {
        static bool Prefix(ref TerrainDef __result, ModuleBase ___generator, ModuleBase ___coordinateZ, ModuleBase ___shallowizer, float ___surfaceLevel, List<IntVec3> ___lhs, List<IntVec3> ___rhs, IntVec3 loc, bool recordForValidation = false)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();
            bool output = true;
            bool doRiverSplit = false;
            switch (settings.selRiverStyle)
            {
                case MapDesignerSettings.RiverStyle.Spring:
                    // TODO: Round out back end, ensure banks get placed correctly
                    if(___coordinateZ.GetValue(loc) < 0)
                    {
                        output = false;
                    }
                    break;

                case MapDesignerSettings.RiverStyle.Confluence:
                    if (___coordinateZ.GetValue(loc) < 0)
                    {
                        output = false;
                        doRiverSplit = true;
                        
                    }

                    break;
                case MapDesignerSettings.RiverStyle.Fork:
                    if (___coordinateZ.GetValue(loc) > 0)
                    {
                        output = false;
                        doRiverSplit = true;
                    }
                    break;
                default:
                    break;
            }

            if (doRiverSplit)
            {
                float value = Mathf.Abs(0.5f * Mathf.Abs(___coordinateZ.GetValue(loc)) - Mathf.Abs(___generator.GetValue(loc)));

                float num = ___surfaceLevel - value;

                if (num > 2f && ___shallowizer.GetValue(loc) > 0.2f)
                {
                    __result = TerrainDefOf.WaterMovingChestDeep;
                }
                else if (num > 0f)
                {
                    if (recordForValidation)
                    {
                        if (value < 0f)
                        {
                            ___lhs.Add(loc);
                        }
                        else
                        {
                            ___rhs.Add(loc);
                        }
                    }
                    __result = TerrainDefOf.WaterMovingShallow;
                }
                else if (MapDesignerSettings.flagRiverBeach && num > 0 - settings.riverBeachSize)
                {
                    __result = TerrainDef.Named(settings.riverShore);
                }
                else
                {
                    __result = null;
                }
            }


            return output;
        }

        //static void Postfix(ref TerrainDef __result, ModuleBase ___generator, float ___surfaceLevel, IntVec3 loc, bool recordForValidation = false)
        //{
        //    if (!MapDesignerSettings.flagRiverBeach)
        //    {
        //        return;
        //    }

        //    float value = ___generator.GetValue(loc);
        //    float num = ___surfaceLevel - Mathf.Abs(value);
        //    MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        //    if (num < 0 && num > 0 - settings.riverBeachSize)
        //    {
        //        __result = TerrainDef.Named(settings.riverShore);
        //    }
        //}
    }


}
