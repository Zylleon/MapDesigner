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
            HelperMethods.BeginChangeCheck();

            Listing_Standard wrapperListing = new Listing_Standard();
            wrapperListing.Begin(rect);

            Listing_Standard mainListing = new Listing_Standard();
            mainListing.ColumnWidth = 0.5f * rect.width - 17f;
            //mainListing.ColumnWidth = 0.333f * rect.width - 34f;


            Rect mainRect = wrapperListing.GetRect(rect.height - 36f);
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
            
            HelperMethods.EndChangeCheck();

            if (InterfaceUtility.SizedTextButton(mainListing, "ZMD_chooseOreTypes".Translate()))
            {
                Find.WindowStack.Add(new OreSelectionDialog());
            }
            
            HelperMethods.BeginChangeCheck();

            mainListing.CheckboxLabeled("ZMD_flagRockChunks".Translate(), ref settings.flagRockChunks, "ZMD_flagRockChunks".Translate());

            mainListing.NewColumn();

            //Text.Font = GameFont.Medium;
            Text.Font = GameFont.Medium;
            mainListing.Label("ZMD_thingsDlcAndMods".Translate());
            Text.Font = GameFont.Small; 
            
            if (ModsConfig.RoyaltyActive)
            {
                if (InterfaceUtility.SizedTextButton(mainListing, "ZMD_thingsRoyalty".Translate(), 300f, true))
                {
                    Find.WindowStack.Add(new ThingsRoyaltyDialog());
                }
            }


            if (ModsConfig.IdeologyActive)
            {
                if (InterfaceUtility.SizedTextButton(mainListing, "ZMD_thingsIdeology".Translate(), 300f, true))
                {
                    Find.WindowStack.Add(new ThingsIdeologyDialog());
                }
            }


            if (ModsConfig.BiotechActive)
            {
                if (InterfaceUtility.SizedTextButton(mainListing, "ZMD_thingsBiotech".Translate(), 300f, true))
                {
                    Find.WindowStack.Add(new ThingsIdeologyDialog());
                }

            }


            if (ModsConfig.IsActive("DankPyon.Medieval.Overhaul") || ModsConfig.IsActive("DankPyon.Medieval.Overhaul_steam"))
            {
                if (InterfaceUtility.SizedTextButton(mainListing, "ZMD_thingsMedievalOverhaul".Translate(), 300f, true))
                {
                    Find.WindowStack.Add(new ThingsMODialog());
                }
            }

            // VPE
            //if (ModsConfig.IsActive("VanillaExpanded.VFEPower"))
            //{
            //    if (InterfaceUtility.SizedTextButton(mainListing, "ZMD_thingsVPE".Translate(), 300f, true))
            //    {
            //        Find.WindowStack.Add(new ThingsVEDialog());
            //    }
            //}
            //Text.Font = GameFont.Small;

            HelperMethods.EndChangeCheck();

            mainListing.End();

            // Reset
            //if (wrapperListing.ButtonText("ZMD_resetThings".Translate()))
            //{
            //    ResetThingsSettings();
            //}

            if (InterfaceUtility.SizedTextButton(wrapperListing, "ZMD_resetThings".Translate(), 300f, true))
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

            // Biotech
            settings.densityAncientPollutionJunk = 1f;
            settings.countAncientExostriderRemains = 1;
            settings.densityPoluxTrees = 1f;
            settings.pollutionLevel = 1;

            // MO
            settings.densityMOBattlefield = 1f;
            settings.densityMOTarPit = 1f;
            settings.densityMOBeeHive = 1f;
            settings.densityMOHornet = 1f;

            // VPE
            //settings.vpe_ChemfuelPonds = new IntRange(1, 3);
            //settings.vpe_HelixienVents = new IntRange(1, 2);

            new OreSelectionDialog().ResetAllOre(settings, HelperMethods.GetMineableList());
            HelperMethods.InvokeOnSettingsChanged();

        }

        public static float ThingsSlider(Listing_Standard listing, float val = 1f, float power = 1f, string label = null)
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
