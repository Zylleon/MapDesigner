using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

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

        }

        public static void DrawNoOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featureNoneInfo".Translate());

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
            settings.priIslandSize = listing.Slider(settings.priIslandSize, 5f, 45f);

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

            settings.lakeSize = InterfaceUtility.LabeledSlider(listing, settings.lakeSize, 0.04f, 1.0f, String.Format("ZMD_lakeSize".Translate(), Math.Round(100 * settings.lakeSize)));

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
            return "ZMD_selectFeature".Translate();
        }

        private static string lakeRoundnessLabel
        {
            get
            {
                int label = 0;
                //if (settings.lakeRoundness > 0.3f)
                //{
                //    label++;
                //}
                //if (settings.lakeRoundness > 0.75f)
                //{
                //    label++;
                //}
                //if (settings.lakeRoundness > 2f)
                //{
                //    label++;
                //}
                //if (settings.lakeRoundness > 2.75f)
                //{
                //    label++;
                //}

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

        }
    }
}
