using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace MapDesigner.UI
{
    public class ThingsMODialog : Window
    {
        public ThingsMODialog()
        {
            //this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
            this.closeOnAccept = false;
            this.optionalTitle = "ZMD_thingsTab".Translate();
            this.doCloseX = true;
            this.doCloseButton = true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            HelperMethods.BeginChangeCheck();

            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            List<ThingDef> list = HelperMethods.GetMineableList();

            Listing_Standard mainListing = new Listing_Standard();
            mainListing.Begin(inRect);

            Text.Font = GameFont.Medium;
            mainListing.Label("ZMD_thingsMedievalOverhaul".Translate());
            Text.Font = GameFont.Small;
            mainListing.GapLine();

            settings.densityMOBattlefield = ThingsCard.ThingsSlider(mainListing, settings.densityMOBattlefield, 3, "ZMD_densityMOBattlefield");
            settings.densityMOTarPit = ThingsCard.ThingsSlider(mainListing, settings.densityMOTarPit, 3, "ZMD_densityMOTarPit");
            settings.densityMOBeeHive = ThingsCard.ThingsSlider(mainListing, settings.densityMOBeeHive, 3, "ZMD_densityMOBeeHive");
            settings.densityMOHornet = ThingsCard.ThingsSlider(mainListing, settings.densityMOHornet, 3, "ZMD_densityMOHornet");

            HelperMethods.EndChangeCheck();

        }

    }
}
