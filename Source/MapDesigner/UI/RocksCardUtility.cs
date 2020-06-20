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
    public static class RocksCardUtility
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawRocksCard(Rect rect)
        {
            rect.height -= 50f;
            Listing_Standard outerListing = new Listing_Standard();
            outerListing.Begin(rect);

            InterfaceUtility.LabeledIntRange(outerListing, ref settings.rockTypeRange, 1, 5, "ZMD_rockTypeRange".Translate());

            outerListing.CheckboxLabeled("ZMD_flagBiomeRocks".Translate(), ref MapDesignerSettings.flagBiomeRocks, "ZMD_flagBiomeRocksTooltip".Translate());
            outerListing.GapLine();

            Listing_Standard listing = new Listing_Standard();
            //Rect rect2 = rect.ContractedBy(4f);
            Rect rect2 = outerListing.GetRect(rect.height - outerListing.CurHeight).ContractedBy(4f);
            Rect viewRect = new Rect(0f, 0f, rect2.width - 18f, viewHeight + 2000f);

            Widgets.BeginScrollView(rect2, ref scrollPosition, viewRect, true);
           
            listing.Begin(viewRect);

            // Rock types
            for (int i = 0; i < 50; i++)
            {
                listing.Label("This is row # " + i);
            }

            listing.Label("That's all there is.............................");

            viewRect.height = listing.CurHeight;

            listing.End();

            Widgets.EndScrollView();
            outerListing.End();
        }

        public static void ResetRocksSettings()
        {
            settings.rockTypeRange = new IntRange(2, 3);
            MapDesignerSettings.flagBiomeRocks = false;
        }

    }
}
