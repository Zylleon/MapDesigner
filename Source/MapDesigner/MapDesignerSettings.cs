using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;


namespace MapDesigner
{

    public class MapDesignerSettings : ModSettings
    {
        public float hillAmount = 1.0f;
        public float hillSize = 0.021f;
        public float hillSmoothness = 2.0f;
        public static bool flagHillClumping = false;


        public float densityPlant = 1.0f;
        public float densityAnimal = 1.0f;
        public float densityRuins = 1.0f;
        //public float densityDanger = 1.0f;
        public float densityGeyser = 1.0f;
        public float densityOre = 1.0f;

        public float sizeRiver = 1.0f;

        public static bool flagCaves = true;



        public Dictionary<string, BiomeDefault> biomeDefaults;
        public Dictionary<string, FloatRange> densityDefaults;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref hillAmount, "hillAmount", 1.0f);
            Scribe_Values.Look(ref hillSize, "hillSize", 0.021f);
            Scribe_Values.Look(ref hillSmoothness, "hillSmoothness", 2.0f);

            Scribe_Values.Look(ref flagHillClumping, "flagHillClumping", false);

            Scribe_Values.Look(ref densityPlant, "densityPlant", 1.0f);
            Scribe_Values.Look(ref densityAnimal, "densityAnimal", 1.0f);
            Scribe_Values.Look(ref densityRuins, "densityRuins", 1.0f);
            //Scribe_Values.Look(ref densityDanger, "densityDanger", 1.0f);
            Scribe_Values.Look(ref densityGeyser, "densityGeyser", 1.0f);
            Scribe_Values.Look(ref densityOre, "densityOre", 1.0f);

            Scribe_Values.Look(ref sizeRiver, "sizeRiver", 1.0f);

            Scribe_Values.Look(ref flagCaves, "flagCaves", true);
        }
    }

    /*
    public class MapDesigner_Mod : Mod
    {
        MapDesignerSettings settings;

        //ZMD_LakeSettings lakeSettings;

        public MapDesigner_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MapDesignerSettings>();
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            // mountains
            listingStandard.Label(hillAmountLabel);
            settings.hillAmount = listingStandard.Slider(settings.hillAmount, 0f, 2.5f);

            listingStandard.Label(hillSizeLabel);
            settings.hillSize = listingStandard.Slider(settings.hillSize, 0.01f, 0.10f);

            if(settings.hillSize > 0.022f)
            {
                listingStandard.CheckboxLabeled("ZMD_flagHillClumping".Translate(), ref MapDesignerSettings.flagHillClumping, "ZMD_flagHillClumpingTooltip".Translate());
            }

            listingStandard.Label(hillSmoothnessLabel);
            settings.hillSmoothness = listingStandard.Slider(settings.hillSmoothness, 0f, 5f);


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

            listingStandard.CheckboxLabeled("ZMD_flagCaves".Translate(), ref MapDesignerSettings.flagCaves, "ZMD_flagCavesTooltip".Translate());

            // reset button
            listingStandard.GapLine();

            if (listingStandard.ButtonText("ZMD_Reset".Translate()))
            {
                ResetAllSettings();
            }


            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            HelperMethods.ApplyBiomeSettings();
        }

        public override string SettingsCategory()
        {
            return "ZMD_ModName".Translate();
        }


        private void ResetAllSettings()
        {
            settings.hillAmount = 1.0f;
            settings.hillSize = 0.021f;
            settings.hillSmoothness = 2.0f;
            MapDesignerSettings.flagHillClumping = false;


            settings.densityPlant = 1.0f;
            settings.densityAnimal = 1.0f;
            settings.densityRuins = 1.0f;
            //settings.densityDanger = 1.0f;
            settings.densityGeyser = 1.0f;
            settings.densityOre = 1.0f;

            MapDesignerSettings.flagCaves = true;
            settings.sizeRiver = 1.0f;
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

    */
}
