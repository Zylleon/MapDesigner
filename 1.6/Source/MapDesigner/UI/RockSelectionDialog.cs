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
            HelperMethods.BeginChangeCheck();

            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            List<ThingDef> list = HelperMethods.GetRockList();

            Listing_Standard outerListing = new Listing_Standard();
            outerListing.Begin(inRect);


            // enable or disable all
            if (outerListing.ButtonText("ZMD_allRockTypes".Translate()))
            {
                AllowAllRocks(settings, list);
            }
            if (outerListing.ButtonText("ZMD_noRockTypes".Translate()))
            {
                DisableAllRocks(settings, list);
            }

            if(!settings.allowedRocks.Any(r => r.Value == true))
            {
                GUI.color = new Color(255, 180, 0);
                outerListing.Label("ZMD_noRockWarning".Translate());

                GUI.color = Color.white;
            }


            // scroll window with rock list
            Rect windowRect = outerListing.GetRect(inRect.height - outerListing.CurHeight).ContractedBy(4f);

            Rect viewRect = new Rect(0f, 0f, 200f, 50f + 29 * list.Count());

            Widgets.BeginScrollView(windowRect, ref scrollPosition, viewRect, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            foreach(ThingDef rock in list)
            {
                bool allowed = settings.allowedRocks[rock.defName];
                Rect singleRockRect = listing.GetRect(5f + Text.LineHeight);

                //the checkbox
                Rect checkBoxRect = singleRockRect.RightPartPixels(singleRockRect.width - Text.LineHeight * 1.5f);
                Listing_Standard checkboxListing = new Listing_Standard();
                checkboxListing.Begin(checkBoxRect);
                checkboxListing.CheckboxLabeled(rock.label, ref allowed, rock.description);
                settings.allowedRocks[rock.defName] = allowed;
                checkboxListing.End();

                //the color block
                Rect colorRect = singleRockRect.LeftPartPixels(Text.LineHeight);
                colorRect.height = Text.LineHeight;
                Color rockColor = new Color(1, 1, 1, 1);
                bool foundColor = false;

                // what color is it?
                ThingDef chunk = rock.building.mineableThing;
                if (chunk != null)
                {
                    ThingDef brick = chunk.butcherProducts.FirstOrDefault().thingDef;
                    if (brick != null)
                    {
                        Color newColor = brick.stuffProps.color;
                        if(newColor != null)
                        {
                            foundColor = true;
                            rockColor = newColor;
                        }
                    }
                    else
                    {
                        Color newColor = chunk.stuffProps.color;
                        if (newColor != null)
                        {
                            foundColor = true;
                            rockColor = newColor;
                        }
                    }
                }

                // If not found, draw the icon
                if (!foundColor)
                {
                    Texture2D noGraphicIcon = ContentFinder<Texture2D>.Get("GUI/ZMD_NoGraphic", true);
                    Widgets.DrawTextureRotated(colorRect, noGraphicIcon, 0);
                }
                // draw the color
                else
                {
                    Widgets.DrawBoxSolid(colorRect, rockColor);
                }
            }

            listing.End();

            Widgets.EndScrollView();

            outerListing.End();

            HelperMethods.EndChangeCheck();
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


        public void DisableAllRocks(MapDesignerSettings settings, List<ThingDef> list)
        {
            Dictionary<string, bool> newRocks = new Dictionary<string, bool>();

            foreach (ThingDef rock in list)
            {
                newRocks.Add(rock.defName, false);
            }

            settings.allowedRocks = newRocks;

            HelperMethods.InvokeOnSettingsChanged();
        }


    }
}
