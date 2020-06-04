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
    public class ZMD_GenStep_Lake : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791; 
            }
        }

        private int beachSize = 20;
        private int lakeSize = 15;
        private int lakeRoundness = 30;        // 0 is round, 30 is very not round
        private int roughness = 15;            // o is smooth

        public override void Generate(Map map, GenStepParams parms)
        {
            //settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<ZMD_LakeSettings>();
            MapGenFloatGrid lakeGrid = MapGenerator.FloatGridNamed("ZMD_Lake");

            //main circle
            IntVec3 mapCenter = map.Center;
            //lakeSize = map.Size.z / 10;

            List<IntVec3> bigCircle = new List<IntVec3>();
            foreach (IntVec3 current in map.AllCells)
            {
                if (HelperMethods.DistanceBetweenPoints(mapCenter, current) < lakeSize)
                {
                    bigCircle.Add(current);
                }
            }


            //make lake squiggly
            List<IntVec3> lake = new List<IntVec3>();

            float freq = Rand.Range(0.01f, 0.04f);
            ModuleBase lakeSquiggleX = new Perlin(freq, 0.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Low);
            ModuleBase lakeSquiggleZ = new Perlin(freq, 0.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Low);

            foreach(IntVec3 tile in bigCircle)
            {
                if(Rand.Chance(0.1f))
                {
                    IntVec3 newCenter = tile;
                    newCenter.x += (int)(lakeRoundness * lakeSquiggleX.GetValue(tile));
                    newCenter.z += (int)(lakeRoundness * lakeSquiggleZ.GetValue(tile));

                    //lake.AddRange(GenRadial.RadialCellsAround(newCenter, lakeRoundness + 2, true));
                    lake.AddRange(GenRadial.RadialCellsAround(newCenter, 2 + lakeRoundness / 2, true).Except(lake));
                }
            }

            //RoughenShape(lake, "ZMD_Lake", map);
            lakeGrid = RoughenShape(lake, lakeGrid, map);

            Log.Message(lake.Count + " lake tiles");

            List<IntVec3> shore = new List<IntVec3>();

            foreach (IntVec3 tile in lake)
            {
                if (Rand.Chance(0.1f))
                {
                    //shore.AddRange(GenRadial.RadialCellsAround(tile, beachSize, true));
                    shore.AddRange(GenRadial.RadialCellsAround(tile, lakeRoundness + 2, true).Except(shore));
                }
            }

            MapGenFloatGrid shoreGrid = MapGenerator.FloatGridNamed("ZMD_LakeShore");

            //RoughenShape(lake, "ZMD_LakeShore", map);
            shoreGrid = RoughenShape(lake, shoreGrid, map);
            shoreGrid = RoughenShape(lake, shoreGrid, map);

            //foreach (IntVec3 tile in bigCircle)
            //{

            //    if(Rand.Chance(0.5f))
            //    {
            //        float radius = lakeRoundness * (float)(1 + Math.Pow(lakeSquiggle.GetValue(tile), 1)) + Rand.Range(1, lakeSize / 2);
            //        if (Rand.Bool)
            //        {
            //            radius *= 3;
            //        }

            //        List<IntVec3> circle = new List<IntVec3>();
            //        circle = GenRadial.RadialCellsAround(tile, Math.Min(56, radius), true).ToList();

            //        foreach (IntVec3 t in circle)
            //        {
            //            if (t.InBounds(map))
            //            {
            //                lakeGrid[t] += 0.01f;
            //            }
            //        }
            //    }
            //}

            TerrainGrid terrainGrid = map.terrainGrid;
            TerrainDef shoreOuterTerr = TerrainDefOf.Soil;
            TerrainDef shoreInnerTerr = TerrainDef.Named("SoilRich");
            TerrainDef shallowTerr = TerrainDefOf.WaterShallow;
            TerrainDef deepTerr = TerrainDefOf.WaterDeep;




            // apply lake terrains
            foreach (IntVec3 current in map.AllCells)
            {
                if (lakeGrid[current] > 0.05f)
                {
                    if (!terrainGrid.TerrainAt(current).IsWater)
                    {
                        terrainGrid.SetTerrain(current, shallowTerr);
                    }
                }

                if (lakeGrid[current] > 0.2f)
                {
                    terrainGrid.SetTerrain(current, deepTerr);

                }
            }

            // beach terrains

            foreach (IntVec3 current in map.AllCells)
            {
                if (!terrainGrid.TerrainAt(current).IsWater)
                {
                    if (shoreGrid[current] > 0.05f)
                    {
                        terrainGrid.SetTerrain(current, shoreOuterTerr);
                    }
                }

            }

            //foreach(IntVec3 c in shore)
            //{
            //    if (c.InBounds(map))
            //    {
            //        terrainGrid.SetTerrain(c, TerrainDefOf.Ice);
            //    }
            //}

            //foreach (IntVec3 c in lake)
            //{
            //    if (c.InBounds(map))
            //    {
            //        terrainGrid.SetTerrain(c, TerrainDefOf.Bridge);
            //    }
            //}
        }



        private MapGenFloatGrid RoughenShape(List<IntVec3> shape, /*string gridName,*/MapGenFloatGrid grid, Map map)
        {
            Log.Message("Roughening shape");
            //MapGenFloatGrid grid = MapGenerator.FloatGridNamed("gridName");

            float incr = 0.001f * roughness;

            foreach(IntVec3 tile in shape)
            {
                if (Rand.Chance(1f / (roughness + 1.0f)))
                {
                    float radius = Rand.Range(1, roughness + 2);
                    if (Rand.Chance(0.3f))
                    {
                        radius *= 3;
                    }
                    List<IntVec3> circle = GenRadial.RadialCellsAround(tile, Math.Min(radius, 56), true).ToList();
                    foreach(IntVec3 t in circle)
                    {
                        if (t.InBounds(map))
                        {
                            grid[t] += incr;
                        }
                    }
                }
            }

            return grid;
        }


    }
}
