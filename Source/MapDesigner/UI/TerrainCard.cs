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
    public class TerrainCard
    {
        public static MapDesignerSettings settings = MapDesignerMod.mod.settings;

        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public static void DrawTerrainCard(Rect rect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            listing.Label("ZMD_terrainTabInfo".Translate());

            listing.GapLine();

            settings.terrainFert = InterfaceUtility.LabeledSlider(listing, settings.terrainFert, 0.30f, 2f, String.Format("ZMD_terrainFertAmt".Translate(), Math.Round(100 * settings.terrainFert)));

            settings.terrainWater = InterfaceUtility.LabeledSlider(listing, settings.terrainWater, 0.30f, 2f, String.Format("ZMD_terrainWaterAmt".Translate(), Math.Round(100 * settings.terrainWater)), null, null, null, "ZMD_terrainWaterTooltip".Translate());
            listing.CheckboxLabeled("ZMD_flagTerrainWater".Translate(), ref settings.flagCaves, "ZMD_flagTerrainWater".Translate());

            // Rocks
            listing.GapLine();

            List<ThingDef> rockList = HelperMethods.GetRockList();
            int maxRocks = Math.Min(15, rockList.Count);

            //InterfaceUtility.LabeledIntRange(listing, ref settings.rockTypeRange, 1, 5, "ZMD_rockTypeRange".Translate());
            InterfaceUtility.LabeledIntRange(listing, ref settings.rockTypeRange, 1, maxRocks, "ZMD_rockTypeRange".Translate());

            listing.CheckboxLabeled("ZMD_flagBiomeRocks".Translate(), ref settings.flagBiomeRocks, "ZMD_flagBiomeRocksTooltip".Translate());

            if (InterfaceUtility.SizedTextButton(listing, "ZMD_chooseRockTypes".Translate()))
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
            settings.flagBiomeRocks = false;

            List<ThingDef> list = HelperMethods.GetRockList();
            Dictionary<string, bool> newRocks = new Dictionary<string, bool>();
            foreach (ThingDef rock in list)
            {
                newRocks.Add(rock.defName, true);
            }
            settings.allowedRocks = newRocks;
        }
    }


}