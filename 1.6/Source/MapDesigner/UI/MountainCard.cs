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

    public static class MountainCard
    {
        public static MapDesignerSettings settings = MapDesignerMod.mod.settings;
        private static float viewHeight;
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawMountainCard(Rect rect)
        {
            HelperMethods.BeginChangeCheck();

            Rect rect2 = rect.ContractedBy(4f);

            // to correctly initialize scrollview height
            float height = 500f;
            if (settings.flagHillRadial)
            {
                height += 100f;
            }
            if (settings.flagHillSplit)
            {
                height += 140f;
            }
            if (settings.flagHillSide)
            {
                height += 100f;
            }
            if (settings.flagHillDonut)
            {
                height += 200f;
            }

            Rect viewRect = new Rect(0f, 0f, rect2.width - 18f, Math.Max(height, viewHeight + 200f));
            Widgets.BeginScrollView(rect2, ref scrollPosition, viewRect, true);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(viewRect);

            // mountains
            settings.hillAmount = InterfaceUtility.LabeledSlider(listingStandard, settings.hillAmount, 0.50f, 1.6f, hillAmountLabel, "ZMD_hillAmount0".Translate(), "ZMD_hillAmount5".Translate());

            // It's reversed because that's more intuitive for the user. Smaller numbers = bigger hills
            settings.hillSize = InterfaceUtility.LabeledSlider(listingStandard, settings.hillSize, 0.1f, 0.010f, hillSizeLabel, "ZMD_size1".Translate(), "ZMD_size5".Translate());
            settings.hillSmoothness = InterfaceUtility.LabeledSlider(listingStandard, settings.hillSmoothness, 0f, 5f, hillSmoothnessLabel, "ZMD_hillSmoothness0".Translate(), "ZMD_hillSmoothness4".Translate());

            // general mountain related
            listingStandard.CheckboxLabeled("ZMD_flagMtnExit".Translate(), ref MapDesignerMod.mod.settings.flagMtnExit, "ZMD_flagMtnExitTooltip".Translate());
           
            //TODO: CAVES
            //listingStandard.CheckboxLabeled("ZMD_flagCaves".Translate(), ref MapDesignerMod.mod.settings.flagCaves, "ZMD_flagCavesTooltip".Translate());

            // hill distribution
            listingStandard.GapLine();

            listingStandard.Label("ZMD_hillArrangementDesc".Translate());
            listingStandard.Gap();

            if (settings.hillSize > 0.022f)
            {
                listingStandard.CheckboxLabeled("ZMD_flagHillClumping".Translate(), ref MapDesignerMod.mod.settings.flagHillClumping, "ZMD_flagHillClumpingTooltip".Translate());
            }

            listingStandard.CheckboxLabeled("ZMD_flagHillRadial".Translate(), ref MapDesignerMod.mod.settings.flagHillRadial, "ZMD_flagHillRadialTooltip".Translate());
            if (MapDesignerMod.mod.settings.flagHillRadial)
            {
                Rect hillRadialRect = listingStandard.GetRect(100f);
                hillRadialRect.xMin += 20f;
                hillRadialRect.xMax -= 20f;

                Listing_Standard hillRadialListing = new Listing_Standard();
                hillRadialListing.Begin(hillRadialRect);

                //settings.hillRadialAmt = InterfaceUtility.LabeledSlider(hillRadialListing, settings.hillRadialAmt, -3.0f, 3.0f, GetHillRadialAmtLabel(settings.hillRadialAmt), "ZMD_center".Translate(), "ZMD_edges".Translate(), "ZMD_hillRadialAmtTooltip".Translate());
                settings.hillRadialAmt = InterfaceUtility.LabeledSlider(hillRadialListing, settings.hillRadialAmt, -2.5f, 2.5f, GetHillRadialAmtLabel(settings.hillRadialAmt), "ZMD_center".Translate(), "ZMD_edges".Translate(), null, "ZMD_hillRadialAmtTooltip".Translate());

                //settings.hillRadialSize = InterfaceUtility.LabeledSlider(hillRadialListing, settings.hillRadialSize, 0.2f, 1.1f, hillRadialSizeLabel, "ZMD_center".Translate(), "ZMD_edges".Translate(), null, "ZMD_hillRadialSizeTooltip".Translate());
                settings.hillRadialSize = InterfaceUtility.LabeledSlider(hillRadialListing, settings.hillRadialSize, 0.1f, 1.2f, hillRadialSizeLabel, null, null, String.Format("ZMD_pctOfMap".Translate(), 100 * Math.Round(settings.hillRadialSize, 1)), "ZMD_hillRadialSizeTooltip".Translate());

                

                hillRadialListing.End();
                listingStandard.Gap();
            }

            listingStandard.CheckboxLabeled("ZMD_flagHillSplit".Translate(), ref MapDesignerMod.mod.settings.flagHillSplit, "ZMD_flagHillSplit".Translate());
            if (MapDesignerMod.mod.settings.flagHillSplit)
            {
                Rect hillSplitRect = listingStandard.GetRect(140f);
                hillSplitRect.xMin += 20f;
                hillSplitRect.xMax -= 20f;
                Listing_Standard hillSplitListing = new Listing_Standard();
                hillSplitListing.Begin(hillSplitRect);
                settings.hillSplitAmt = InterfaceUtility.LabeledSlider(hillSplitListing, settings.hillSplitAmt, -3.0f, 3.0f, GetHillRadialAmtLabel(settings.hillSplitAmt), "ZMD_center".Translate(), "ZMD_edges".Translate(), null, "ZMD_hillRadialAmtTooltip".Translate());

                settings.hillSplitSize = InterfaceUtility.LabeledSlider(hillSplitListing, settings.hillSplitSize, 0.05f, 1.1f, "ZMD_size".Translate(), null, null, String.Format("ZMD_pctOfMap".Translate(), 100 * Math.Round(settings.hillSplitSize, 1)), "ZMD_hillRadialSizeTooltip".Translate());

                settings.hillSplitDir = InterfaceUtility.AnglePicker(hillSplitListing, settings.hillSplitDir, "ZMD_Angle".Translate());

                hillSplitListing.End();
                listingStandard.Gap();
            }

            listingStandard.CheckboxLabeled("ZMD_flagHillSide".Translate(), ref MapDesignerMod.mod.settings.flagHillSide, "ZMD_flagHillSide".Translate());
            if(MapDesignerMod.mod.settings.flagHillSide)
            {
                Rect hillSideRect = listingStandard.GetRect(100f);
                hillSideRect.xMin += 20f;
                hillSideRect.xMax -= 20f;
                Listing_Standard hillSideListing = new Listing_Standard();
                hillSideListing.Begin(hillSideRect);

                settings.hillSideAmt = InterfaceUtility.LabeledSlider(hillSideListing, settings.hillSideAmt, 0.2f, 3.0f, GetHillSideAmtLabel(settings.hillSideAmt));

                settings.hillSideDir = InterfaceUtility.AnglePicker(hillSideListing, settings.hillSideDir, "ZMD_Angle".Translate(), 3, true);

                hillSideListing.End();
                listingStandard.Gap();

            }

            listingStandard.CheckboxLabeled("ZMD_flagHillDonut".Translate(), ref MapDesignerMod.mod.settings.flagHillDonut, "ZMD_flagHillDonutTooltip".Translate());
            if (MapDesignerMod.mod.settings.flagHillDonut)
            {
                Rect hillDonutRect = listingStandard.GetRect(250f);
                hillDonutRect.xMin += 20f;
                hillDonutRect.xMax -= 20f;

                Listing_Standard hillDonutListing = new Listing_Standard();
                hillDonutListing.Begin(hillDonutRect);
                
                settings.hillDonutAmt = InterfaceUtility.LabeledSlider(hillDonutListing, settings.hillDonutAmt, -1.5f, 1.5f, GetHillDonutAmtLabel(settings.hillDonutAmt));
                settings.hillDonutSize = InterfaceUtility.LabeledSlider(hillDonutListing, settings.hillDonutSize, 0.1f, 1.2f, hillDonutSizeLabel, null, null, String.Format("ZMD_pctOfMap".Translate(), 100 * Math.Round(settings.hillDonutSize, 1)), "ZMD_hillRadialSizeTooltip".Translate());

                InterfaceUtility.LocationPicker(hillDonutListing, 0.45f, ref settings.hillDonutDisp, 100 * settings.hillDonutSize, "GUI/ZMD_ring");

                hillDonutListing.End();
            }

            HelperMethods.EndChangeCheck();

            // reset
            listingStandard.GapLine();
            //if (InterfaceUtility.SizedTextButton(listingStandard, "ZMD_resetMountain".Translate(), -1, true))
            if (listingStandard.ButtonText("ZMD_resetMountain".Translate()))
            {
                ResetMountainSettings();
            }

            listingStandard.End();

            viewHeight = listingStandard.CurHeight;
            Widgets.EndScrollView();

        }


        public static void ResetMountainSettings()
        {
            settings.hillAmount = 1.0f;
            settings.hillSize = 0.021f;
            settings.hillSmoothness = 2.0f;
            settings.flagMtnExit = false;
            settings.flagCaves = true;

            settings.flagHillClumping = false;

            settings.flagHillRadial = false;
            settings.hillRadialAmt = 1.0f;
            settings.hillRadialSize = 0.65f;

            settings.flagHillSplit = false;
            settings.hillSplitAmt = 1.5f;
            settings.hillSplitDir = 90f;
            settings.hillSplitSize = 0.35f;

            settings.flagHillSide = false;
            settings.hillSideAmt = 1.0f;
            settings.hillSideDir = 180f;

            settings.flagHillDonut = false;
            settings.hillDonutAmt = -0.9f;
            settings.hillDonutSize = 0.75f;
            settings.hillDonutDisp = new Vector3(0.0f, 0.0f, 0.0f);

            HelperMethods.InvokeOnSettingsChanged();

        }


        private static string GetHillRadialAmtLabel(float val)
        {
                int label = 0;
                if (val > -1.7f)    //1
                {
                    label++;
                }
                if (val > -1.0f)    //2
                {
                    label++;
                }
                if (val > -0.3f)    //no cluster = 3
                {
                    label++;
                }
                if (val > 0.3f)     //4
                {
                    label++;
                }
                if (val > 1.0f)     //5
                {
                    label++;
                }
                if (val > 1.7f)     //6
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_hillRadialAmtLabel", "ZMD_hillRadialAmt" + label);
        }


        private static string GetHillSideAmtLabel(float val)
        {
            int label = 0;
            if (val > 0.4f)    //1
            {
                label++;
            }
            if (val > 1.2f)    //2
            {
                label++;
            }
            if (val > 2.0f)    //no cluster = 3
            {
                label++;
            }
           
            return InterfaceUtility.FormatLabel("ZMD_hillRadialAmtLabel", "ZMD_hillSideAmt" + label);
        }

        private static string GetHillDonutAmtLabel(float val)
        {

            int label = 0;      // 0 - Distinct ring hill
            if (val > -1)    //1 Broken ring hill
            {
                label++;
            }
            if (val > -0.6f)    //2  Slight ring hill
            {
                label++;
            }
            if (val > -0.25f)    //no cluster = 3
            {
                label++;
            }
            if (val > 0.25f)    //4 Slight ring valley
            {
                label++;
            }
            if (val > 0.6f)    //5 Partial ring valley
            {
                label++;
            }
            if (val > 1f)    //6 Distinct ring valley
            {
                label++;
            }

            return InterfaceUtility.FormatLabel("ZMD_skew", "ZMD_hillDonutAmt" + label);
        }
        private static string hillRadialSizeLabel
        {
            get
            {
                float radialSize = settings.hillRadialSize;
                int label = 0;
                if (radialSize > 0.35f)
                {
                    label++;
                }
                if (radialSize > 0.55f)
                {
                    label++;
                }
                if (radialSize > 0.80f)
                {
                    label++;
                }
                if (radialSize > 0.95f)
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_hillRadialSizeLabel", "ZMD_hillRadialSize" + label);
            }
        }

        private static string hillDonutSizeLabel
        {
            get
            {
                float radialSize = settings.hillRadialSize;
                int label = 0;
                if (radialSize > 0.25f)
                {
                    label++;
                }
                if (radialSize > 0.45f)
                {
                    label++;
                }
                if (radialSize > 0.70f)
                {
                    label++;
                }
                if (radialSize > 0.90f)
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_size", "ZMD_hillRadialSize" + label);
            }
        }
        public static string hillAmountLabel
        {
            get
            {
                int label = 0;
                if (settings.hillAmount > 0.85f)
                {
                    label++;
                }
                if (settings.hillAmount > 0.95f)
                {
                    label++;
                }
                if (settings.hillAmount > 1.05f)
                {
                    label++;
                }
                if (settings.hillAmount > 1.15f)
                {
                    label++;
                }
                if (settings.hillAmount > 1.30f)
                {
                    label++;
                }
                return InterfaceUtility.FormatLabel("ZMD_hillAmountLabel", "ZMD_hillAmount" + label);
            }
        }


        private static string hillSizeLabel
        {
            get
            {
                int label = 0;                      // huge

                if (settings.hillSize > 0.013f)   // vanilla
                {
                    label++;
                }
                if (settings.hillSize > 0.033f)  // small
                {
                    label++;
                }
                if (settings.hillSize > 0.055f)    // tiny
                {
                    label++;
                }
                if (settings.hillSize > 0.085f)   // very tiny
                {
                    label++;
                }

                return InterfaceUtility.FormatLabel("ZMD_hillSizeLabel", "ZMD_hillSize" + label);
            }
        }


        private static string hillSmoothnessLabel
        {
            get
            {
                int label = 0;
                if (settings.hillSmoothness > 0.8f)
                {
                    label++;
                }
                if (settings.hillSmoothness > 1.5f)
                {
                    label++;
                }
                if (settings.hillSmoothness > 2.5f)
                {
                    label++;
                }
                if (settings.hillSmoothness > 3.8f)
                {
                    label++;
                }

                return InterfaceUtility.FormatLabel("ZMD_hillSmoothnessLabel", "ZMD_hillSmoothness" + label);
            }
        }

    }
}
