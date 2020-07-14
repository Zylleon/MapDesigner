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
    public static class RiversCardUtility
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawRiversCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.GapLine();

            settings.sizeRiver = InterfaceUtility.LabeledSlider(listing, settings.sizeRiver, 0.1f, 3f, InterfaceUtility.FormatLabel("ZMD_sizeRiver", "ZMD_size" + ThingsCardUtility.GetDensityLabel(settings.sizeRiver)), "ZMD_size1".Translate(), "ZMD_size6".Translate());
            listing.Gap(listing.verticalSpacing);

            // river beaches
            listing.CheckboxLabeled("ZMD_flagRiverBeach".Translate(), ref MapDesignerSettings.flagRiverBeach, "ZMD_flagRiverBeach".Translate());

            if(MapDesignerSettings.flagRiverBeach)
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

            listing.CheckboxLabeled("ZMD_flagRiverDir".Translate(), ref MapDesignerSettings.flagRiverDir, "ZMD_flagRiverDir".Translate());
            if (MapDesignerSettings.flagRiverDir)
            {
                settings.riverDir = InterfaceUtility.AnglePicker(listing, settings.riverDir, "ZMD_Angle".Translate(), 2, true);
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
            MapDesignerSettings.flagRiverBeach = false;
            settings.riverShore = "SoilRich";
            settings.riverBeachSize = 10f;
            MapDesignerSettings.flagRiverDir = false;
            settings.riverDir = 180f;
        }

    }
}
