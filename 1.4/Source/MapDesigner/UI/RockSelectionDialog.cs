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
    public class RockSelectionDialog : Window
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public RockSelectionDialog()
        {
            //this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
            this.closeOnAccept = false;
            this.optionalTitle = "ZMD_chooseRockTypes".Translate();
            this.doCloseX = true;
            this.doCloseButton = true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            bool prevChanged = GUI.changed;
            GUI.changed = false;

            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            List<ThingDef> list = HelperMethods.GetRockList();

            Listing_Standard outerListing = new Listing_Standard();
            outerListing.Begin(inRect);

            if (outerListing.ButtonText("ZMD_allRockTypes".Translate()))
            {
                AllowAllRocks(settings, list);
            }

            Rect windowRect = outerListing.GetRect(inRect.height - outerListing.CurHeight).ContractedBy(4f);

            Rect viewRect = new Rect(0f, 0f, 200f, 50f + 24 * list.Count());

            Widgets.BeginScrollView(windowRect, ref scrollPosition, viewRect, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            foreach(ThingDef rock in list)
            {
                bool allowed = settings.allowedRocks[rock.defName];
                listing.CheckboxLabeled(rock.label, ref allowed);
                settings.allowedRocks[rock.defName] = allowed;
            }

            listing.End();

            Widgets.EndScrollView();

            if (GUI.changed)
            {
                HelperMethods.InvokeOnSettingsChanged();
            }
            GUI.changed = GUI.changed || prevChanged;
        }


        public void AllowAllRocks(MapDesignerSettings settings, List<ThingDef> list)
        {
            Dictionary<string, bool> newRocks = new Dictionary<string, bool>();

            foreach (ThingDef rock in list)
            {
                newRocks.Add(rock.defName, true);
            }

            settings.allowedRocks = newRocks;

            HelperMethods.InvokeOnSettingsChanged();
        }

        

    }
}
