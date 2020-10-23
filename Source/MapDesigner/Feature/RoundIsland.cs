using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace MapDesigner.Feature
{
    public class RoundIsland : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1129121203;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            //MapGenFloatGrid priGrid = MapGenerator.FloatGridNamed("ZMD_PRI");

            IntVec3 center = map.Center;

            int outerRadius = (int)(0.01 * map.Size.x * MapDesignerMod.mod.settings.priIslandSize);

            int beachSize = (int)MapDesignerMod.mod.settings.priBeachSize;
            int innerRadius = outerRadius - beachSize;

            List<IntVec3> beachCells = new List<IntVec3>();
            List<IntVec3> landCells = new List<IntVec3>();

            Log.Message("[Map Designer] Creating multiple islands");

            if (MapDesignerMod.mod.settings.priMultiSpawn)
            {
                Log.Message("[Map Designer] Island Multispawn");

                outerRadius /= 2;
                innerRadius /= 2;
                //beachSize /= 2;

                List<CircleDef> islandList = new List<CircleDef>();
                islandList.Add(new CircleDef(center, outerRadius));
                CircleDef circle = new CircleDef(center, outerRadius);
                islandList = GenNestedCircles(circle, islandList);

                List<CircleDef> finalIslands = new List<CircleDef>();

                islandList = islandList.OrderByDescending(ci => ci.Radius).ToList();

                foreach (CircleDef c in islandList)
                {
                    if (c.Radius > 3)
                    {
                        if (c.Center.DistanceToEdge(map) >= 15)
                        {
                            List<IntVec3> newCells = HelperMethods.GenCircle(map, c.Center, c.Radius);
                            if (!newCells.Intersect(beachCells).Any())
                            {
                                finalIslands.Add(c);
                                beachCells.AddRange(HelperMethods.GenCircle(map, c.Center, c.Radius));
                            }

                        }
                    }
                }

                foreach (CircleDef f in finalIslands)
                {
                    if (f.Radius > beachSize * 2)
                    {
                        landCells.AddRange(HelperMethods.GenCircle(map, f.Center, f.Radius - beachSize));
                        beachCells = beachCells.Except(HelperMethods.GenCircle(map, f.Center, f.Radius - beachSize)).ToList();
                    }
                }
            }

            else
            {
                Log.Message("[Map Designer] Creating single island");

                landCells = HelperMethods.GenCircle(map, center, innerRadius);
                beachCells = HelperMethods.GenCircle(map, center, outerRadius).Except(landCells).ToList();
            }


            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            List<IntVec3> waterCells = map.AllCells.Except(landCells).Except(beachCells).ToList();


            foreach (IntVec3 current in beachCells)
            {
                //priGrid[current] = 1f;
                fertility[current] = -995f;
                elevation[current] = 0;
            }


            foreach (IntVec3 current in waterCells)
            {
                //priGrid[current] = 2f;
                fertility[current] = -999f;
                elevation[current] = 0;
            }
        }
        

        private static List<CircleDef> GenNestedCircles(CircleDef starterCircle, List<CircleDef> circles, int i = 0)
        {
            int radDist = starterCircle.Radius;

            if (i < 3)
            {
                IntRange additions = new IntRange((int)(radDist * 1.3), (int)(radDist * 3));
                IntRange sizeWobble = new IntRange(-5, 5);

                for (int j = 0; j < 4 - i; j++)
                {
                    IntVec3 newCenter = starterCircle.Center;
                    if (Rand.Bool)
                    {
                        newCenter.x += additions.RandomInRange;
                    }
                    else
                    {
                        newCenter.x -= additions.RandomInRange;
                    }
                    if (Rand.Bool)
                    {
                        newCenter.z += additions.RandomInRange;
                    }
                    else
                    {
                        newCenter.z -= additions.RandomInRange;
                    }

                    CircleDef newCircle = new CircleDef(newCenter, (int)(sizeWobble.RandomInRange + radDist * 0.65));
                    circles.Add(newCircle);
                    circles.Concat(GenNestedCircles(newCircle, circles, i + 1));
                }
            }

            return circles;

        }


        class CircleDef
        {
            public IntVec3 Center;
            public int Radius;

            public CircleDef(IntVec3 center, int radius)
            {
                Center = center;
                Radius = radius;
            }
        }

    }
}
