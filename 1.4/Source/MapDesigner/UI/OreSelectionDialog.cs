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
    public class OreSelectionDialog : Window
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public OreSelectionDialog()
        {
            //this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
            this.closeOnAccept = false;
            this.optionalTitle = "ZMD_chooseOreTypes".Translate();
            this.doCloseX = true;
            this.doCloseButton = true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            bool prevChanged = GUI.changed;
            GUI.changed = false;

            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            List<ThingDef> list = HelperMethods.GetMineableList();

            Listing_Standard outerListing = new Listing_Standard();
            outerListing.Begin(inRect);

            if (outerListing.ButtonText("ZMD_resetOreTypes".Translate()))
            {
                ResetAllOre(settings, list);
            }

            Rect windowRect = outerListing.GetRect(inRect.height - outerListing.CurHeight).ContractedBy(4f);

            Rect viewRect = new Rect(0f, 0f, 400f, 50f + 46 * list.Count());

            Widgets.BeginScrollView(windowRect, ref scrollPosition, viewRect, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            foreach(ThingDef ore in list)
            {
                float commonality = settings.oreCommonality[ore.defName];

                //listing.CheckboxLabeled(ore.label, ref commonality);

                commonality = InterfaceUtility.LabeledSlider(listing, settings.oreCommonality[ore.defName], 0f, 2f, ore.label); ;

                settings.oreCommonality[ore.defName] = commonality;
            }

            listing.End();

            Widgets.EndScrollView();


            if (GUI.changed)
            {
                HelperMethods.InvokeOnSettingsChanged();
            }
            GUI.changed = GUI.changed || prevChanged;
        }


        public void ResetAllOre(MapDesignerSettings settings, List<ThingDef> list)
        {
            Dictionary<string, float> newOre = new Dictionary<string, float>();

            foreach (ThingDef ore in list)
            {
                newOre.Add(ore.defName, 1f);
            }

            settings.oreCommonality = newOre;
        }

        

    }
}
