using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.Noise;

namespace MapDesigner.Feature
{
    public class Lake : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }

        private float lakeSize = 0.40f;
        private float lakeBeachSize = 10f;
        private float lakeRoundness = 1.5f;

        public override void Generate(Map map, GenStepParams parms)
        {
            MapDesignerSettings settings = MapDesignerMod.mod.settings;

            lakeSize = settings.lakeSize * map.Size.x / 2;
            lakeRoundness = settings.lakeRoundness;
            lakeBeachSize = settings.lakeBeachSize;
            if (map.Biome.defName.Contains("BiomesIsland"))
            {
                lakeSize *= 0.5f;
            }

            MapGenFloatGrid lakeGrid = MapGenerator.FloatGridNamed("ZMD_Lake");
            IntVec3 mapCenter = map.Center;
            ModuleBase moduleBase = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            foreach (IntVec3 current in map.AllCells)
            {
                float distance = HelperMethods.DistanceBetweenPoints(current, mapCenter);
                lakeGrid[current] = 0f + lakeRoundness * moduleBase.GetValue(current) + 0.1f * (lakeSize - distance);
            }

            float deepBelow = lakeGrid[mapCenter] * (1 -settings.lakeDepth);
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            float deepWater = -2005;
            float shallowWater = -1025;
            if (settings.flagLakeSalty)
            {
                deepWater = -2015;
                shallowWater = -1035;
            }
            float beachValue = Feature_TerrainFrom.ValueFromTerrain(settings.lakeShore);

            foreach (IntVec3 current in map.AllCells)
            {
                if (elevation[current] < 0.65f)         // leaves mountains & most surrounding gravel untouched
                {
                    if (lakeGrid[current] > deepBelow)
                    {
                        fertility[current] = deepWater;
                    }
                    else
                    {
                        if (lakeGrid[current] > 0f)
                        {
                            fertility[current] = shallowWater;
                        }
                        else if (lakeGrid[current] > 0f - 0.1f * lakeBeachSize)
                        {
                            fertility[current] = beachValue;
                        }
                    }
                }
            }

        }
    }
}
