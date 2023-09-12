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
    public class ThingsIdeologyDialog : Window
    {
        public ThingsIdeologyDialog()
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
            mainListing.Label("ZMD_thingsIdeology".Translate());
            Text.Font = GameFont.Small;
            mainListing.GapLine();

            settings.countMechanoidRemains = (int)Math.Round(InterfaceUtility.LabeledSlider(mainListing, settings.countMechanoidRemains, 0, 15, null, "ZMD_countMechanoidRemains".Translate(), settings.countMechanoidRemains.ToString()));
            settings.densityAncientPipelineSection = ThingsCard.ThingsSlider(mainListing, settings.densityAncientPipelineSection, 2, "ZMD_densityAncientPipelineSection");
            settings.densityAncientJunkClusters = ThingsCard.ThingsSlider(mainListing, settings.densityAncientJunkClusters, 2, "ZMD_densityAncientJunkClusters");


            mainListing.CheckboxLabeled("ZMD_flagRoadDebris".Translate(), ref settings.flagRoadDebris, "ZMD_flagRoadDebrisTooltip".Translate());
            mainListing.CheckboxLabeled("ZMD_flagCaveDebris".Translate(), ref settings.flagCaveDebris, "ZMD_flagCaveDebrisTooltip".Translate());
            mainListing.CheckboxLabeled("ZMD_flagAncientUtilityBuilding".Translate(), ref settings.flagAncientUtilityBuilding, "ZMD_flagAncientUtilityBuildingTooltip".Translate());
            mainListing.CheckboxLabeled("ZMD_flagAncientTurret".Translate(), ref settings.flagAncientTurret, "ZMD_flagAncientTurret".Translate());
            mainListing.CheckboxLabeled("ZMD_flagAncientMechs".Translate(), ref settings.flagAncientMechs, "ZMD_flagAncientMechs".Translate());
            mainListing.CheckboxLabeled("ZMD_flagAncientLandingPad".Translate(), ref settings.flagAncientLandingPad, "ZMD_flagAncientLandingPad".Translate());
            mainListing.CheckboxLabeled("ZMD_flagAncientFences".Translate(), ref settings.flagAncientFences, "ZMD_flagAncientFences".Translate());
            mainListing.End();

            HelperMethods.EndChangeCheck();
        }

    }
}
