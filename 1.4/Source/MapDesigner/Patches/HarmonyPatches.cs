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
                Log.Message("[Map Designer for 1.4] Initializing.... ");
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


                if (ModsConfig.IdeologyActive)
                {
                    // RoadDebris
                    harmony.Patch(AccessTools.Method(typeof(RimWorld.GenStep_ScatterRoadDebris), "Generate"), new HarmonyMethod(typeof(IdeologyPatch).GetMethod("RoadDebris", BindingFlags.NonPublic | BindingFlags.Static)));

                    // CaveDebris
                    harmony.Patch(AccessTools.Method(typeof(RimWorld.GenStep_ScatterCaveDebris), "Generate"), new HarmonyMethod(typeof(IdeologyPatch).GetMethod("CaveDebris", BindingFlags.NonPublic | BindingFlags.Static)));

                    // AncientUtilityBuilding
                    harmony.Patch(AccessTools.Method(typeof(RimWorld.GenStep_ScatterAncientUtilityBuilding), "Generate"), new HarmonyMethod(typeof(IdeologyPatch).GetMethod("AncientUtilityBuilding", BindingFlags.NonPublic | BindingFlags.Static)));
                    
                    // AncientTurret
                    harmony.Patch(AccessTools.Method(typeof(RimWorld.GenStep_ScatterAncientTurret), "Generate"), new HarmonyMethod(typeof(IdeologyPatch).GetMethod("AncientTurret", BindingFlags.NonPublic | BindingFlags.Static)));

                    // AncientMechs
                    harmony.Patch(AccessTools.Method(typeof(RimWorld.GenStep_ScatterAncientMechs), "Generate"), new HarmonyMethod(typeof(IdeologyPatch).GetMethod("AncientMechs", BindingFlags.NonPublic | BindingFlags.Static)));

                    // AncientLandingPad
                    harmony.Patch(AccessTools.Method(typeof(RimWorld.GenStep_ScatterAncientLandingPad), "Generate"), new HarmonyMethod(typeof(IdeologyPatch).GetMethod("AncientLandingPad", BindingFlags.NonPublic | BindingFlags.Static)));

                    // AncientFences
                    harmony.Patch(AccessTools.Method(typeof(RimWorld.GenStep_ScatterAncientFences), "Generate"), new HarmonyMethod(typeof(IdeologyPatch).GetMethod("AncientFences", BindingFlags.NonPublic | BindingFlags.Static)));
                }


                if (ModsConfig.BiotechActive)
                {
                    try
                    {
                        MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.GenStep_PoluxTrees), "DesiredTreeCountForMap");
                        //HarmonyMethod postfix = new HarmonyMethod(typeof(MapDesigner).GetMethod("AnimaTreePatch"));
                        HarmonyMethod postfix = new HarmonyMethod(typeof(PoluxTreesPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static));
                        harmony.Patch(targetmethod, null, postfix);

                        targetmethod = AccessTools.Method(typeof(Verse.PollutionUtility), "PolluteMapToPercent");
                        //HarmonyMethod postfix = new HarmonyMethod(typeof(MapDesigner).GetMethod("AnimaTreePatch"));
                        HarmonyMethod prefix = new HarmonyMethod(typeof(PollutionLevelPatch).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static));
                        harmony.Patch(targetmethod, prefix);
                    }
                    catch
                    {
                        Log.Message("[Map Designer] ERROR: Failed to patch polux trees, pollution");
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
