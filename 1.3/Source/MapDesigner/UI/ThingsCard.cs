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
            Listing_Standard wrapperListing = new Listing_Standard();
            wrapperListing.Begin(rect);

            Listing_Standard mainListing = new Listing_Standard();
            mainListing.ColumnWidth = 0.5f * rect.width - 17f;


            Rect mainRect = wrapperListing.GetRect(rect.height - 40f);
            //Rect mainRect = new Rect(rect.xMin, rect.yMin, rect.width, rect.height );

            mainListing.Begin(mainRect);

            Text.Font = GameFont.Medium;
            mainListing.Label("ZMD_thingsClassic".Translate());
            Text.Font = GameFont.Small;

            // stuff density
            settings.densityPlant = ThingsSlider(mainListing, settings.densityPlant, 1, "ZMD_densityPlant");
            settings.densityAnimal = ThingsSlider(mainListing, settings.densityAnimal, 1, "ZMD_densityAnimal");
            settings.densityRuins = ThingsSlider(mainListing, settings.densityRuins, 3, "ZMD_densityRuins");
            //power is 3 here to match up with actual results
            settings.densityDanger = ThingsSlider(mainListing, settings.densityDanger, 3, "ZMD_densityDanger");
            settings.densityGeyser = ThingsSlider(mainListing, settings.densityGeyser, 2, "ZMD_densityGeyser");
            settings.densityOre = ThingsSlider(mainListing, settings.densityOre, 2, "ZMD_densityOre");

            if (InterfaceUtility.SizedTextButton(mainListing, "ZMD_chooseOreTypes".Translate()))
            {
                Find.WindowStack.Add(new OreSelectionDialog());
            }

            mainListing.CheckboxLabeled("ZMD_flagRockChunks".Translate(), ref settings.flagRockChunks, "ZMD_flagRockChunks".Translate());


            if (ModsConfig.RoyaltyActive)
            {
                mainListing.Gap();
                Text.Font = GameFont.Medium;
                mainListing.Label("ZMD_thingsRoyalty".Translate());
                Text.Font = GameFont.Small;
                settings.animaCount = (float)Math.Round(InterfaceUtility.LabeledSlider(mainListing, settings.animaCount, 0f, 15f, null, "ZMD_animaCount".Translate(), settings.animaCount.ToString()));
            }

            if (ModsConfig.IdeologyActive)
            {
                mainListing.NewColumn();
                Text.Font = GameFont.Medium;
                mainListing.Label("ZMD_thingsIdeology".Translate());
                Text.Font = GameFont.Small;

                settings.countMechanoidRemains = (int)Math.Round(InterfaceUtility.LabeledSlider(mainListing, settings.countMechanoidRemains, 0, 15, null, "ZMD_countMechanoidRemains".Translate(), settings.countMechanoidRemains.ToString()));
                settings.densityAncientPipelineSection = ThingsSlider(mainListing, settings.densityAncientPipelineSection, 2, "ZMD_densityAncientPipelineSection");
                settings.densityAncientJunkClusters = ThingsSlider(mainListing, settings.densityAncientJunkClusters, 2, "ZMD_densityAncientJunkClusters");


                mainListing.CheckboxLabeled("ZMD_flagRoadDebris".Translate(), ref settings.flagRoadDebris, "ZMD_flagRoadDebrisTooltip".Translate());
                mainListing.CheckboxLabeled("ZMD_flagCaveDebris".Translate(), ref settings.flagCaveDebris, "ZMD_flagCaveDebrisTooltip".Translate());
                mainListing.CheckboxLabeled("ZMD_flagAncientUtilityBuilding".Translate(), ref settings.flagAncientUtilityBuilding, "ZMD_flagAncientUtilityBuildingTooltip".Translate());
                mainListing.CheckboxLabeled("ZMD_flagAncientTurret".Translate(), ref settings.flagAncientTurret, "ZMD_flagAncientTurret".Translate());
                mainListing.CheckboxLabeled("ZMD_flagAncientMechs".Translate(), ref settings.flagAncientMechs, "ZMD_flagAncientMechs".Translate());
                mainListing.CheckboxLabeled("ZMD_flagAncientLandingPad".Translate(), ref settings.flagAncientLandingPad, "ZMD_flagAncientLandingPad".Translate());
                mainListing.CheckboxLabeled("ZMD_flagAncientFences".Translate(), ref settings.flagAncientFences, "ZMD_flagAncientFences".Translate());
            }

            mainListing.End();

            // Reset
            if (wrapperListing.ButtonText("ZMD_resetThings".Translate()))
            {
                ResetThingsSettings();
            }
            wrapperListing.End();
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

            // Ideology
            settings.flagRoadDebris = true;
            settings.flagCaveDebris = true;
            settings.flagAncientUtilityBuilding = true;
            settings.flagAncientTurret = true;
            settings.flagAncientMechs = true;
            settings.flagAncientLandingPad = true;
            settings.flagAncientFences = true;

            settings.countMechanoidRemains = 1;
            settings.densityAncientPipelineSection = 1f;
            settings.densityAncientJunkClusters = 1f;

            new OreSelectionDialog().ResetAllOre(settings, HelperMethods.GetMineableList());
        }

        private static float ThingsSlider(Listing_Standard listing, float val = 1f, float power = 1f, string label = null)
        {
            //return InterfaceUtility.LabeledSlider(listing, val, 0f, 2.5f, UI.InterfaceUtility.FormatLabel(label, "ZMD_density" + GetDensityLabel(val)), null, null, HelperMethods.GetDisplayValue(val, power) + " x");
            return InterfaceUtility.LabeledSlider(listing, val, 0f, 2.5f, null, label.Translate(), HelperMethods.GetDisplayValue(val, power) + " x");

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
