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
        // Mountains
        public float hillAmount = 1.0f;
        public float hillSize = 0.021f;
        public float hillSmoothness = 2.0f;
        public static bool flagHillClumping = false;
        public static bool flagCaves = true;

        // Things
        public float densityPlant = 1.0f;
        public float densityAnimal = 1.0f;
        public float densityRuins = 1.0f;
        //public float densityDanger = 1.0f;
        public float densityGeyser = 1.0f;
        public float densityOre = 1.0f;

        public float sizeRiver = 1.0f;

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
        //public static bool priMarshyBeaches = false;



        // Helper stuff
        public Dictionary<string, BiomeDefault> biomeDefaults;
        public Dictionary<string, FloatRange> densityDefaults;


        public override void ExposeData()
        {
            Scribe_Values.Look(ref hillAmount, "hillAmount", 1.0f);
            Scribe_Values.Look(ref hillSize, "hillSize", 0.021f);
            Scribe_Values.Look(ref hillSmoothness, "hillSmoothness", 2.0f);

            Scribe_Values.Look(ref flagHillClumping, "flagHillClumping", false);
            Scribe_Values.Look(ref flagCaves, "flagCaves", true);

            Scribe_Values.Look(ref densityPlant, "densityPlant", 1.0f);
            Scribe_Values.Look(ref densityAnimal, "densityAnimal", 1.0f);
            Scribe_Values.Look(ref densityRuins, "densityRuins", 1.0f);
            //Scribe_Values.Look(ref densityDanger, "densityDanger", 1.0f);
            Scribe_Values.Look(ref densityGeyser, "densityGeyser", 1.0f);
            Scribe_Values.Look(ref densityOre, "densityOre", 1.0f);

            Scribe_Values.Look(ref sizeRiver, "sizeRiver", 1.0f);

            Scribe_Values.Look(ref selectedFeature, "selectedFeature", Features.None);


            // Features
            // Round islands
            Scribe_Values.Look(ref priIslandSize, "priIslandSize", 40f);
            Scribe_Values.Look(ref priBeachSize, "priBeachSize", 5f);
            Scribe_Values.Look(ref priMultiSpawn, "priMultiSpawn", false);
            //Scribe_Values.Look(ref priMarshyBeaches, "priMarshyBeaches", false);




        }
    }

}
