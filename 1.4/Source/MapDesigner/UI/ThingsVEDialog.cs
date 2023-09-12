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
    public class ThingsVEDialog : Window
    {
        public ThingsVEDialog()
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
            mainListing.Label("ZMD_thingsVPE".Translate());
            Text.Font = GameFont.Small;
            mainListing.GapLine();

            InterfaceUtility.LabeledIntRange(mainListing, ref settings.vpe_ChemfuelPonds, 0, 20, "ZMD_VPE_chemfuelPonds".Translate());
            InterfaceUtility.LabeledIntRange(mainListing, ref settings.vpe_HelixienVents, 0, 20, "ZMD_VPE_helixienVents".Translate());

            HelperMethods.EndChangeCheck();
        }

    }
}
