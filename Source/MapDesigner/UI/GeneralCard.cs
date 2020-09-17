using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace MapDesigner.UI
{
    public class GeneralCard
    {
        public static MapDesignerSettings settings = MapDesigner_Mod.mod.settings;

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        private enum Preset : byte
        {
            Vanilla,
            FertileValley,
            Barrens,
            Unnatural,
            FishingVillage,
            Canyon,
            ZyllesChoice,
            Random
        }

        private static Preset selPreset;

        public static void DrawGeneralCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.Label("ZMD_generalTabInfo".Translate());

            listing.GapLine();

            listing.CheckboxLabeled("ZMD_flagHomeMapOnly".Translate(), ref MapDesignerSettings.flagHomeMapOnly, "ZMD_flagHomeMapOnlyDesc".Translate());

            listing.GapLine();

            #region presets
            Rect selPresetRect = listing.GetRect(40f);

            Rect selButtonRect = selPresetRect;
            Rect descRect = selPresetRect;
            selButtonRect.xMax -= 0.66f * rect.width;
            descRect.xMin += 20f + 0.34f * rect.width;

            Widgets.Label(descRect, (GetPresetLabel() + "Desc").Translate());
            Listing_Standard listing_selPreset = new Listing_Standard();
            listing_selPreset.Begin(selButtonRect);

            // preset selection
            if (listing_selPreset.ButtonTextLabeled("ZMD_presets".Translate(), GetPresetLabel().Translate()))
            {
                List<FloatMenuOption> presetList = new List<FloatMenuOption>();

                presetList.Add(new FloatMenuOption("ZMD_presetVanilla".Translate(), delegate
                {
                    selPreset = Preset.Vanilla;
                }));
                presetList.Add(new FloatMenuOption("ZMD_presetFertileValley".Translate(), delegate
                {
                    selPreset = Preset.FertileValley;
                }));
                presetList.Add(new FloatMenuOption("ZMD_presetBarrens".Translate(), delegate
                {
                    selPreset = Preset.Barrens;
                }));
                presetList.Add(new FloatMenuOption("ZMD_presetUnnatural".Translate(), delegate
                {
                    selPreset = Preset.Unnatural;
                }));
                presetList.Add(new FloatMenuOption("ZMD_presetFishingVillage".Translate(), delegate
                {
                    selPreset = Preset.FishingVillage;
                }));
                presetList.Add(new FloatMenuOption("ZMD_presetCanyon".Translate(), delegate
                {
                    selPreset = Preset.Canyon;
                }));
                presetList.Add(new FloatMenuOption("ZMD_presetZyllesChoice".Translate(), delegate
                {
                    selPreset = Preset.ZyllesChoice;
                }));
                presetList.Add(new FloatMenuOption("ZMD_presetRandom".Translate(), delegate
                {
                    selPreset = Preset.Random;
                }));
                Find.WindowStack.Add(new FloatMenu(presetList));
            }

            listing_selPreset.End();
            listing.GapLine();
            if (listing.ButtonText("ZMD_applyPreset".Translate()))
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                ApplyPreset();
            }


            #endregion

            listing.GapLine();
            if (listing.ButtonText("ZMD_reset".Translate()))
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                ResetAllSettings();
            }

            listing.End();
        }

        private static void ApplyPreset()
        {
            switch (selPreset)
            {
                case Preset.Vanilla:
                    ResetAllSettings();
                    break;
                case Preset.FertileValley:
                    PresetFertileValley();
                    break;
                case Preset.Barrens:
                    PresetBarrens();
                    break;
                case Preset.Unnatural:
                    PresetUnnatural();
                    break;
                case Preset.FishingVillage:
                    PresetFishingVillage();
                    break;
                case Preset.Canyon:
                    PresetCanyon();
                    break;
                case Preset.ZyllesChoice:
                    PresetZyllesChoice();
                    break;
                default:
                    break;
            }
        }

        private static string GetPresetLabel()
        {
            string label = "ZMD_presetVanilla";

            switch(selPreset)
            {
                case Preset.Vanilla:
                    label = "ZMD_presetVanilla";
                    break;
                case Preset.FertileValley:
                    label = "ZMD_presetFertileValley";
                    break;
                case Preset.Barrens:
                    label = "ZMD_presetBarrens";
                    break;
                case Preset.Unnatural:
                    label = "ZMD_presetUnnatural";
                    break;
                case Preset.FishingVillage:
                    label = "ZMD_presetFishingVillage";
                    break;
                case Preset.Canyon:
                    label = "ZMD_presetCanyon";
                    break;
                case Preset.ZyllesChoice:
                    label = "ZMD_presetZyllesChoice";
                    break;
                case Preset.Random:
                    label = "ZMD_presetRandom";
                    break;
            }

            return label;
        }

        public static void ResetAllSettings()
        {
            MountainCard.ResetMountainSettings();
            TerrainCard.ResetTerrainSettings();
            ThingsCard.ResetThingsSettings();
            RiversCard.ResetRiversSettings();
            settings.selectedFeature = MapDesignerSettings.Features.None;
        }

        public static void PresetRandom()
        {
            // Mountains
            settings.hillAmount = Rand.Range(0f, 2.5f);
            settings.hillSize = Rand.Range(0.010f, 0.1f);
            settings.hillSmoothness = Rand.Range(0f, 5f);
            MapDesignerSettings.flagCaves = true;

            // Mountain shape
            //MapDesignerSettings.flagHillClumping = false;
            MapDesignerSettings.flagHillRadial = false;
            MapDesignerSettings.flagHillSplit = false;
            MapDesignerSettings.flagHillSide = false;

            int mountainShape = Rand.Range(0, 3);
            switch (mountainShape)
            {
                case 0:
                    MapDesignerSettings.flagHillRadial = true;
                    settings.hillRadialAmt = Rand.Range(-3f, 3f);
                    settings.hillRadialSize = Rand.Range(0.2f, 1.1f);
                    break;
                case 1:
                    MapDesignerSettings.flagHillSplit = true;
                    settings.hillSplitAmt = Rand.Range(-3f, 3f);
                    settings.hillSplitDir = 10f * (float)Math.Round(Rand.Range(0f, 17f));
                    settings.hillSplitSize = Rand.Range(0.05f, 1.1f);
                    break;
                case 2:
                    MapDesignerSettings.flagHillSide = true;
                    settings.hillSideAmt = Rand.Range(0.2f, 3f);
                    settings.hillSideDir = 10f * (float)Math.Round(Rand.Range(0f, 35f));
                    break;
                default:
                    break;
            }

            // Things
            settings.densityPlant = Rand.Range(0f, 2.5f);
            settings.densityAnimal = Rand.Range(0f, 2.5f);
            settings.densityRuins = Rand.Range(0f, 2.5f);
            settings.densityDanger = Rand.Range(0f, 2.5f);

            settings.densityGeyser = Rand.Range(0f, 2.5f);
            settings.densityOre = Rand.Range(0f, 2.5f);
            settings.animaCount = (float)Rand.Range(1, 15);

            // Rocks
            settings.rockTypeRange = new IntRange(1, 5);
            MapDesignerSettings.flagBiomeRocks = true;

            // Rivers
            //Log.Message("Randomizing rivers");
            //settings.sizeRiver = Rand.Range(0.1f, 3f);
            //MapDesignerSettings.flagRiverBeach = Rand.Bool;
            //settings.riverShore = RiversCardUtility.terrainOptions.RandomElement().defName;
            //settings.riverBeachSize = Rand.Range(2f, 35f);

            // Terrain
            settings.terrainFert = Rand.Range(0.3f, 2f);
            settings.terrainWater = Rand.Range(0.3f, 2f);
            MapDesignerSettings.flagTerrainWater = Rand.Bool;

            // Features
            //int mapFeature = Rand.Range(0, 2);
            //switch (mapFeature)
            //{
            //    case 1:
            //        // Perfectly Round Islands
            //        settings.selectedFeature = MapDesignerSettings.Features.RoundIsland;
            //        settings.priIslandSize = Rand.Range(5f, 45f);
            //        settings.priBeachSize = Rand.Range(1f, 18f);
            //        MapDesignerSettings.priMultiSpawn = Rand.Bool;
            //        break;
            //    case 2:
            //        // Lake
            //        settings.selectedFeature = MapDesignerSettings.Features.Lake;
            //        settings.lakeSize = Rand.Range(0.04f, 1.0f);
            //        settings.lakeRoundness = Rand.Range(0f, 3.5f);
            //        settings.lakeBeachSize = Rand.Range(0f, 35f);
            //        settings.lakeDepth = Rand.Range(0f, 1f);
            //        MapDesignerSettings.flagLakeSalty = Rand.Bool;
            //        //settings.lakeShore = RiversCardUtility.terrainOptions.RandomElement().defName;
            //        break;
            //    default:
            //        settings.selectedFeature = MapDesignerSettings.Features.None;
            //        break;
            //}
        }

        public static void PresetFertileValley()
        {
            ResetAllSettings();

            settings.hillAmount = 1.2f;
            MapDesignerSettings.flagCaves = false;
            MapDesignerSettings.flagMtnExit = true;
            MapDesignerSettings.flagHillRadial = true;
            settings.densityRuins = 0.5f;
            settings.densityPlant = 1.3f;
            settings.terrainFert = 1.5f;
            MapDesignerSettings.flagRiverBeach = true;
        }


        public static void PresetCanyon()
        {
            ResetAllSettings();

            settings.hillAmount = 1.1f;
            MapDesignerSettings.flagCaves = false;
            MapDesignerSettings.flagHillSplit = true;
            settings.hillSplitAmt = 2.2f;

            float angle = 10 * Rand.Range(0, 17);
            settings.hillSplitDir = angle;
            Log.Message("Angle: " + angle);
            MapDesignerSettings.flagRiverDir = true;
            if(Rand.Bool)
            {
                angle += 180f;
            }
            settings.riverDir = angle;

            settings.rockTypeRange = new IntRange(4, 5);
        }

        public static void PresetBarrens()
        {
            ResetAllSettings();

            settings.hillSize = 0.05f;
            settings.hillSmoothness = 3.0f;
            settings.densityGeyser = 1.3f;
            settings.densityPlant = 0.5f;
            settings.terrainFert = 0.7f;
            settings.terrainWater = 0.7f;
        }

        public static void PresetUnnatural()
        {
            ResetAllSettings();

            settings.hillAmount = 1.1f;
            settings.hillSmoothness = 0f;
            settings.rockTypeRange = new IntRange(1, 1);
            settings.densityDanger = 1.5f;
            settings.animaCount = Rand.Range(3, 8);

        }

        public static void PresetFishingVillage()
        {
            ResetAllSettings();

            settings.hillSize = 0.01f;
            settings.terrainWater = 2f;
            MapDesignerSettings.flagTerrainWater = true;
        }

        public static void PresetZyllesChoice()
        {
            ResetAllSettings();

            settings.hillAmount = 1.15f;
            settings.hillSize = 0.035f;
            settings.terrainFert = 1.4f;
            settings.densityOre = 1.2f;

        }
    }
}
