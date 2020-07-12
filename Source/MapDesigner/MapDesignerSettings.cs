using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;


namespace MapDesigner
{

    public class MapDesignerSettings : ModSettings
    {
        // General
        public static bool flagPopup = true;

        // Mountains
        public float hillAmount = 1.0f;
        public float hillSize = 0.021f;
        public float hillSmoothness = 2.0f;
        public static bool flagCaves = true;

        public static bool flagHillClumping = false;

        public static bool flagHillRadial = false;
        public float hillRadialAmt = 1.0f;
        public float hillRadialSize = 0.65f;

        public static bool flagHillSplit = false;
        public float hillSplitAmt = 1.5f;
        public float hillSplitDir = 90f;
        public float hillSplitSize = 0.35f;

        public static bool flagHillSide = false;
        public float hillSideAmt = 1.0f;
        public float hillSideDir = 180f;

        // Things
        public float densityPlant = 1.0f;
        public float densityAnimal = 1.0f;
        public float densityRuins = 1.0f;
        public float densityDanger = 1.0f;

        public float densityGeyser = 1.0f;
        public float densityOre = 1.0f;
        public float animaCount = 1.0f;

        // Rocks
        public IntRange rockTypeRange = new IntRange(2, 3);
        public static bool flagBiomeRocks = false;
        public Dictionary<string, bool> allowedRocks = new Dictionary<string, bool>();


        // Rivers
        public float sizeRiver = 1.0f;
        public static bool flagRiverBeach = false;
        public string riverShore = "SoilRich";
        public float riverBeachSize = 10f;                // in tiles

        // Terrain
        public float terrainFert = 1f;
        public float terrainWater = 1f;
        public static bool flagTerrainWater = false;

        // Features
        public enum Features : byte
        {
            None,
            RoundIsland,
            Lake
        }

        public MapDesignerSettings.Features selectedFeature = Features.None;

        // Perfectly Round Islands
        public float priIslandSize = 40f;
        public float priBeachSize = 5f;
        public static bool priMultiSpawn = false;

        // Lake
        public float lakeSize = 0.20f;                  // proportion of map size
        public float lakeBeachSize = 10f;                // in tiles
        public float lakeRoundness = 1.5f;              // 0 = perfectly round
        public float lakeDepth = 0.5f;                  // proportion of deep water
        public static bool flagLakeSalty = false;
        public string lakeShore = "Sand";

        // Helper stuff
        public Dictionary<string, BiomeDefault> biomeDefaults;
        public Dictionary<string, FloatRange> densityDefaults;


        public override void ExposeData()
        {
            // General
            Scribe_Values.Look(ref flagPopup, "flagPopup", true);

            // Mountains
            Scribe_Values.Look(ref hillAmount, "hillAmount", 1.0f);
            Scribe_Values.Look(ref hillSize, "hillSize", 0.021f);
            Scribe_Values.Look(ref hillSmoothness, "hillSmoothness", 2.0f);
            Scribe_Values.Look(ref flagCaves, "flagCaves", true);

            Scribe_Values.Look(ref flagHillClumping, "flagHillClumping", false);

            Scribe_Values.Look(ref flagHillRadial, "flagHillRadial", false);
            Scribe_Values.Look(ref hillRadialAmt, "hillRadialAmt", 0.0f);
            Scribe_Values.Look(ref hillRadialSize, "hillRadialSize", 0.65f);

            Scribe_Values.Look(ref flagHillSplit, "flagHillSplit", false);
            Scribe_Values.Look(ref hillSplitAmt, "hillSplitAmt", 1.5f);
            Scribe_Values.Look(ref hillSplitDir, "hillSplitDir", 90f);
            Scribe_Values.Look(ref hillSplitSize, "hillSplitSize", 0.35f);

            Scribe_Values.Look(ref flagHillSide, "flagHillSide", false);
            Scribe_Values.Look(ref hillSideAmt, "hillSideAmt", 1.0f);
            Scribe_Values.Look(ref hillSideDir, "hillSideDir", 180f);


            // Things
            Scribe_Values.Look(ref densityPlant, "densityPlant", 1.0f);
            Scribe_Values.Look(ref densityAnimal, "densityAnimal", 1.0f);
            Scribe_Values.Look(ref densityRuins, "densityRuins", 1.0f);
            Scribe_Values.Look(ref densityDanger, "densityDanger", 1.0f);
            Scribe_Values.Look(ref densityGeyser, "densityGeyser", 1.0f);
            Scribe_Values.Look(ref densityOre, "densityOre", 1.0f);
            Scribe_Values.Look(ref animaCount, "animaCount", 1.0f);

            // Rocks
            Scribe_Values.Look(ref rockTypeRange, "rockTypeRange", new IntRange(2, 3));
            Scribe_Values.Look(ref flagBiomeRocks, "flagBiomeRocks", false);
            Scribe_Collections.Look(ref allowedRocks, "allowedRocks", LookMode.Value, LookMode.Value);

            // Rivers
            Scribe_Values.Look(ref sizeRiver, "sizeRiver", 1.0f);
            Scribe_Values.Look(ref flagRiverBeach, "flagFertileRivers", false);
            Scribe_Values.Look(ref riverShore, "riverShore", "SoilRich");
            Scribe_Values.Look(ref riverBeachSize, "riverBeachSize", 10f);


            // Terrain
            Scribe_Values.Look(ref terrainFert, "terrainFert", 1.0f);
            Scribe_Values.Look(ref terrainWater, "terrainWater", 1.0f);
            Scribe_Values.Look(ref flagTerrainWater, "flagTerrainWater", false);


            // Features
            Scribe_Values.Look(ref selectedFeature, "selectedFeature", Features.None);

            // Round islands
            Scribe_Values.Look(ref priIslandSize, "priIslandSize", 40f);
            Scribe_Values.Look(ref priBeachSize, "priBeachSize", 5f);
            Scribe_Values.Look(ref priMultiSpawn, "priMultiSpawn", false);

            // Lake
            Scribe_Values.Look(ref lakeSize, "lakeSize", 0.20f);
            Scribe_Values.Look(ref lakeBeachSize, "lakeBeachSize", 10f);
            Scribe_Values.Look(ref lakeRoundness, "lakeRoundness", 1.5f);
            Scribe_Values.Look(ref lakeDepth, "lakeDepth", 0.5f);
            Scribe_Values.Look(ref flagLakeSalty, "flagLakeSalty", false);
            Scribe_Values.Look(ref lakeShore, "lakeShore", "Sand");



        }
    }

}
