﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;


namespace MapDesigner.UI
{
    public static class RiversCard
    {
        public static MapDesignerSettings settings = MapDesignerMod.mod.settings;

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawRiversCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.GapLine();

            // size and style
            settings.sizeRiver = InterfaceUtility.LabeledSlider(listing, settings.sizeRiver, 0.1f, 3f, InterfaceUtility.FormatLabel("ZMD_sizeRiver", "ZMD_size" + ThingsCard.GetDensityLabel(settings.sizeRiver)), "ZMD_size1".Translate(), "ZMD_size6".Translate());
            Rect selStyleRect = listing.GetRect(40f);

            Rect selButtonRect = selStyleRect;
            Rect descRect = selStyleRect;
            selButtonRect.xMax -= 0.66f * rect.width;
            descRect.xMin += 0.34f * rect.width;
            Rect iconRect = new Rect(descRect);
            iconRect.xMin += 10f;
            iconRect.xMax = iconRect.xMin + 40f;

            string texPath = String.Format("GUI/{0}", GetRiverStyleLabel(settings.selRiverStyle));
            Texture2D icon = ContentFinder<Texture2D>.Get(texPath, true);
            Widgets.DrawTextureRotated(iconRect, icon, 0);
            descRect.xMin += 60f;

            Widgets.Label(descRect, (GetRiverStyleLabel(settings.selRiverStyle) + "Desc").Translate());
            //Rect descRect = listing.GetRect(60f).RightHalf();
            Listing_Standard Listing_selRiverStyle = new Listing_Standard();
            Listing_selRiverStyle.Begin(selButtonRect);

