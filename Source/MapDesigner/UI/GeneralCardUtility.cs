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
    public class GeneralCardUtility
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawGeneralCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.Label("ZMD_generalTabInfo".Translate());

            listing.GapLine();

            //listing.CheckboxLabeled("Enable terrain", ref MapDesignerSettings.flagTerrain, "Enable terrain");

            //settings.terrainFert = InterfaceUtility.LabeledSlider(listing, settings.terrainFert, 0.25f, 3f, "Terrain: " + settings.terrainFert);


            // reset
            listing.GapLine();
            if (listing.ButtonText("ZMD_reset".Translate()))
            {
                ResetAllSettings();
            }

            listing.End();
        }


        public static void ResetAllSettings()
        {
            settings.terrainFert = 1f;

            //settings.selectedFeature = MapDesignerSettings.Features.None;
            //MountainCardUtility.ResetMountainSettings();
            //RiversCardUtility.ResetRiversSettings();
            //RocksCardUtility.ResetRocksSettings();
            //ThingsCardUtility.ResetThingsSettings();
        }
    }


}