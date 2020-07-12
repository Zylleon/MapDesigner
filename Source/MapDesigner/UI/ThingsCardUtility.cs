﻿using System;
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
            settings.densityPlant = InterfaceUtility.LabeledSlider(listingStandard, settings.densityPlant, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityPlant", "ZMD_density" + GetDensityLabel(settings.densityPlant)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityAnimal = InterfaceUtility.LabeledSlider(listingStandard, settings.densityAnimal, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityAnimal", "ZMD_density" + GetDensityLabel(settings.densityAnimal)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityRuins = InterfaceUtility.LabeledSlider(listingStandard, settings.densityRuins, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityRuins", "ZMD_density" + GetDensityLabel(settings.densityRuins)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityDanger = InterfaceUtility.LabeledSlider(listingStandard, settings.densityDanger, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityDanger", "ZMD_density" + GetDensityLabel(settings.densityDanger)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityGeyser = InterfaceUtility.LabeledSlider(listingStandard, settings.densityGeyser, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityGeyser", "ZMD_density" + GetDensityLabel(settings.densityGeyser)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            settings.densityOre = InterfaceUtility.LabeledSlider(listingStandard, settings.densityOre, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityOre", "ZMD_density" + GetDensityLabel(settings.densityOre)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            if (ModsConfig.RoyaltyActive)
            {
                settings.animaCount = InterfaceUtility.LabeledSlider(listingStandard, settings.animaCount, 1f, 15f, "ZMD_animaCount".Translate(), "1", "15");
            }

            // Rocks
            listingStandard.GapLine();
            InterfaceUtility.LabeledIntRange(listingStandard, ref settings.rockTypeRange, 1, 5, "ZMD_rockTypeRange".Translate());

            listingStandard.CheckboxLabeled("ZMD_flagBiomeRocks".Translate(), ref MapDesignerSettings.flagBiomeRocks, "ZMD_flagBiomeRocksTooltip".Translate());

            listingStandard.GapLine();

            if (listingStandard.ButtonText("ZMD_chooseRockTypes".Translate()))
            {
                Find.WindowStack.Add(new RockSelectionDialog());
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

            settings.rockTypeRange = new IntRange(2, 3);
            MapDesignerSettings.flagBiomeRocks = false;
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
