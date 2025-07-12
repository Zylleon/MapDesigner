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
    public class ThingsRoyaltyDialog : Window
    {
        public ThingsRoyaltyDialog()
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
            mainListing.Label("ZMD_thingsRoyalty".Translate());
            Text.Font = GameFont.Small;
            mainListing.GapLine();

            settings.animaCount = (float)Math.Round(InterfaceUtility.LabeledSlider(mainListing, settings.animaCount, 0f, 15f, null, "ZMD_animaCount".Translate(), settings.animaCount.ToString()));

            mainListing.End();

            HelperMethods.EndChangeCheck();
        }

    }
}
