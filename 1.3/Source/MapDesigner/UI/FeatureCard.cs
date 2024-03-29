﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using static MapDesigner.MapDesignerSettings;

namespace MapDesigner.UI
{
    public static class FeatureCard
    {
        public static MapDesignerSettings settings = MapDesignerMod.mod.settings;


        public static void DrawFeaturesCard(Rect rect)
        {

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);

            listingStandard.Label("ZMD_featureTabInfo".Translate());

            Rect selectRect = listingStandard.GetRect(30f).LeftHalf();

            Listing_Standard featureSelectListing = new Listing_Standard();
            featureSelectListing.Begin(selectRect);

            if (featureSelectListing.ButtonTextLabeled("ZMD_selectFeature".Translate(), GetFeatureLabel(settings.selectedFeature)))
            {
                List<FloatMenuOption> featureList = new List<FloatMenuOption>();

                featureList.Add(new FloatMenuOption("ZMD_featureNone".Translate(), delegate
                {
                    settings.selectedFeature = MapDesignerSettings.Features.None;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));

                featureList.Add(new FloatMenuOption("ZMD_featurePRI".Translate(), delegate
                {
                    settings.selectedFeature = MapDesignerSettings.Features.RoundIsland;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                featureList.Add(new FloatMenuOption("ZMD_featureLake".Translate(), delegate
                {
                    settings.selectedFeature = MapDesignerSettings.Features.Lake;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                featureList.Add(new FloatMenuOption("ZMD_featureNI".Translate(), delegate
                {
                    settings.selectedFeature = MapDesignerSettings.Features.NatIsland;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));

                Find.WindowStack.Add(new FloatMenu(featureList));
            }
            featureSelectListing.End();

            listingStandard.GapLine();

            DrawFeatureOptions(listingStandard);

            listingStandard.End();

        }


        public static void DrawFeatureOptions(Listing_Standard listing)
        {
            //this.settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

            MapDesignerSettings.Features selectedFeature = settings.selectedFeature;

            if (settings.selectedFeature == MapDesignerSettings.Features.None)
            {
                DrawNoOptions(listing);
            }
            else if (settings.selectedFeature == MapDesignerSettings.Features.RoundIsland)
            {
                DrawRoundIslandOptions(listing);
            }
            else if (settings.selectedFeature == MapDesignerSettings.Features.Lake)
            {
                DrawLakeOptions(listing);
            }
            else if (settings.selectedFeature == MapDesignerSettings.Features.NatIsland)
            {
                DrawNIOptions(listing);
            }

        }

        public static void DrawNoOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featureNoneInfo".Translate());
            
            listing.Gap();

            if (listing.ButtonText("ZMD_resetFeatures".Translate()))
            {
                ResetFeatureSettings();
            }
        }

        public static void DrawRoundIslandOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featurePRIInfo".Translate());

            if (listing.ButtonTextLabeled("ZMD_priIslandStyle".Translate(), priStyleLabel))
            {
                List<FloatMenuOption> featureList = new List<FloatMenuOption>();

                featureList.Add(new FloatMenuOption("ZMD_priSingleLabel".Translate(), delegate
                {
                    settings.priStyle = MapDesignerSettings.PriStyle.Single;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));

                featureList.Add(new FloatMenuOption("ZMD_priMultiLabel".Translate(), delegate
                {
                    settings.priStyle = MapDesignerSettings.PriStyle.Multi;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));

                Find.WindowStack.Add(new FloatMenu(featureList));
            }

            listing.CheckboxLabeled("ZMD_flagLakeSalty".Translate(), ref settings.flagPriSalty, "ZMD_flagLakeSalty".Translate());



            listing.Label("ZMD_priIslandSizeLabel".Translate());
            settings.priIslandSize = listing.Slider(settings.priIslandSize, 5f, 75f);

            if (settings.priStyle == MapDesignerSettings.PriStyle.Single)
            {
                //InterfaceUtility.LocationPicker(listing, 0.0f, ref settings.priSingleCenterDisp, 40f);

                //InterfaceUtility.LocationPicker(listing, 0.0f, ref settings.priSingleCenterDisp, settings.priIslandSize

                InterfaceUtility.LocationPicker(listing, 0.5f, ref settings.priSingleCenterDisp, 2 * settings.priIslandSize);

            }

            listing.Label("ZMD_priBeachSizeLabel".Translate());
            settings.priBeachSize = listing.Slider(settings.priBeachSize, 1f, 18f);

            //listing.CheckboxLabeled("ZMD_priMultiSpawnLabel".Translate(), ref settings.priMultiSpawn);

          

        }


        public static void DrawLakeOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featureLakeInfo".Translate());

            settings.lakeSize = InterfaceUtility.LabeledSlider(listing, settings.lakeSize, 0.04f, 1.5f, String.Format("ZMD_lakeSize".Translate(), Math.Round(100 * settings.lakeSize)));

            InterfaceUtility.LocationPicker(listing, 0.45f, ref settings.lakeCenterDisp, 100 * settings.lakeSize);

            settings.lakeRoundness = InterfaceUtility.LabeledSlider(listing, settings.lakeRoundness, 0f, 6f, lakeRoundnessLabel, "ZMD_lakeRoundness0".Translate(), "ZMD_lakeRoundness4".Translate());

            settings.lakeBeachSize = InterfaceUtility.LabeledSlider(listing, settings.lakeBeachSize, 0f, 35f, "ZMD_lakeBeachSize".Translate(), "ZMD_size0".Translate(), "ZMD_size6".Translate());

            settings.lakeDepth = InterfaceUtility.LabeledSlider(listing, settings.lakeDepth, 0f, 1f, lakeDepthLabel, "ZMD_lakeDepth0".Translate(), "ZMD_lakeDepth4".Translate());

            listing.CheckboxLabeled("ZMD_flagLakeSalty".Translate(), ref settings.flagLakeSalty, "ZMD_flagLakeSalty".Translate());

            List<TerrainDef> shoreOptions = new List<TerrainDef>();

            shoreOptions.Add(TerrainDefOf.Soil);
            shoreOptions.Add(TerrainDef.Named("SoilRich"));
            shoreOptions.Add(TerrainDefOf.Sand);
            shoreOptions.Add(TerrainDef.Named("MarshyTerrain"));
            shoreOptions.Add(TerrainDef.Named("Mud"));
            shoreOptions.Add(TerrainDefOf.Ice);

            if (listing.ButtonTextLabeled("ZMD_lakeShore".Translate(), TerrainDef.Named(settings.lakeShore).label))
            {
                List<FloatMenuOption> shoreTerrList = new List<FloatMenuOption>();

                foreach (TerrainDef terr in shoreOptions)
                {
                    shoreTerrList.Add(new FloatMenuOption(terr.label, delegate { settings.lakeShore = terr.defName; }, MenuOptionPriority.Default));
                }

                Find.WindowStack.Add(new FloatMenu(shoreTerrList));
            }



        }

        public static void DrawNIOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featureNiInfo".Translate());
            Rect niStyleRect = listing.GetRect(30f).LeftHalf();

            Listing_Standard niStyleSelectListing = new Listing_Standard();
            niStyleSelectListing.Begin(niStyleRect);
            if (niStyleSelectListing.ButtonTextLabeled("ZMD_niStyle".Translate(), GetNiStyleLabel(settings.niStyle)))
            {
                List<FloatMenuOption> styleList = new List<FloatMenuOption>();

                styleList.Add(new FloatMenuOption("ZMD_niStyleRound".Translate(), delegate
                {
                    settings.niStyle = MapDesignerSettings.NiStyle.Round;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));

                styleList.Add(new FloatMenuOption("ZMD_niStyleSquare".Translate(), delegate
                {
                    settings.niStyle = MapDesignerSettings.NiStyle.Square;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));

                styleList.Add(new FloatMenuOption("ZMD_niStyleRing".Translate(), delegate
                {
                    settings.niStyle = MapDesignerSettings.NiStyle.Ring;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));

                styleList.Add(new FloatMenuOption("ZMD_niStyleSquareRing".Translate(), delegate
                {
                    settings.niStyle = MapDesignerSettings.NiStyle.SquareRing;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                Find.WindowStack.Add(new FloatMenu(styleList));
            }
            niStyleSelectListing.End();

            settings.niSize = InterfaceUtility.LabeledSlider(listing, settings.niSize, 0.04f, 1.5f, String.Format("ZMD_priIslandSizeLabel".Translate(), Math.Round(100 * settings.niSize)));

            InterfaceUtility.LocationPicker(listing, 0.45f, ref settings.niCenterDisp, 100 * settings.niSize, niStyleTexture);

            settings.niRoundness = InterfaceUtility.LabeledSlider(listing, settings.niRoundness, 0f, 6f, niRoundnessLabel, "ZMD_niRoundness0".Translate(), "ZMD_niRoundness4".Translate());

            settings.niBeachSize = InterfaceUtility.LabeledSlider(listing, settings.niBeachSize, 0f, 35f, "ZMD_priBeachSizeLabel".Translate(), "ZMD_size0".Translate(), "ZMD_size6".Translate());

            List<TerrainDef> shoreOptions = new List<TerrainDef>();

            shoreOptions.Add(TerrainDefOf.Soil);
            shoreOptions.Add(TerrainDef.Named("SoilRich"));
            shoreOptions.Add(TerrainDefOf.Sand);
            shoreOptions.Add(TerrainDef.Named("MarshyTerrain"));
            shoreOptions.Add(TerrainDef.Named("Mud"));
            shoreOptions.Add(TerrainDefOf.Ice);

            Rect niShoreRect = listing.GetRect(30f).LeftHalf();

            Listing_Standard niShoreSelectListing = new Listing_Standard();
            niShoreSelectListing.Begin(niShoreRect);
            if (niShoreSelectListing.ButtonTextLabeled("ZMD_lakeShore".Translate(), TerrainDef.Named(settings.niShore).label))
            {
                List<FloatMenuOption> shoreTerrList = new List<FloatMenuOption>();

                foreach (TerrainDef terr in shoreOptions)
                {
                    shoreTerrList.Add(new FloatMenuOption(terr.label, delegate { settings.niShore = terr.defName; }, MenuOptionPriority.Default));
                }

                Find.WindowStack.Add(new FloatMenu(shoreTerrList));
            }
            niShoreSelectListing.End();

            settings.niWaterDepth = InterfaceUtility.LabeledSlider(listing, settings.niWaterDepth, -0.2f, 1.3f, niWaterDepthLabel, "ZMD_lakeDepth0".Translate(), "ZMD_lakeDepth4".Translate());
            if (settings.niWaterDepth >= 0.5f)
            {
                GUI.color = new Color(255, 180, 0);
                listing.Label("ZMD_niWaterDepthWarning".Translate());

                GUI.color = Color.white;
            }

            listing.CheckboxLabeled("ZMD_flagLakeSalty".Translate(), ref settings.flagNiSalty, "ZMD_flagLakeSalty".Translate());

           

        }


        #region labels

        public static string GetFeatureLabel(MapDesignerSettings.Features feature)
        {
            if (feature == MapDesignerSettings.Features.None)
            {
                return "ZMD_featureNone".Translate();
            }
            if (feature == MapDesignerSettings.Features.RoundIsland)
            {
                return "ZMD_featurePRI".Translate();
            }
            if (feature == MapDesignerSettings.Features.Lake)
            {
                return "ZMD_featureLake".Translate();
            }
            if(feature == MapDesignerSettings.Features.NatIsland)
            {
                return "ZMD_featureNI".Translate();
            }
            return "ZMD_selectFeature".Translate();
        }

        public static string GetNiStyleLabel(MapDesignerSettings.NiStyle style)
        {
            if (style == MapDesignerSettings.NiStyle.Round)
            {
                return "ZMD_niStyleRound".Translate();
            }
            if (style == MapDesignerSettings.NiStyle.Square)
            {
                return "ZMD_niStyleSquare".Translate();
            }
            if (style == MapDesignerSettings.NiStyle.SquareRing)
            {
                return "ZMD_niStyleSquareRing".Translate();
            }
            if (style == MapDesignerSettings.NiStyle.Ring)
            {
                return "ZMD_niStyleRing".Translate();
            }
            return "ZMD_niSelStyle".Translate();
        }

        private static string lakeRoundnessLabel
        {
            get
            {
                int label = 0;
                if (settings.lakeRoundness > 0.75f)
                {
                    label++;
                }
                if (settings.lakeRoundness > 1.75f)
                {
                    label++;
                }
                if (settings.lakeRoundness > 3f)
                {
                    label++;
                }
                if (settings.lakeRoundness > 4.25f)
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_lakeRoundness", "ZMD_lakeRoundness" + label);
            }
        }

        private static string niRoundnessLabel
        {
            get
            {
                int label = 0;
                if (settings.niRoundness > 0.75f)
                {
                    label++;
                }
                if (settings.niRoundness > 1.75f)
                {
                    label++;
                }
                if (settings.niRoundness > 3f)
                {
                    label++;
                }
                if (settings.niRoundness > 4.25f)
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_niRoundness", "ZMD_niRoundness" + label);
            }
        }

        private static string lakeDepthLabel
        {
            get
            {
                int label = 0;
                if (settings.lakeDepth > 0.2f)
                {
                    label++;
                }
                if (settings.lakeDepth > 0.4f)
                {
                    label++;
                }
                if (settings.lakeDepth > 0.6f)
                {
                    label++;
                }
                if (settings.lakeDepth > 0.8f)
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_lakeDepth", "ZMD_lakeDepth" + label);
            }
        }

        private static string niWaterDepthLabel
        {
            get
            {
                int label = 0;
                if (settings.niWaterDepth > 0.1f)
                {
                    label++;
                }
                if (settings.niWaterDepth > 0.3f)
                {
                    label++;
                }
                if (settings.niWaterDepth > 0.50f)
                {
                    label++;
                }
                if (settings.niWaterDepth > 0.85f)
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_niWaterDepth", "ZMD_lakeDepth" + label);
            }
        }

        private static string niStyleTexture
        {
            get
            {
                string output = "GUI/ZMD_circle";
                switch (settings.niStyle)
                {
                    case MapDesignerSettings.NiStyle.Round:
                        output = "GUI/ZMD_circle";
                        break;
                    case MapDesignerSettings.NiStyle.Square:
                        output = "GUI/ZMD_square";
                        break;
                    case MapDesignerSettings.NiStyle.Ring:
                        output = "GUI/ZMD_ring";
                        break;
                    case MapDesignerSettings.NiStyle.SquareRing:
                        output = "GUI/ZMD_squareRing";
                        break;

                }
                return output;
            }
        }
        private static string priStyleLabel
        {
            get
            {
                string output = "";
                switch (settings.priStyle)
                {
                    case MapDesignerSettings.PriStyle.Single:
                        output = "ZMD_priSingleDesc".Translate();
                        break;

                    case MapDesignerSettings.PriStyle.Multi:
                        output = "ZMD_priMultiDesc".Translate();
                        break;
                }

                return output;
            }

            
        }

    #endregion



        public static void ResetFeatureSettings()
        {
            settings.selectedFeature = MapDesignerSettings.Features.None;

            settings.priIslandSize = 40f;
            settings.priBeachSize = 5f;
            settings.priStyle = MapDesignerSettings.PriStyle.Single;
            settings.priSingleCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);
            settings.flagPriSalty = false;

            settings.lakeSize = 0.20f;
            settings.lakeBeachSize = 10f;
            settings.lakeRoundness = 1.5f;
            settings.lakeDepth = 0.5f;
            settings.flagLakeSalty = false;
            settings.lakeShore = "Sand";
            settings.lakeCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);

            settings.niSize = 0.75f;
            settings.niBeachSize = 20f;
            settings.niRoundness = 1.5f;
            settings.flagNiSalty = false;
            settings.niShore = "Sand";
            settings.niCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);
            settings.niStyle = NiStyle.Round;
            settings.niWaterDepth = 0.4f;

        }
    }
}
