using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;

using RimWorld;

namespace MapDesigner
{
    public class ZMD_RiverMaker
    {
        private ModuleBase generator;

        private ModuleBase coordinateX;

        private ModuleBase coordinateZ;

        private ModuleBase shallowizer;

        private float surfaceLevel;

        private float shallowFactor = 0.2f;

        private List<IntVec3> lhs = new List<IntVec3>();

        private List<IntVec3> rhs = new List<IntVec3>();

        public ZMD_RiverMaker(Vector3 center, float angle, RiverDef riverDef)
        {
            this.surfaceLevel = riverDef.widthOnMap / 2f;
            this.coordinateX = new AxisAsValueX();
            this.coordinateZ = new AxisAsValueZ();
            this.coordinateX = new Rotate(0.0, (double)(-(double)angle), 0.0, this.coordinateX);
            this.coordinateZ = new Rotate(0.0, (double)(-(double)angle), 0.0, this.coordinateZ);
            this.coordinateX = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), this.coordinateX);
            this.coordinateZ = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), this.coordinateZ);
            ModuleBase x = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
            ModuleBase z = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);

            //ModuleBase x = new Perlin(0.030, 0.5, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
            //ModuleBase z = new Perlin(0.080, 0.5, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);

            ModuleBase dirFactor = new Const(8.0);

            x = new Multiply(x, dirFactor);
            z = new Multiply(z, dirFactor);

            //ModuleBase xDirFactor = new Const(10.0);
            //ModuleBase zDirFactor = new Const(50.0);

            //x = new Multiply(x, xDirFactor);
            //z = new Multiply(z, zDirFactor);

            this.coordinateX = new Displace(this.coordinateX, x, new Const(0.0), x);
            this.coordinateZ = new Displace(this.coordinateZ, x, new Const(0.0), z);
            this.generator = this.coordinateX;
            this.shallowizer = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
            this.shallowizer = new Abs(this.shallowizer);
        }

        public TerrainDef TerrainAt(IntVec3 loc, bool recordForValidation = false)
        {
            float value = this.generator.GetValue(loc);
            float num = this.surfaceLevel - Mathf.Abs(value);
            if (num > 2f && this.shallowizer.GetValue(loc) > this.shallowFactor)
            {
                return TerrainDefOf.WaterMovingChestDeep;
            }
            if (num > 0f)
            {
                if (recordForValidation)
                {
                    if (value < 0f)
                    {
                        this.lhs.Add(loc);
                    }
                    else
                    {
                        this.rhs.Add(loc);
                    }
                }
                return TerrainDefOf.WaterMovingShallow;
            }

            ////my stuff here

            //if(Math.Sqrt(Mathf.Abs(this.surfaceLevel)) - Mathf.Abs(value) < -20f)         // idk
            //{
            //    return TerrainDefOf.Ice;
            //}

            //if (Mathf.Abs(value) < Rand.Range(12, 16))                    // random fuzzing
            //{
            //    return TerrainDef.Named("SoilRich");
            //}

            //if (Mathf.Abs(value) + Math.Max(loc.x % 10, loc.z % 10) < 30f)        // bleh
            //{
            //    return TerrainDefOf.Ice;
            //}

            //if (surfaceLevel > 2.5f)
            //{
            //    return TerrainDefOf.Ice;
            //}

            //if (num > -10)
            //{
            //    return TerrainDefOf.Ice;
            //}

            ////my stuff ends

            return null;
        }

        public Vector3 WaterCoordinateAt(IntVec3 loc)
        {
            return new Vector3(this.coordinateX.GetValue(loc), 0f, this.coordinateZ.GetValue(loc));
        }

        public void ValidatePassage(Map map)
        {
            IntVec3 intVec = (from loc in this.lhs
                              where loc.InBounds(map) && loc.GetTerrain(map) == TerrainDefOf.WaterMovingShallow
                              select loc).RandomElementWithFallback(IntVec3.Invalid);
            IntVec3 intVec2 = (from loc in this.rhs
                               where loc.InBounds(map) && loc.GetTerrain(map) == TerrainDefOf.WaterMovingShallow
                               select loc).RandomElementWithFallback(IntVec3.Invalid);
            if (intVec == IntVec3.Invalid || intVec2 == IntVec3.Invalid)
            {
                Log.Error("Failed to find river edges in order to verify passability", false);
                return;
            }
            while (!map.reachability.CanReach(intVec, intVec2, PathEndMode.OnCell, TraverseMode.PassAllDestroyableThings))
            {
                if (this.shallowFactor > 1f)
                {
                    Log.Error("Failed to make river shallow enough for passability", false);
                    return;
                }
                this.shallowFactor += 0.1f;
                foreach (IntVec3 current in map.AllCells)
                {
                    if (current.GetTerrain(map) == TerrainDefOf.WaterMovingChestDeep && this.shallowizer.GetValue(current) <= this.shallowFactor)
                    {
                        map.terrainGrid.SetTerrain(current, TerrainDefOf.WaterMovingShallow);
                    }
                }
            }
        }
    }
}