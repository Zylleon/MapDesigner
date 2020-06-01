using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


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

        //public ZMD_LakeSettings settings;
        private int beachSize;
        private int lakeSize;

        public override void Generate(Map map, GenStepParams parms)
        {
            //settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<ZMD_LakeSettings>();
            Log.Message("Genning Lake");

            MapGenFloatGrid lakeGrid = MapGenerator.FloatGridNamed("ZMD_Lake");

            //main circle
            IntVec3 mapCenter = map.Center;

            lakeSize = map.Size.z / 10;
            beachSize = 15;

            List<IntVec3> bigCircle = new List<IntVec3>();
            foreach (IntVec3 current in map.AllCells)
            {
                if (DistanceBetweenPoints(mapCenter, current) < lakeSize)
                {
                    bigCircle.Add(current);
                }
            }


            //make lake squiggly
            List<IntVec3> lake = new List<IntVec3>();

            foreach (IntVec3 tile in bigCircle)
            {
                if(Rand.Chance(0.1f))
                {
                    List<IntVec3> circle = new List<IntVec3>();
                    circle = GenRadial.RadialCellsAround(tile, Rand.Range(lakeSize / 2, lakeSize), true).ToList();

                    foreach (IntVec3 t in circle)
                    {
                        if (t.InBounds(map))
                        {
                            lakeGrid[t] += 0.01f;
                        }
                    }
                }
            }

            TerrainGrid terrainGrid = map.terrainGrid;
            TerrainDef shoreOuterTerr = TerrainDefOf.Soil;
            TerrainDef shoreInnerTerr = TerrainDef.Named("SoilRich");
            TerrainDef shallowTerr = TerrainDefOf.WaterShallow;
            TerrainDef deepTerr = TerrainDefOf.WaterDeep;


            MapGenFloatGrid shoreGrid = MapGenerator.FloatGridNamed("ZMD_LakeShore");


            // apply lake terrains
            foreach (IntVec3 current in map.AllCells)
            {
                if (lakeGrid[current] > 0.05f)
                {
                    if (!terrainGrid.TerrainAt(current).IsWater)
                    {
                        terrainGrid.SetTerrain(current, shallowTerr);
                    }

                    if (Rand.Chance(0.1f))
                    {
                        List<IntVec3> circle = new List<IntVec3>();
                        int innerRad = Rand.Range(1, beachSize);
                        if(Rand.Bool)
                        {
                            innerRad = Math.Min(56, innerRad * 3);
                        }
                        circle = GenRadial.RadialCellsAround(current, innerRad, true).ToList();

                        foreach (IntVec3 t in circle)
                        {
                            if (t.InBounds(map))
                            {
                                shoreGrid[t] += 0.01f;
                            }
                        }
                    }

                    
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
                    Log.Message("terraining");
                    if (shoreGrid[current] > 0.1f)
                    {
                        terrainGrid.SetTerrain(current, shoreOuterTerr);
                    }
                    if (shoreGrid[current] > 0.3f)
                    {
                        terrainGrid.SetTerrain(current, shoreInnerTerr);
                    }
                    
                }

            }
        }




        private float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);
            dist = (float)Math.Sqrt(xDist + zDist);

            return dist;
        }

    }
}
