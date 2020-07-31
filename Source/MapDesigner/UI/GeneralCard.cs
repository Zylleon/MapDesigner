using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;



namespace MapDesigner.UI
{
    public class GeneralCard
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawGeneralCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.Label("ZMD_generalTabInfo".Translate());

            listing.GapLine();

            // randomize
            if (listing.ButtonText("ZMD_randomize".Translate()))
            {
                RandomizeSettings();
            }

            // reset
            listing.GapLine();
            if (listing.ButtonText("ZMD_reset".Translate()))
            {
                ResetAllSettings();
            }

            listing.End();
        }


        public static void ResetAllSettings()
        {
            MountainCard.ResetMountainSettings();
            TerrainCard.ResetTerrainSettings();
            ThingsCard.ResetThingsSettings();
            RiversCard.ResetRiversSettings();
            settings.selectedFeature = MapDesignerSettings.Features.None;
        }


        public static void RandomizeSettings()
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

    }
}
