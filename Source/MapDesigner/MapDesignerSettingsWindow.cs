using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace MapDesigner
{
    public class MapDesigner_Mod : Mod
    {
        private enum InfoCardTab : byte
        {
            Mountains,
            Things,
            Feature,
            Beta
        }
        private MapDesigner_Mod.InfoCardTab tab;

        MapDesignerSettings settings;

        public MapDesigner_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MapDesignerSettings>();
        }

        public override string SettingsCategory()
        {
            return "ZMD_ModName".Translate();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            HelperMethods.ApplyBiomeSettings();
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            Rect rect3 = new Rect(inRect);

            List<TabRecord> list = new List<TabRecord>();

            TabRecord mountainTab = new TabRecord("ZMD_mountainTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Mountains;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Mountains);
            list.Add(mountainTab);

            TabRecord ThingsTab = new TabRecord("ZMD_thingsTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Things;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Things);
            list.Add(ThingsTab);

            TabRecord featureTab = new TabRecord("ZMD_featureTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Feature;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Feature);
            list.Add(featureTab);

            //TabRecord betaTab = new TabRecord("ZMD_betaTab".Translate(), delegate
            //{
            //    this.tab = MapDesigner_Mod.InfoCardTab.Beta;
            //}, this.tab == MapDesigner_Mod.InfoCardTab.Beta);
            //list.Add(betaTab);

            TabDrawer.DrawTabs(rect3, list, 200f);
            this.FillCard(rect3.ContractedBy(18f));

            listingStandard.End();

            //base.DoSettingsWindowContents(inRect);
        }


        protected void FillCard(Rect cardRect)
        {
            if (this.tab == MapDesigner_Mod.InfoCardTab.Mountains)
            {
                DrawMountainCard(cardRect);
            }
            else if (this.tab == MapDesigner_Mod.InfoCardTab.Things)
            {
                DrawThingsCard(cardRect);
            }
            else if (this.tab == MapDesigner_Mod.InfoCardTab.Feature)
            {
                DrawFeaturesCard(cardRect);
            }
            else if (this.tab == MapDesigner_Mod.InfoCardTab.Beta)
            {
                // do beta tab
            }
        }


        #region draw cards

        private void DrawMountainCard(Rect rect)
        {

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);

            // mountains
            listingStandard.Label(hillAmountLabel);
            settings.hillAmount = listingStandard.Slider(settings.hillAmount, 0f, 2.5f);

            listingStandard.Label(hillSizeLabel);
            settings.hillSize = listingStandard.Slider(settings.hillSize, 0.01f, 0.10f);

            listingStandard.Label(hillSmoothnessLabel);
            settings.hillSmoothness = listingStandard.Slider(settings.hillSmoothness, 0f, 5f);

            // general mountain related
            listingStandard.CheckboxLabeled("ZMD_flagCaves".Translate(), ref MapDesignerSettings.flagCaves, "ZMD_flagCavesTooltip".Translate());

            listingStandard.CheckboxLabeled("ZMD_flagOneRock".Translate(), ref MapDesignerSettings.flagOneRock, "ZMD_flagOneRockTooltip".Translate());

            // hill distribution
            if (settings.hillSize > 0.022f)
            {
                listingStandard.CheckboxLabeled("ZMD_flagHillClumping".Translate(), ref MapDesignerSettings.flagHillClumping, "ZMD_flagHillClumpingTooltip".Translate());
            }

            
            listingStandard.CheckboxLabeled("ZMD_flagHillRadial".Translate(), ref MapDesignerSettings.flagHillRadial, "ZMD_flagHillRadialTooltip".Translate());
            if (MapDesignerSettings.flagHillRadial)
            {
                Rect hillRadialRect = listingStandard.GetRect(60f + Text.CalcHeight(hillRadialAmtLabel, listingStandard.ColumnWidth - 40f) * 2);
                hillRadialRect.xMin += 20f;
                hillRadialRect.xMax -= 20f;

                Listing_Standard hillRadialListing = new Listing_Standard();
                hillRadialListing.Begin(hillRadialRect);

                hillRadialListing.Label(hillRadialAmtLabel, -1, "ZMD_hillRadialAmtTooltip".Translate());
                settings.hillRadialAmt = hillRadialListing.Slider(settings.hillRadialAmt, -3.0f, 3.0f);

                hillRadialListing.Label(hillRadialSizeLabel, -1, "ZMD_hillRadialSizeTooltip".Translate());
                settings.hillRadialSize = hillRadialListing.Slider(settings.hillRadialSize, 0.2f, 1.1f);

                hillRadialListing.End();
            }

            // reset
            listingStandard.GapLine();
            if (listingStandard.ButtonText("ZMD_resetMountain".Translate()))
            {
                ResetMountainSettings();
            }

            listingStandard.End();
        }

        private void DrawThingsCard(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            // stuff density

            listingStandard.Label(FormatLabel("ZMD_densityPlant", "ZMD_density" + GetDensityLabel(settings.densityPlant)));
            settings.densityPlant = listingStandard.Slider(settings.densityPlant, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityAnimal", "ZMD_density" + GetDensityLabel(settings.densityAnimal)));
            settings.densityAnimal = listingStandard.Slider(settings.densityAnimal, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityRuins", "ZMD_density" + GetDensityLabel(settings.densityRuins)));
            settings.densityRuins = listingStandard.Slider(settings.densityRuins, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityDanger", "ZMD_density" + GetDensityLabel(settings.densityDanger)), -1, "ZMD_densityDangerTooltip".Translate());
            settings.densityDanger = listingStandard.Slider(settings.densityDanger, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityGeyser", "ZMD_density" + GetDensityLabel(settings.densityGeyser)));
            settings.densityGeyser = listingStandard.Slider(settings.densityGeyser, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityOre", "ZMD_density" + GetDensityLabel(settings.densityOre)));
            settings.densityOre = listingStandard.Slider(settings.densityOre, 0f, 2.5f);

            // Things
            listingStandard.GapLine();

            listingStandard.Label(FormatLabel("ZMD_sizeRiver", "ZMD_size" + GetDensityLabel(settings.sizeRiver)));
            settings.sizeRiver = listingStandard.Slider(settings.sizeRiver, 0.1f, 3f);


            listingStandard.GapLine();

            if (listingStandard.ButtonText("ZMD_resetThings".Translate()))
            {
                ResetThingsSettings();
            }
            listingStandard.End();

        }


        private void DrawFeaturesCard(Rect rect)
        {

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);

            listingStandard.Label("ZMD_featureTabInfo".Translate());


            if (listingStandard.ButtonTextLabeled("ZMD_selectFeature".Translate(), (new FeatureCardUtility()).GetFeatureLabel(settings.selectedFeature)))
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

            listingStandard.GapLine();

            (new FeatureCardUtility()).DrawFeatureOptions(listingStandard);

            listingStandard.End();

        }

        #endregion


        #region reset buttons

        private void ResetAllSettings()
        {
            ResetMountainSettings();
            ResetThingsSettings();
            //ResetFeatureSettings();
        }


        private void ResetMountainSettings()
        {
            settings.hillAmount = 1.0f;
            settings.hillSize = 0.021f;
            settings.hillSmoothness = 2.0f;
            MapDesignerSettings.flagCaves = true;
            MapDesignerSettings.flagOneRock = false;

            MapDesignerSettings.flagHillClumping = false;

            MapDesignerSettings.flagHillRadial = false;
            settings.hillRadialAmt = 1.0f;
            settings.hillRadialSize = 0.65f;
        }


        private void ResetThingsSettings()
        {
            settings.densityPlant = 1.0f;
            settings.densityAnimal = 1.0f;
            settings.densityRuins = 1.0f;
            settings.densityDanger = 1.0f;
            settings.densityGeyser = 1.0f;
            settings.densityOre = 1.0f;
            settings.sizeRiver = 1.0f;
        }


        #endregion


        #region labels

        private string hillRadialAmtLabel
        {
            get
            {
                float radialAmt = settings.hillRadialAmt;
                int label = 0;
                if (radialAmt > -2.0f)    //1
                {
                    label++;
                }
                if (radialAmt > -1.0f)    //2
                {
                    label++;
                }
                if (radialAmt > -0.4f)    //no cluster = 3
                {
                    label++;
                }
                if (radialAmt > 0.4f)     //4
                {
                    label++;
                }
                if (radialAmt > 1.0f)     //5
                {
                    label++;
                }
                if (radialAmt > 2.0f)     //6
                {
                    label++;
                }
                return FormatLabel("ZMD_hillRadialAmtLabel", "ZMD_hillRadialAmt" + label);
            }
        }


        private string hillRadialSizeLabel
        {
            get
            {
                float radialSize = settings.hillRadialSize;
                int label = 0;
                if (radialSize > 0.35f)
                {
                    label++;
                }
                if (radialSize > 0.55f)
                {
                    label++;
                }
                if (radialSize > 0.80f)
                {
                    label++;
                }
                if (radialSize > 0.95f)
                {
                    label++;
                }
                return FormatLabel("ZMD_hillRadialSizeLabel", "ZMD_hillRadialSize" + label);
            }
        }

        private string hillAmountLabel
        {
            get
            {
                int label = 0;
                if (settings.hillAmount > 0.5f)
                {
                    label++;
                }
                if (settings.hillAmount > 0.8f)
                {
                    label++;
                }
                if (settings.hillAmount > 1.2f)
                {
                    label++;
                }
                if (settings.hillAmount > 1.5f)
                {
                    label++;
                }
                if (settings.hillAmount > 1.75f)
                {
                    label++;
                }
                return FormatLabel("ZMD_hillAmountLabel", "ZMD_hillAmount" + label);
            }
        }


        private string hillSizeLabel
        {
            get
            {
                int label = 0;                      // huge

                if (settings.hillSize > 0.013f)   // vanilla
                {
                    label++;
                }
                if (settings.hillSize > 0.033f)  // small
                {
                    label++;
                }
                if (settings.hillSize > 0.055f)    // tiny
                {
                    label++;
                }
                if (settings.hillSize > 0.085f)   // very tiny
                {
                    label++;
                }

                return FormatLabel("ZMD_hillSizeLabel", "ZMD_hillSize" + label);
            }
        }


        private string hillSmoothnessLabel
        {
            get
            {
                int label = 0;
                if (settings.hillSmoothness > 0.8f)
                {
                    label++;
                }
                if (settings.hillSmoothness > 1.5f)
                {
                    label++;
                }
                if (settings.hillSmoothness > 2.5f)
                {
                    label++;
                }
                if (settings.hillSmoothness > 3.8f)
                {
                    label++;
                }

                return FormatLabel("ZMD_hillSmoothnessLabel", "ZMD_hillSmoothness" + label);
            }
        }


        private string FormatLabel(string label, string desc)
        {
            return String.Format("{0}: {1}", label.Translate(), desc.Translate());
        }


        private static int GetDensityLabel(float density)
        {
            int output = 0;
            if (density > 0.1f) //very low
            {
                output++;
            }
            if (density > 0.4f)  //low
            {
                output++;
            }
            if (density > 0.85f)  //average
            {
                output++;
            }
            if (density > 1.2f) //high
            {
                output++;
            }
            if (density > 1.7f) //very high
            {
                output++;
            }
            if (density > 2.3f) //extremely high
            {
                output++;
            }
            return output;
        }

        #endregion



    }
}
