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
        private int roughness = 10;            // o is smooth

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
                IntVec3 newCenter = tile;
                newCenter.x += (int)(lakeRoundness * lakeSquiggleX.GetValue(tile));
                newCenter.z += (int)(lakeRoundness * lakeSquiggleZ.GetValue(tile));

                //lake.AddRange(GenRadial.RadialCellsAround(newCenter, lakeRoundness + 2, true));
                lake.AddRange(GenRadial.RadialCellsAround(newCenter, 2 + lakeRoundness / 2, true).Except(lake));

            }

            //RoughenShape(lake, "ZMD_Lake", map);
            RoughenShape(lake, lakeGrid, map);

            Log.Message(lake.Count + " lake tiles");

            List<IntVec3> shore = new List<IntVec3>();

            foreach (IntVec3 tile in lake)
            {
                //shore.AddRange(GenRadial.RadialCellsAround(tile, beachSize, true));
                shore.AddRange(GenRadial.RadialCellsAround(tile, lakeRoundness + 2, true).Except(shore));

            }
            Log.Message("gonna roughen beach");
            Log.Message(shore.Count + " shore tiles");
            MapGenFloatGrid shoreGrid = MapGenerator.FloatGridNamed("ZMD_LakeShore");

            //RoughenShape(lake, "ZMD_LakeShore", map);
            RoughenShape(lake, shoreGrid, map);


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
                if (lakeGrid[current] > 0.02f)
                {
                    if (!terrainGrid.TerrainAt(current).IsWater)
                    {
                        terrainGrid.SetTerrain(current, shallowTerr);
                    }

                    //if (Rand.Chance(0.1f))
                    //{
                    //    List<IntVec3> circle = new List<IntVec3>();
                    //    int innerRad = Rand.Range(1, beachSize);
                    //    if(Rand.Bool)
                    //    {
                    //        innerRad = Math.Min(56, innerRad * 3);
                    //    }
                    //    circle = GenRadial.RadialCellsAround(current, innerRad, true).ToList();

                    //    foreach (IntVec3 t in circle)
                    //    {
                    //        if (t.InBounds(map))
                    //        {
                    //            shoreGrid[t] += 0.01f;
                    //        }
                    //    }
                    //}

                    
                }

                if (lakeGrid[current] > 0.5f)
                {
                    terrainGrid.SetTerrain(current, deepTerr);

                }
            }

            // beach terrains

            foreach (IntVec3 current in map.AllCells)
            {
                if (!terrainGrid.TerrainAt(current).IsWater)
                {
                    if (shoreGrid[current] > 0.02f)
                    {
                        terrainGrid.SetTerrain(current, shoreOuterTerr);
                    }
                    if (shoreGrid[current] > 0.3f)
                    {
                        terrainGrid.SetTerrain(current, shoreInnerTerr);
                    }
                    
                }

            }

            foreach(IntVec3 c in shore)
            {
                if (c.InBounds(map))
                {
                    terrainGrid.SetTerrain(c, TerrainDefOf.Ice);
                }
            }

            foreach (IntVec3 c in lake)
            {
                if (c.InBounds(map))
                {
                    terrainGrid.SetTerrain(c, TerrainDefOf.Bridge);
                }
            }
        }



        private void RoughenShape(List<IntVec3> shape, /*string gridName,*/MapGenFloatGrid grid, Map map)
        {
            Log.Message("Roughening shape");
            //MapGenFloatGrid grid = MapGenerator.FloatGridNamed("gridName");

            foreach(IntVec3 tile in shape)
            {
                if(Rand.Chance(1 / (roughness + 1)))
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
                            grid[t] += 0.01f;
                        }
                    }
                }
            }
        }


    }
}