            // river style selection
            if (Listing_selRiverStyle.ButtonTextLabeled("ZMD_riverStyle".Translate(), GetRiverStyleLabel(settings.selRiverStyle).Translate()))
            {
                List<FloatMenuOption> riverStyleList = new List<FloatMenuOption>();

                riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleVanilla".Translate(), delegate
                {
                    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Vanilla;
                }));
                riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleSpring".Translate(), delegate
                {
                    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Spring;
                }));
                //riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleConfluence".Translate(), delegate
                //{
                //    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Confluence;
                //}));
                //riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleFork".Translate(), delegate
                //{
                //    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Fork;
                //}));
                //riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleOxbow".Translate(), delegate
                //{
                //    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Oxbow;
                //}));
                Find.WindowStack.Add(new FloatMenu(riverStyleList));
            }
            Listing_selRiverStyle.End();

            listing.Gap(listing.verticalSpacing);

            // river beaches
            listing.CheckboxLabeled("ZMD_flagRiverBeach".Translate(), ref settings.flagRiverBeach, "ZMD_flagRiverBeach".Translate());

            if(MapDesignerMod.mod.settings.flagRiverBeach)
            {
                Rect riverBeachRect = listing.GetRect(90f);
                riverBeachRect.xMin += 20f;
                riverBeachRect.xMax -= 20f;
                Listing_Standard riverBeachListing = new Listing_Standard();
                riverBeachListing.Begin(riverBeachRect);

                settings.riverBeachSize = InterfaceUtility.LabeledSlider(riverBeachListing, settings.riverBeachSize, 0f, 35f, "ZMD_riverBeachSize".Translate(), "ZMD_size0".Translate(), "ZMD_size6".Translate());

                List<TerrainDef> shoreOptions = new List<TerrainDef>();

                shoreOptions.Add(TerrainDefOf.Soil);
                shoreOptions.Add(TerrainDef.Named("SoilRich"));
                shoreOptions.Add(TerrainDefOf.Sand);
                shoreOptions.Add(TerrainDef.Named("MarshyTerrain"));
                shoreOptions.Add(TerrainDef.Named("Mud"));
                shoreOptions.Add(TerrainDefOf.Ice);

                Rect selectRect = riverBeachListing.GetRect(30f).LeftHalf();
                Listing_Standard terrainListing = new Listing_Standard();
                terrainListing.Begin(selectRect);

                if (terrainListing.ButtonTextLabeled("ZMD_lakeShore".Translate(), TerrainDef.Named(settings.riverShore).label))
                {
                    List<FloatMenuOption> shoreTerrList = new List<FloatMenuOption>();
                    foreach (TerrainDef terr in shoreOptions)
                    {
                        shoreTerrList.Add(new FloatMenuOption(terr.label, delegate { settings.riverShore = terr.defName; }, MenuOptionPriority.Default));
                    }
                    Find.WindowStack.Add(new FloatMenu(shoreTerrList));
                }
                terrainListing.End();
                riverBeachListing.End();
            }

            // river direction
            listing.CheckboxLabeled("ZMD_flagRiverDir".Translate(), ref MapDesignerMod.mod.settings.flagRiverDir, "ZMD_flagRiverDir".Translate());
            if (MapDesignerMod.mod.settings.flagRiverDir)
            {
                settings.riverDir = InterfaceUtility.AnglePicker(listing, settings.riverDir, "ZMD_Angle".Translate(), 2, true);
                listing.Gap();
            }

            // river location
            listing.CheckboxLabeled("ZMD_riverPosition".Translate(), ref MapDesignerMod.mod.settings.flagRiverLoc, "ZMD_riverPosition".Translate());
            if (MapDesignerMod.mod.settings.flagRiverLoc)
            {
                Rect riverLocRect = listing.GetRect(125f);
                riverLocRect.xMin += 20f;
                riverLocRect.xMax -= 20f;
                Listing_Standard riverLocListing = new Listing_Standard();
                riverLocListing.Begin(riverLocRect);

                Rect locSelRect = riverLocListing.GetRect(100f);

                Rect dotRect = locSelRect;
                dotRect.xMax = dotRect.xMin + locSelRect.height;
                Widgets.DrawBox(dotRect);

                Rect dot = dotRect;
                dot.yMin = 30f - 100f * settings.riverPctSouth;
                dot.xMin = 30f + 100f * settings.riverPctEast;
                dot.height = 40f;
                dot.width = 40f;

                Texture2D dotIcon = ContentFinder<Texture2D>.Get("GUI/ZMD_dot", true);
                Widgets.DrawTextureRotated(dot, dotIcon, 0);


                Rect dotSliderRect = locSelRect;
                dotSliderRect.xMin += locSelRect.height + 20f;

                Listing_Standard locSelListing = new Listing_Standard();
                locSelListing.Begin(dotSliderRect);

                settings.riverPctSouth = InterfaceUtility.LabeledSlider(locSelListing, settings.riverPctSouth, -0.3f, 0.3f, null, "ZMD_south".Translate(), "ZMD_north".Translate());

                settings.riverPctEast = InterfaceUtility.LabeledSlider(locSelListing, settings.riverPctEast, -0.3f, 0.3f, null, "ZMD_west".Translate(), "ZMD_east".Translate());


                locSelListing.End();

                riverLocListing.End();
            }



                // reset
                listing.GapLine();
            if (listing.ButtonText("ZMD_resetRivers".Translate()))
            {
                ResetRiversSettings();
            }

            listing.End();
        }


        public static void ResetRiversSettings()
        {
            settings.sizeRiver = 1.0f;
            MapDesignerMod.mod.settings.flagRiverBeach = false;
            settings.riverShore = "SoilRich";
            settings.riverBeachSize = 10f;
            MapDesignerMod.mod.settings.flagRiverDir = false;
            settings.riverDir = 180f;
            settings.selRiverStyle = MapDesignerSettings.RiverStyle.Vanilla;
            settings.flagRiverLoc = false;
            settings.flagRiverLocAbs = false;
            settings.riverPctSouth = 0.0f;
            settings.riverPctEast = 0.0f;


            //public bool flagRiverLoc = false;
            //public bool flagRiverLocAbs = false;
            //public float riverPctSouth = 0f;
            //public float riverPctEast = 0f;
        }


        public static string GetRiverStyleLabel(MapDesignerSettings.RiverStyle style)
        {
            string label = "ZMD_riverStyle";
            switch (style)
            {
                case MapDesignerSettings.RiverStyle.Vanilla:
                    label = "ZMD_riverStyleVanilla";
                    break;
                case MapDesignerSettings.RiverStyle.Spring:
                    label = "ZMD_riverStyleSpring";
                    break;
                case MapDesignerSettings.RiverStyle.Confluence:
                    label = "ZMD_riverStyleConfluence";
                    break;
                case MapDesignerSettings.RiverStyle.Fork:
                    label = "ZMD_riverStyleFork";
                    break;
                case MapDesignerSettings.RiverStyle.Oxbow:
                    label = "ZMD_riverStyleOxbow";
                    break;
                default:
                    label = "ZMD_riverStyleVanilla";
                    break;
            }
            
            return label;
        }
    }
}
