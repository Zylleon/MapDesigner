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
    public class BetaCardUtility
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawBetaCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.Label("ZMD_betaTabInfo".Translate());

            listing.GapLine();

            settings.terrainFert = InterfaceUtility.LabeledSlider(listing, settings.terrainFert, 0.25f, 2f, "ZMD_terrainFertAmt".Translate() + settings.terrainFert);

            // reset
            listing.GapLine();
            if (listing.ButtonText("ZMD_reset".Translate()))
            {
                ResetBetaSettings();
            }

            listing.End();
        }


        public static void ResetBetaSettings()
        {
            settings.terrainFert = 1f;
            
        }
    }


}