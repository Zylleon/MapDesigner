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
    public static class ThingsCardUtility
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        public static void DrawThingsCard(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);

            // stuff density
            settings.densityPlant = InterfaceUtility.LabeledSlider(listingStandard, settings.densityPlant, 0f, 2.5f, String.Format("ZMD_densityPlant".Translate(), HelperMethods.GetDisplayPercent(settings.densityPlant)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityAnimal = InterfaceUtility.LabeledSlider(listingStandard, settings.densityAnimal, 0f, 2.5f, String.Format("ZMD_densityAnimal".Translate(), HelperMethods.GetDisplayPercent(settings.densityAnimal)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityRuins = InterfaceUtility.LabeledSlider(listingStandard, settings.densityRuins, 0f, 2.5f, String.Format("ZMD_densityRuins".Translate(), HelperMethods.GetDisplayPercent(settings.densityRuins, 3)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityDanger = InterfaceUtility.LabeledSlider(listingStandard, settings.densityDanger, 0f, 2.5f, String.Format("ZMD_densityDanger".Translate(), HelperMethods.GetDisplayPercent(settings.densityDanger, 4)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityGeyser = InterfaceUtility.LabeledSlider(listingStandard, settings.densityGeyser, 0f, 2.5f, String.Format("ZMD_densityGeyser".Translate(), HelperMethods.GetDisplayPercent(settings.densityGeyser, 2)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityOre = InterfaceUtility.LabeledSlider(listingStandard, settings.densityOre, 0f, 2.5f, String.Format("ZMD_densityOre".Translate(), HelperMethods.GetDisplayPercent(settings.densityOre, 2)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

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
