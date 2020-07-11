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
using System.Reflection;


namespace MapDesigner.Patches
{
    public static class HarmonyPatches
    {
        [StaticConstructorOnStartup]
        public static class MapDesigner
        {
            static MapDesigner()
            {
                Harmony harmony = new Harmony("zylle.MapDesigner");
                Log.Message("Initializing Map Designer");
                harmony.PatchAll();

                MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_Terrain), "TerrainFrom");
                HarmonyMethod postfix = new HarmonyMethod(typeof(MapDesigner).GetMethod("RiverBeachPostfix"));
                harmony.Patch(targetmethod, null, postfix);

                HelperMethods.InitBiomeDefaults();
                HelperMethods.ApplyBiomeSettings();

            }


            /// <summary>
            /// River banks take priority over coastal beaches
            /// </summary>
            /// <param name="c"></param>
            /// <param name="map"></param>
            /// <param name="elevation"></param>
            /// <param name="fertility"></param>
            /// <param name="river"></param>
            /// <param name="__result"></param>
            public static void RiverBeachPostfix(IntVec3 c, Map map, RiverMaker river, ref TerrainDef __result)
            {
                if (!MapDesignerSettings.flagRiverBeach)
                {
                    return;
                }
                if(Find.World.CoastDirectionAt(map.Tile) == Rot4.Invalid)
                {
                    return;
                }
                if(river == null)
                {
                    return;
                }

                if (river.TerrainAt(c, true) != null && BeachMaker.BeachTerrainAt(c, map.Biome)?.IsWater != true)
                {
                    __result = river.TerrainAt(c, true);
                }
            }
        }
    }
}
