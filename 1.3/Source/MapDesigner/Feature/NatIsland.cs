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
    public class NatIsland : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1492710749;
            }
        }

        private float niSize = 0.40f;
        private float niBeachSize = 10f;
        private float niRoundness = 1.5f;

        public override void Generate(Map map, GenStepParams parms)
        {
            MapDesignerSettings settings = MapDesignerMod.mod.settings;

            niSize = settings.niSize * map.Size.x / 2;
            if(settings.niStyle == MapDesignerSettings.NiStyle.Ring || settings.niStyle == MapDesignerSettings.NiStyle.SquareRing)
            {
                // this is because rings are -centered- on the island diameter
                niSize *= .78f;
            }
            niRoundness = settings.niRoundness;
            niBeachSize = settings.niBeachSize;

            MapGenFloatGrid niGrid = MapGenerator.FloatGridNamed("ZMD_NatIsland");
            IntVec3 mapCenter = map.Center;

            mapCenter.x += (int)(MapDesignerMod.mod.settings.niCenterDisp.x * map.Size.x);
            mapCenter.z += (int)(MapDesignerMod.mod.settings.niCenterDisp.z * map.Size.z);

            ModuleBase noiseModule = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            

            if (settings.niStyle == MapDesignerSettings.NiStyle.Square)
            {
                foreach (IntVec3 current in map.AllCells)
                {
                    float distance = Math.Max(Math.Abs(current.z - mapCenter.z), Math.Abs(current.x - mapCenter.x));
                    niGrid[current] = 0f + 10 * niRoundness * noiseModule.GetValue(current) + distance;
                }
            }

            else if (settings.niStyle == MapDesignerSettings.NiStyle.Round)
            {
                foreach (IntVec3 current in map.AllCells)
                {
                    float distance = HelperMethods.DistanceBetweenPoints(current, mapCenter);
                    niGrid[current] = 0f + 10 * niRoundness * noiseModule.GetValue(current) + distance;
                }
            }

            else if (settings.niStyle == MapDesignerSettings.NiStyle.Ring)
            {
                foreach (IntVec3 current in map.AllCells)
                {
                    float distance = HelperMethods.DistanceBetweenPoints(current, mapCenter);

                    distance -= niSize;
                    distance *= 3.5f;
                    distance = Math.Abs(distance);
                    niGrid[current] = 0f + 30 * niRoundness * noiseModule.GetValue(current) + distance;
                }
            }

            else if (settings.niStyle == MapDesignerSettings.NiStyle.SquareRing)
            {
                foreach (IntVec3 current in map.AllCells)
                {
                    float distance = Math.Max(Math.Abs(current.z - mapCenter.z), Math.Abs(current.x - mapCenter.x));

                    distance -= niSize;
                    distance *= 3.5f;
                    distance = Math.Abs(distance);
                    niGrid[current] = 0f + 30 * niRoundness * noiseModule.GetValue(current) + distance;
                }
            }



            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            float deepWater = -2005;
            float shallowWater = -2025;
            float shoreline = -1025;
            if (settings.flagNiSalty)
            {
                deepWater = -2015;
                shallowWater = -2035;
                shoreline = -1035;
            }
            float beachValue = Feature_TerrainFrom.ValueFromTerrain(settings.niShore);
            if (settings.niStyle == MapDesignerSettings.NiStyle.Ring || settings.niStyle == MapDesignerSettings.NiStyle.SquareRing)
            {
                niBeachSize *= 2f;
            }

            foreach (IntVec3 current in map.AllCells)
            {
                // removes mountains from water
                if(niGrid[current] > niSize)
                {
                    elevation[current] = 0f;
                }

                if (elevation[current] < 0.65f)         // leaves mountains & most surrounding gravel untouched
                {
                    if (niGrid[current] < niSize && niGrid[current] >= niSize - niBeachSize)
                    {
                        fertility[current] = beachValue;
                    }
                    else if (niGrid[current] > niSize)
                    {
                        if (niGrid[current] > niSize + 22 && fertility[current] <= settings.niWaterDepth)
                        {
                            fertility[current] = deepWater;
                        }
                        else if (niGrid[current] <= niSize + 22)
                        {
                            fertility[current] = shoreline;
                        }

                        else
                        {
                            fertility[current] = shallowWater;
                        }
                    }
                }
            }

        }
    }
}
