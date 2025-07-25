﻿using System;
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
    /// <summary>
    ///  Adds river banks
    /// </summary>
    /// 
    /*
    [HarmonyPatch(typeof(RimWorld.RiverMaker))]
    [HarmonyPatch(nameof(RimWorld.RiverMaker.TerrainAt))]
    static class RiverBankPatch
    {
        static bool Prefix(ref TerrainDef __result, ModuleBase ___generator, ModuleBase ___coordinateX, ModuleBase ___coordinateZ, ModuleBase ___shallowizer, float ___surfaceLevel, float ___shallowFactor, List<IntVec3> ___lhs, List<IntVec3> ___rhs, IntVec3 loc, bool recordForValidation = false)
        {
            float value = ___generator.GetValue(loc);
            float num = ___surfaceLevel - Mathf.Abs(value);
            //float num = ___surfaceLevel - value;
            if (num > 2f && ___shallowizer.GetValue(loc) > ___shallowFactor)
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
            else if(MapDesignerMod.mod.settings.flagRiverBeach)
            {
                MapDesignerSettings settings = MapDesignerMod.mod.settings;
                if (num > 0 - settings.riverBeachSize)
                {
                    __result = TerrainDef.Named(settings.riverShore);
                }
            }

            return false;
        }
    }

    */

    /// <summary>
    /// Makes river banks take priority over coastal beaches
    /// </summary>
    /// 
    /*
    [HarmonyPatch(typeof(RimWorld.GenStep_Terrain), "TerrainFrom")]
    static class RiverbankBeachFix
    {
        static void Postfix(IntVec3 c, Map map, RiverMaker river, ref TerrainDef __result)
        {
            if (!MapDesignerMod.mod.settings.flagRiverBeach)
            {
                return;
            }
            if (Find.World.CoastDirectionAt(map.Tile) == Rot4.Invalid)
            {
                return;
            }
            if (river == null)
            {
                return;
            }

            if (river.TerrainAt(c, true) != null && BeachMaker.BeachTerrainAt(c, map.Biome)?.IsWater != true)
            {
                __result = river.TerrainAt(c, true);
            }
        }
    }
    */

}
