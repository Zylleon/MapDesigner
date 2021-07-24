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
    public static class ThingsCard
    {
        public static MapDesignerSettings settings = MapDesignerMod.mod.settings;

        public static void DrawThingsCard(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);

            // stuff density
            settings.densityPlant = ThingsSlider(listingStandard, settings.densityPlant, 1, "ZMD_densityPlant");
            settings.densityAnimal = ThingsSlider(listingStandard, settings.densityAnimal, 1, "ZMD_densityAnimal");
            settings.densityRuins = ThingsSlider(listingStandard, settings.densityRuins, 3, "ZMD_densityRuins");
            //power is 3 here to match up with actual results
            settings.densityDanger = ThingsSlider(listingStandard, settings.densityDanger, 3, "ZMD_densityDanger");
            settings.densityGeyser = ThingsSlider(listingStandard, settings.densityGeyser, 2, "ZMD_densityGeyser");
            settings.densityOre = ThingsSlider(listingStandard, settings.densityOre, 2, "ZMD_densityOre");

            //TODO: Fix and enable ore commonality selection
            //if (InterfaceUtility.SizedTextButton(listingStandard, "ZMD_chooseOreTypes".Translate()))
            //{
            //    Find.WindowStack.Add(new OreSelectionDialog());
            //}

            listingStandard.CheckboxLabeled("ZMD_flagRockChunks".Translate(), ref settings.flagRockChunks, "ZMD_flagRockChunksTooltip".Translate());


            if (ModsConfig.RoyaltyActive)
            {
                settings.animaCount = (float)Math.Round(InterfaceUtility.LabeledSlider(listingStandard, settings.animaCount, 0f, 15f, "ZMD_animaCount".Translate() + settings.animaCount));
            }

            // Reset
            if (listingStandard.ButtonText("ZMD_resetThings".Translate()))
            {
                ResetThingsSettings();
            }
            listingStandard.End();
        }


        public static void ResetThingsSettings()
        {
            settings.densityPlant = 1.0f;
            settings.densityAnimal = 1.0f;
            settings.densityRuins = 1.0f;
            settings.densityDanger = 1.0f;
            settings.densityGeyser = 1.0f;
            settings.densityOre = 1.0f;
            settings.animaCount = 1.0f;
            settings.flagRockChunks = true;
        }

        private static float ThingsSlider(Listing_Standard listing, float val = 1f, float power = 1f, string label = null)
        {
            return InterfaceUtility.LabeledSlider(listing, val, 0f, 2.5f, UI.InterfaceUtility.FormatLabel(label, "ZMD_density" + GetDensityLabel(val)), null, null, HelperMethods.GetDisplayValue(val, power) + " x");
        }

        private static string GetSliderLabel(string label, float value, float power)
        {

            string output = String.Format(label.Translate(), HelperMethods.GetDisplayValue(value, power));

            output = String.Format("{0}\n{1}", output, ("ZMD_density" + GetDensityLabel(value)).Translate());
            return output;

        }


        public static int GetDensityLabel(float density)
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


    }
}
