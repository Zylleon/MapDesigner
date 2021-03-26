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

        private Map myMap;

        private IntVec3 center = new IntVec3(125, 0, 125);

        private List<IntVec3> beachCells = new List<IntVec3>();
        private List<IntVec3> landCells = new List<IntVec3>();

        private int outerRadius = 50;
        private int innerRadius = 40;
        private int beachSize = (int)MapDesignerMod.mod.settings.priBeachSize;

        public override void Generate(Map map, GenStepParams parms)
        {
            myMap = map;
            //IntVec3 center = map.Center;
            center = myMap.Center;

            outerRadius = (int)(0.01 * myMap.Size.x * MapDesignerMod.mod.settings.priIslandSize);

            int beachSize = (int)MapDesignerMod.mod.settings.priBeachSize;
            innerRadius = outerRadius - beachSize;



            Log.Message("[Map Designer] Creating multiple islands");


            switch (MapDesignerMod.mod.settings.priStyle)
            {
                case MapDesignerSettings.PriStyle.Single:
                    GenSingleIsland();
                    break;

                case MapDesignerSettings.PriStyle.Multi:
                    GenMultiIslands();
                    break;

            }

            if (false)
            {
                /*
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

                    center.x += (int)(MapDesignerMod.mod.settings.priSingleCenterLoc.x * map.Size.x);
                    center.z += (int)(MapDesignerMod.mod.settings.priSingleCenterLoc.z * map.Size.z);

                    landCells = HelperMethods.GenCircle(map, center, innerRadius);
                    beachCells = HelperMethods.GenCircle(map, center, outerRadius).Except(landCells).ToList();
                }

                */
            }

            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            List<IntVec3> waterCells = map.AllCells.Except(landCells).Except(beachCells).ToList();

            foreach (IntVec3 current in beachCells)
            {
                fertility[current] = -1075f;
                elevation[current] = 0;
            }

            foreach (IntVec3 current in waterCells)
            {
                fertility[current] = -2025f;
                elevation[current] = 0;
            }
        }
        


        private void GenMultiIslands()
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
                    if (c.Center.DistanceToEdge(myMap) >= 15)
                    {
                        List<IntVec3> newCells = HelperMethods.GenCircle(myMap, c.Center, c.Radius);
                        if (!newCells.Intersect(beachCells).Any())
                        {
                            finalIslands.Add(c);
                            beachCells.AddRange(HelperMethods.GenCircle(myMap, c.Center, c.Radius));
                        }

                    }
                }
            }

            foreach (CircleDef f in finalIslands)
            {
                if (f.Radius > beachSize * 2)
                {
                    landCells.AddRange(HelperMethods.GenCircle(myMap, f.Center, f.Radius - beachSize));
                    beachCells = beachCells.Except(HelperMethods.GenCircle(myMap, f.Center, f.Radius - beachSize)).ToList();
                }
            }
        }

        private void GenSingleIsland()
        {
            Log.Message("[Map Designer] Creating single island");

            center.x += (int)(MapDesignerMod.mod.settings.priSingleCenterLoc.x * myMap.Size.x);
            center.z += (int)(MapDesignerMod.mod.settings.priSingleCenterLoc.z * myMap.Size.z);

            landCells = HelperMethods.GenCircle(myMap, center, innerRadius);
            beachCells = HelperMethods.GenCircle(myMap, center, outerRadius).Except(landCells).ToList();
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
