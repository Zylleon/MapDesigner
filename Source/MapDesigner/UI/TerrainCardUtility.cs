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
    public class TerrainCardUtility
    {
        public static MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawTerrainCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.Label("ZMD_betaTabInfo".Translate());

            listing.GapLine();

            //settings.terrainWater = InterfaceUtility.LabeledSlider(listing, settings.terrainWater, 0.25f, 2f, "ZMD_terrainWaterAmt".Translate() + settings.terrainWater);
            settings.terrainFert = InterfaceUtility.LabeledSlider(listing, settings.terrainFert, 0.25f, 2f, String.Format("ZMD_terrainFertAmt".Translate(), Math.Round(100 * settings.terrainFert)));

            settings.terrainWater = InterfaceUtility.LabeledSlider(listing, settings.terrainWater, 0.25f, 2f, String.Format("ZMD_terrainWaterAmt".Translate(), Math.Round(100 * settings.terrainWater)), null, null, "ZMD_terrainWaterTooltip".Translate());
            listing.CheckboxLabeled("ZMD_flagTerrainWater".Translate(), ref MapDesignerSettings.flagCaves, "ZMD_flagTerrainWater".Translate());

            // Rocks
            listing.GapLine();
            InterfaceUtility.LabeledIntRange(listing, ref settings.rockTypeRange, 1, 5, "ZMD_rockTypeRange".Translate());

            listing.CheckboxLabeled("ZMD_flagBiomeRocks".Translate(), ref MapDesignerSettings.flagBiomeRocks, "ZMD_flagBiomeRocksTooltip".Translate());

            if (listing.ButtonText("ZMD_chooseRockTypes".Translate()))
            {
                Find.WindowStack.Add(new RockSelectionDialog());
            }

            // reset
            listing.GapLine();
            if (listing.ButtonText("ZMD_resetTerrain".Translate()))
            {
                ResetTerrainSettings();
            }

            listing.End();
        }


        public static void ResetTerrainSettings()
        {
            settings.terrainFert = 1f;
            settings.terrainWater = 1f;
            settings.rockTypeRange = new IntRange(2, 3);
            MapDesignerSettings.flagBiomeRocks = false;
        }
    }


}