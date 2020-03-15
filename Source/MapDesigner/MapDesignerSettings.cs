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

        public static bool flagCaves = true;
        //public static bool marshyBeaches = false;
        //public static bool multiSpawn = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref hillAmount, "hillAmount", 1.0f);
            Scribe_Values.Look(ref hillSize, "hillSize", 0.021f);
            Scribe_Values.Look(ref hillSmoothness, "hillSmoothness", 2.0f);

            Scribe_Values.Look(ref flagCaves, "flagCaves", true);
            //Scribe_Values.Look(ref marshyBeaches, "marshyBeaches", false);
            //Scribe_Values.Look(ref multiSpawn, "multiSpawn", false);
        }
    }

    public class MapDesigner_Mod : Mod
    {
        MapDesignerSettings settings;

        public MapDesigner_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MapDesignerSettings>();
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {
            //inRect.width = 450f;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            int hillAmountLabel = GetHillAmountLabel(settings.hillAmount);
            listingStandard.Label(("ZMD_hillAmount" + hillAmountLabel).Translate());
            settings.hillAmount = listingStandard.Slider(settings.hillAmount, 0f, 2.5f);

            listingStandard.Label(("ZMD_hillSize" + GetHillSizeLabel(settings.hillSize)).Translate());
            settings.hillSize = listingStandard.Slider(settings.hillSize, 0.01f, 0.10f);

            //listingStandard.Label("ZMD_hillSmoothness".Translate());
            listingStandard.Label(("ZMD_hillSmoothness" + GetHillSmoothnesstLabel(settings.hillSmoothness)).Translate() + settings.hillSmoothness);
            settings.hillSmoothness = listingStandard.Slider(settings.hillSmoothness, 0f, 5f);

            listingStandard.CheckboxLabeled("ZMD_FlagCaves".Translate(), ref MapDesignerSettings.flagCaves, "ZMD_FlagCavesTooltip".Translate());


            //listingStandard.CheckboxLabeled("ZPRI_MarshyBeachesLabel".Translate(), ref ZPRI_Settings.marshyBeaches);

            //listingStandard.CheckboxLabeled("ZPRI_MarshyBeachesLabel".Translate(), ref MapDesigner_Settings.marshyBeaches, "ZPRI_MarshyBeachesTooltip".Translate());

            //listingStandard.CheckboxLabeled("ZPRI_MultiSpawnLabel".Translate(), ref MapDesigner_Settings.multiSpawn);

            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ZMD_ModName".Translate();
        }

        #region labels

        private static int GetHillSizeLabel(float hillSize)
        {
            int label = 0;

            if(hillSize > 0.013f)   // vanilla
            {
                label++;
            }
            if (hillSize > 0.033f)  // small
            {
                label++;
            }
            if(hillSize > 0.055f)    // tiny
            {
                label++;
            }
            if (hillSize > 0.085f)   // very tiny
            {
                label++;
            }

            return label;

        }

        private static int GetHillAmountLabel(float hillAmount)
        {
            int label = 0;
            if(hillAmount > 0.5f)
            {
                label++;
            }
            if (hillAmount > 0.8f)
            {
                label++;
            }
            if (hillAmount > 1.2f)
            {
                label++;
            }
            if (hillAmount > 1.5f)
            {
                label++;
            }
            if (hillAmount > 1.75f)
            {
                label++;
            }
            return label;
            //return 3;
        }

        private static int GetHillSmoothnesstLabel(float hillSmoothness)
        {
            int label = 0;
            if (hillSmoothness > 0.8f)
            {
                label++;
            }
            if (hillSmoothness > 1.5f)
            {
                label++;
            }
            if (hillSmoothness > 2.5f)
            {
                label++;
            }
            if (hillSmoothness > 3.8f)
            {
                label++;
            }
            return label;
        }
        #endregion
    }
}
