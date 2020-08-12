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

                //MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_Terrain), "TerrainFrom");
                //HarmonyMethod postfix = new HarmonyMethod(typeof(MapDesigner).GetMethod("RiverBeachPostfix"));
                //harmony.Patch(targetmethod, null, postfix);

                HelperMethods.InitBiomeDefaults();
                HelperMethods.ApplyBiomeSettings();
            }
        }
    }
}
