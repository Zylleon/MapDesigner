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
            Misc,
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
            //return "Map Designer test";
            return "ZMD_ModName".Translate();

        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            HelperMethods.ApplyBiomeSettings();
        }


        private void ResetAllSettings()
        {
            ResetMountainSettings();
            ResetMiscSettings();
        }

        private void ResetMountainSettings()
        {
            settings.hillAmount = 1.0f;
            settings.hillSize = 0.021f;
            settings.hillSmoothness = 2.0f;
            MapDesignerSettings.flagHillClumping = false;
            MapDesignerSettings.flagCaves = true;

        }

        private void ResetMiscSettings()
        {
            settings.densityPlant = 1.0f;
            settings.densityAnimal = 1.0f;
            settings.densityRuins = 1.0f;
            //settings.densityDanger = 1.0f;
            settings.densityGeyser = 1.0f;
            settings.densityOre = 1.0f;
            settings.sizeRiver = 1.0f;

        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            //Listing_Standard listingStandard = new Listing_Standard();
            //listingStandard.Begin(inRect);

            //Rect rect = new Rect(inRect);
            //rect = rect.ContractedBy(18f);
            //rect.height = 34f;
            //Text.Font = GameFont.Medium;
            //Widgets.Label(rect, this.tab.ToString().Translate());
            //Widgets.Label(rect, "ZMD_ModName".Translate());
            Rect rect2 = new Rect(inRect);
            //rect2.yMin = rect.yMax;
            rect2.yMax -= 38f;
            Rect rect3 = rect2;
            rect3.yMin += 45f;
            List<TabRecord> list = new List<TabRecord>();

            TabRecord mountainTab = new TabRecord("ZMD_MountainTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Mountains;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Mountains);
            list.Add(mountainTab);

            TabRecord miscTab = new TabRecord("ZMD_MiscTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Misc;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Misc);
            list.Add(miscTab);

            TabRecord featureTab = new TabRecord("ZMD_FeatureTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Feature;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Feature);
            list.Add(featureTab);

            TabRecord betaTab = new TabRecord("ZMD_BetaTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Beta;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Beta);
            list.Add(betaTab);

            TabDrawer.DrawTabs(rect3, list, 200f);
            this.FillCard(rect3.ContractedBy(18f));

            //base.DoSettingsWindowContents(inRect);
        }


        protected void FillCard(Rect cardRect)
        {
            if (this.tab == MapDesigner_Mod.InfoCardTab.Mountains)
            {
                // do mountain tab
                DrawMountainCard(cardRect);
            }
            else if (this.tab == MapDesigner_Mod.InfoCardTab.Misc)
            {
                // do misc tab
                DrawMiscCard(cardRect);
            }
            else if (this.tab == MapDesigner_Mod.InfoCardTab.Feature)
            {
                // do feature tab

                //cardRect.yMin += 8f;
                //HealthCardUtility.DrawPawnHealthCard(cardRect, (Pawn)this.thing, false, false, null);
            }
            else if (this.tab == MapDesigner_Mod.InfoCardTab.Beta)
            {
                // do beta tab

                //RecordsCardUtility.DrawRecordsCard(cardRect, (Pawn)this.thing);
            }
        }


        private void DrawMountainCard(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);

            // mountains
            listingStandard.Label(hillAmountLabel);
            settings.hillAmount = listingStandard.Slider(settings.hillAmount, 0f, 2.5f);

            listingStandard.Label(hillSizeLabel);
            settings.hillSize = listingStandard.Slider(settings.hillSize, 0.01f, 0.10f);

            if (settings.hillSize > 0.022f)
            {
                listingStandard.CheckboxLabeled("ZMD_flagHillClumping".Translate(), ref MapDesignerSettings.flagHillClumping, "ZMD_flagHillClumpingTooltip".Translate());
            }

            listingStandard.Label(hillSmoothnessLabel);
            settings.hillSmoothness = listingStandard.Slider(settings.hillSmoothness, 0f, 5f);

            listingStandard.CheckboxLabeled("ZMD_flagCaves".Translate(), ref MapDesignerSettings.flagCaves, "ZMD_flagCavesTooltip".Translate());

            if (listingStandard.ButtonText("ZMD_ResetMountain".Translate()))
            {
                ResetMountainSettings();
            }
        }

        private void DrawMiscCard(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            // stuff density
            listingStandard.GapLine();

            listingStandard.Label(FormatLabel("ZMD_densityPlant", "ZMD_density" + GetDensityLabel(settings.densityPlant)));
            settings.densityPlant = listingStandard.Slider(settings.densityPlant, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityAnimal", "ZMD_density" + GetDensityLabel(settings.densityAnimal)));
            settings.densityAnimal = listingStandard.Slider(settings.densityAnimal, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityRuins", "ZMD_density" + GetDensityLabel(settings.densityRuins)));
            settings.densityRuins = listingStandard.Slider(settings.densityRuins, 0f, 2.5f);

            //listingStandard.Label(FormatLabel("ZMD_densityDanger", "ZMD_density" + GetDensityLabel(settings.densityDanger)));
            //settings.densityDanger = listingStandard.Slider(settings.densityDanger, 0f, 2.5f);


            listingStandard.Label(FormatLabel("ZMD_densityGeyser", "ZMD_density" + GetDensityLabel(settings.densityGeyser)));
            settings.densityGeyser = listingStandard.Slider(settings.densityGeyser, 0f, 2.5f);

            listingStandard.Label(FormatLabel("ZMD_densityOre", "ZMD_density" + GetDensityLabel(settings.densityOre)));
            settings.densityOre = listingStandard.Slider(settings.densityOre, 0f, 2.5f);

            // misc
            listingStandard.GapLine();

            listingStandard.Label(FormatLabel("ZMD_sizeRiver", "ZMD_size" + GetDensityLabel(settings.sizeRiver)));
            settings.sizeRiver = listingStandard.Slider(settings.sizeRiver, 0.1f, 2.5f);

            if (listingStandard.ButtonText("ZMD_ResetMisc".Translate()))
            {
                ResetMiscSettings();
            }

        }


        #region labels


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
