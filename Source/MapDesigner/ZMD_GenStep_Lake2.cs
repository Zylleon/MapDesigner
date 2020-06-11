using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.Noise;

namespace MapDesigner
{
    public class ZMD_GenStep_Lake2 : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }

        private float lakeSize = 0.40f;
        private float lakeBeachSize = 5f;
        private float lakeRoughness = 2.5f;

        public override void Generate(Map map, GenStepParams parms)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

            lakeSize = settings.lakeSize * map.Size.x / 2;
            lakeRoughness = settings.lakeRoughness;
            lakeBeachSize = settings.lakeBeachSize;

            MapGenFloatGrid lakeGrid = MapGenerator.FloatGridNamed("ZMD_Lake");
            IntVec3 mapCenter = map.Center;
            ModuleBase moduleBase = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            foreach (IntVec3 current in map.AllCells)
            {
                float distance = HelperMethods.DistanceBetweenPoints(current, mapCenter);
                lakeGrid[current] = 0f + lakeRoughness * moduleBase.GetValue(current) + 0.1f * (lakeSize - distance);
            }

            TerrainGrid terrainGrid = map.terrainGrid;
            TerrainDef terrDeep = TerrainDefOf.WaterDeep;
            TerrainDef terrShallow = TerrainDefOf.WaterShallow;
            TerrainDef terrShore = TerrainDef.Named(settings.lakeShore);
            //TerrainDef terrShore = TerrainDefOf.Sand;

            if (MapDesignerSettings.flagLakeSalty)
            {
                terrDeep = TerrainDefOf.WaterOceanDeep;
                terrShallow = TerrainDefOf.WaterOceanShallow;
            }

            float deepBelow = lakeGrid[mapCenter] * (1 -settings.lakeDepth);


            foreach (IntVec3 current in map.AllCells)
            {
                if (lakeGrid[current] > deepBelow)
                {
                    terrainGrid.SetTerrain(current, terrDeep);
                }
                else if (!terrainGrid.TerrainAt(current).IsRiver)
                {
                    if (lakeGrid[current] > 0f)
                    {
                        terrainGrid.SetTerrain(current, terrShallow);
                    }
                    else if (lakeGrid[current] > 0f - 0.1f * lakeBeachSize)
                    {
                        terrainGrid.SetTerrain(current, terrShore);
                    }
                }
               
            }

        }
    }
}
