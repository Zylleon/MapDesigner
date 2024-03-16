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
    public class ThingsBiotechDialog : Window
    {
        public ThingsBiotechDialog()
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
            mainListing.Label("ZMD_thingsBiotech".Translate());
            Text.Font = GameFont.Small;
            mainListing.GapLine();

            settings.densityAncientPollutionJunk = ThingsCard.ThingsSlider(mainListing, settings.densityAncientPollutionJunk, 3, "ZMD_densityAncientPollutionJunk");
            settings.densityPoluxTrees = ThingsCard.ThingsSlider(mainListing, settings.densityPoluxTrees, 3, "ZMD_densityPoluxTrees");
            settings.countAncientExostriderRemains = (int)Math.Round(InterfaceUtility.LabeledSlider(mainListing, settings.countAncientExostriderRemains, 0, 15, null, "ZMD_countAncientExostriderRemains".Translate(), settings.countAncientExostriderRemains.ToString()));
            settings.pollutionLevel = ThingsCard.ThingsSlider(mainListing, settings.pollutionLevel, 3, "ZMD_pollutionLevel");
            mainListing.End();

            HelperMethods.EndChangeCheck();

        }

    }
}
