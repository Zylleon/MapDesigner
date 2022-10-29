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
    public static class RiversCard
    {
        public static MapDesignerSettings settings = MapDesignerMod.mod.settings;

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawRiversCard(Rect rect)
        {
            bool prevChanged = GUI.changed;
            GUI.changed = false;

            // setting up the scrollbar
            Rect rect2 = rect.ContractedBy(4f);

            // initial height with everything collapsed
            float height = 400f;
            if (settings.flagRiverBeach)
            {
                height += 100f;
            }
            if (settings.flagRiverDir)
            {
                height += 70f;
            }
            if (settings.flagRiverLoc)
            {
                height += 150f;
            }

            Rect viewRect = new Rect(0f, 0f, rect2.width - 18f, Math.Max(height, viewHeight));
            Widgets.BeginScrollView(rect2, ref scrollPosition, viewRect, true);


            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);
            listing.GapLine();

            // River size and style
            settings.sizeRiver = InterfaceUtility.LabeledSlider(listing, settings.sizeRiver, 0.1f, 3f, InterfaceUtility.FormatLabel("ZMD_sizeRiver", "ZMD_size" + ThingsCard.GetDensityLabel(settings.sizeRiver)), "ZMD_size1".Translate(), "ZMD_size6".Translate());
            Rect selStyleRect = listing.GetRect(40f);

            Rect selButtonRect = selStyleRect;
            Rect descRect = selStyleRect;
            selButtonRect.xMax -= 0.66f * viewRect.width;
            descRect.xMin += 0.34f * viewRect.width;
            Rect iconRect = new Rect(descRect);
            iconRect.xMin += 10f;
            iconRect.xMax = iconRect.xMin + 40f;

            string texPath = String.Format("GUI/{0}", GetRiverStyleLabel(settings.selRiverStyle));
            Texture2D icon = ContentFinder<Texture2D>.Get(texPath, true);
            Widgets.DrawTextureRotated(iconRect, icon, 0);
            descRect.xMin += 60f;

            Widgets.Label(descRect, (GetRiverStyleLabel(settings.selRiverStyle) + "Desc").Translate());
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
                riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleCanal".Translate(), delegate
                {
                    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Canal;
                }));

                riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleConfluence".Translate(), delegate
                {
                    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Confluence;
                }));
                riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleFork".Translate(), delegate
                {
                    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Fork;
                }));
                riverStyleList.Add(new FloatMenuOption("ZMD_riverStyleOxbow".Translate(), delegate
                {
                    settings.selRiverStyle = MapDesignerSettings.RiverStyle.Oxbow;
                }));
                Find.WindowStack.Add(new FloatMenu(riverStyleList));
            }
            Listing_selRiverStyle.End();

            listing.Gap(listing.verticalSpacing);

            // river banks
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
                if(settings.flagRiverLocAbs)
                {
                    listing.CheckboxLabeled(String.Format("{0}: {1}", "ZMD_riverLocAbs".Translate(), "ZMD_riverLocAbsDesc".Translate()), ref MapDesignerMod.mod.settings.flagRiverLocAbs, "ZMD_riverLocAbsDesc".Translate());
                }
                else
                {
                    listing.CheckboxLabeled(String.Format("{0}: {1}", "ZMD_riverLocRel".Translate(), "ZMD_riverLocRelDesc".Translate()), ref MapDesignerMod.mod.settings.flagRiverLocAbs, "ZMD_riverLocRelDesc".Translate());

                }

                InterfaceUtility.LocationPicker(listing, 0.3f, ref settings.riverCenterDisp, 40f);
            }



            // Beaches
            listing.GapLine();

            Rect selCoastDir = listing.GetRect(35f);
            selCoastDir.xMax -= 0.66f * viewRect.width;
            Listing_Standard Listing_selCoastDir = new Listing_Standard();
            Listing_selCoastDir.Begin(selCoastDir);

            // beach direction
            if (Listing_selCoastDir.ButtonTextLabeled("ZMD_coastDir".Translate(), GetCoastDirLabel(settings.coastDir).Translate()))
            {
                List<FloatMenuOption> coastDirList = new List<FloatMenuOption>();

                coastDirList.Add(new FloatMenuOption("ZMD_coastDirVanilla".Translate(), delegate
                {
                    settings.coastDir = MapDesignerSettings.CoastDirection.Vanilla;
                }));
                coastDirList.Add(new FloatMenuOption("ZMD_north".Translate(), delegate
                {
                    settings.coastDir = MapDesignerSettings.CoastDirection.North;
                }));
                coastDirList.Add(new FloatMenuOption("ZMD_east".Translate(), delegate
                {
                    settings.coastDir = MapDesignerSettings.CoastDirection.East;
                }));
                coastDirList.Add(new FloatMenuOption("ZMD_south".Translate(), delegate
                {
                    settings.coastDir = MapDesignerSettings.CoastDirection.South;
                }));
                coastDirList.Add(new FloatMenuOption("ZMD_west".Translate(), delegate
                {
                    settings.coastDir = MapDesignerSettings.CoastDirection.West;
                }));

                Find.WindowStack.Add(new FloatMenu(coastDirList));
            }
            Listing_selCoastDir.End();

            // beach terrain
            Rect beachTerrRect = listing.GetRect(35f);
            beachTerrRect.xMax -= 0.66f * viewRect.width;

            //beachTerrRect.xMin += 20f;
            //beachTerrRect.xMax -= 20f;
            //Listing_Standard riverBeachListing = new Listing_Standard();
            //riverBeachListing.Begin(beachTerrRect);

            List<string> beachTerrOptions = new List<string>();

            beachTerrOptions.Add("Sand");
            beachTerrOptions.Add("Soil");
            beachTerrOptions.Add("SoilRich");
            beachTerrOptions.Add("MarshyTerrain");
            beachTerrOptions.Add("Mud");
            beachTerrOptions.Add("Ice");

            Listing_Standard beachTerrListing = new Listing_Standard();
            beachTerrListing.Begin(beachTerrRect);

            if (beachTerrListing.ButtonTextLabeled("ZMD_coastTerr".Translate(), settings.beachTerr == "Vanilla" ? "ZMD_coastTerrVanilla".Translate().ToString() : TerrainDef.Named(settings.beachTerr).label))
            {
                List<FloatMenuOption> beachTerrList = new List<FloatMenuOption>();
                beachTerrList.Add(new FloatMenuOption("ZMD_coastTerrVanilla".Translate(), delegate { settings.beachTerr = "Vanilla"; }, MenuOptionPriority.Default));

                foreach (string terr in beachTerrOptions)
                {
                    beachTerrList.Add(new FloatMenuOption(TerrainDef.Named(terr).label, delegate { settings.beachTerr = terr; }, MenuOptionPriority.Default));
                }
                Find.WindowStack.Add(new FloatMenu(beachTerrList));
            }
            beachTerrListing.End();



            // reset
            listing.GapLine();

            if (listing.ButtonText("ZMD_resetRivers".Translate()))
            {
                ResetRiversSettings();
            }

            listing.End();

            viewHeight = listing.CurHeight;
            Widgets.EndScrollView();
            if (GUI.changed)
            {
                HelperMethods.InvokeOnSettingsChanged();
            }
            GUI.changed = GUI.changed || prevChanged;
        }


        public static void ResetRiversSettings()
        {
            //Rivers
            settings.sizeRiver = 1.0f;
            MapDesignerMod.mod.settings.flagRiverBeach = false;
            settings.riverShore = "SoilRich";
            settings.riverBeachSize = 10f;
            MapDesignerMod.mod.settings.flagRiverDir = false;
            settings.riverDir = 180f;
            settings.selRiverStyle = MapDesignerSettings.RiverStyle.Vanilla;
            settings.flagRiverLoc = false;
            settings.flagRiverLocAbs = false;
            settings.riverCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);

            //Beaches
            settings.coastDir = MapDesignerSettings.CoastDirection.Vanilla;
            settings.beachTerr = "Vanilla";

            HelperMethods.InvokeOnSettingsChanged();
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
                case MapDesignerSettings.RiverStyle.Canal:
                    label = "ZMD_riverStyleCanal";
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

        public static string GetCoastDirLabel(MapDesignerSettings.CoastDirection dir)
        {
            string label = "ZMD_coastDirVanilla";
            switch (dir)
            {
                case MapDesignerSettings.CoastDirection.North:
                    label = "ZMD_north";
                    break;
                case MapDesignerSettings.CoastDirection.East:
                    label = "ZMD_east";
                    break;
                case MapDesignerSettings.CoastDirection.South:
                    label = "ZMD_south";
                    break;
                case MapDesignerSettings.CoastDirection.West:
                    label = "ZMD_west";
                    break;
            }
            return label;
        }

    }
}
