using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace MapDesigner.UI
{
    public static class GeneralCardUtility
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        public static void DrawGeneralCard(Rect rect)
        {

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            //listing.Label("ZMD_generalTabInfo".Translate());

            listing.CheckboxLabeled("ZMD_flagPopup".Translate(), ref MapDesignerSettings.flagPopup);
            listing.Label("ZMD_flagPopupDesc".Translate());

            listing.GapLine();

            //listing.Label("ZMD_mapInfo".Translate());


            //settings.hillAmount = InterfaceUtility.LabeledSlider(listing, settings.hillAmount, 0f, 2.5f, (new MountainCardUtility()).hillAmountLabel, "ZMD_hillAmount0".Translate(), "ZMD_hillAmount5".Translate());

            //settings.densityPlant = InterfaceUtility.LabeledSlider(listing, settings.densityPlant, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityPlant", "ZMD_density" + ThingsCardUtility.GetDensityLabel(settings.densityPlant)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            //settings.densityAnimal = InterfaceUtility.LabeledSlider(listing, settings.densityAnimal, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityAnimal", "ZMD_density" + GetDensityLabel(settings.densityAnimal)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            //settings.densityOre = InterfaceUtility.LabeledSlider(listing, settings.densityOre, 0f, 2.5f, InterfaceUtility.FormatLabel("ZMD_densityOre", "ZMD_density" + GetDensityLabel(settings.densityOre)), "ZMD_density0".Translate(), "ZMD_density6".Translate());

            //InterfaceUtility.LabeledIntRange(listing, ref settings.rockTypeRange, 1, 5, "ZMD_rockTypeRange".Translate());


            listing.End();

        }

    }
}
