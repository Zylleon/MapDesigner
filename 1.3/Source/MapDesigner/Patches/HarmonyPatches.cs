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
using MapDesigner.Patches;
using MapDesigner.Feature;


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
                Log.Message("[Map Designer for 1.3] Initializing.... ");
                harmony.PatchAll();

                if (ModsConfig.RoyaltyActive)
                {
                    try
                    {
                        MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_AnimaTrees), "DesiredTreeCountForMap");
                        //HarmonyMethod postfix = new HarmonyMethod(typeof(MapDesigner).GetMethod("AnimaTreePatch"));
                        HarmonyMethod postfix = new HarmonyMethod(typeof(AnimaTreePatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static));
                        harmony.Patch(targetmethod, null, postfix);
                    }
                    catch
                    {
                        Log.Message("[Map Designer] ERROR: Failed to patch anima trees");
                    }
                }

                if (GenTypes.GetTypeInAnyAssembly("MapReroll.MapPreviewGenerator") != null)
                {
                    Log.Message("Found Map Reroll");
                    HelperMethods.ApplyMapRerollPatches();
                   
                }

                HelperMethods.InitBiomeDefaults();
                HelperMethods.ApplyBiomeSettings();
            }
        }
    }
}
