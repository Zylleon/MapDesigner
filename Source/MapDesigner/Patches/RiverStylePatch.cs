using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;


namespace MapDesigner.Patches
{
    [HarmonyPatch(typeof(RimWorld.RiverMaker))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(float), typeof(RiverDef) })]
    static class RiverStylePatch
    {

        static bool Prefix (ref Vector3 center)
        {
            if (MapDesignerMod.mod.settings.flagRiverLoc)
            {
                center.x += Find.GameInitData.mapSize * MapDesignerMod.mod.settings.riverPctEast;
                center.z += Find.GameInitData.mapSize * MapDesignerMod.mod.settings.riverPctSouth;
            }

            return true;
        }



        static void Postfix(ref ModuleBase ___generator, ModuleBase ___coordinateX, ModuleBase ___coordinateZ, ModuleBase ___shallowizer, float ___surfaceLevel, List<IntVec3> ___lhs, List<IntVec3> ___rhs, Vector3 center, float angle)
        {
            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            //___generator = new Abs(___generator);
            //___coordinateX = new Abs(___coordinateX);
            //ModuleBase originalBranch = new AxisAsValueX();
            //ModuleBase riverA = new AxisAsValueX();
            //ModuleBase riverB = new AxisAsValueX();

            ////ModuleBase originalBranch = new AxisAsValueX();
            ////originalBranch = new Rotate(0.0, angle, 0.0, originalBranch);
            ////originalBranch = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), originalBranch);
            ////originalBranch = new Subtract(new Abs(originalBranch), new Min(___coordinateZ, new Const(0.0)));

            switch (settings.selRiverStyle)
            {
                case MapDesignerSettings.RiverStyle.Vanilla:
                    break;
                case MapDesignerSettings.RiverStyle.Spring:
                    //Log.Message("[Map Designer] Calling springs from the ground");
                    ___generator = new Subtract(new Abs(___coordinateX), new Min(___coordinateZ, new Const(0.0)));

                    break;

                //case MapDesignerSettings.RiverStyle.Confluence:
                //    Log.Message("[Map Designer] Merging rivers");

                //    originalBranch = new Rotate(0.0, angle - 180, 0.0, originalBranch);
                //    originalBranch = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), originalBranch);
                //    originalBranch = new Subtract(new Abs(originalBranch), new Min(___coordinateZ, new Const(0.0)));

                //    riverA = new Rotate(0.0, angle - 30, 0.0, riverA);
                //    riverA = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverA);
                //    riverA = new Subtract(new Abs(riverA), new Min(new Invert(___coordinateZ), new Const(0.0)));

                //    riverB = new Rotate(0.0, angle + 30, 0.0, riverB);
                //    riverB = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverB);
                //    riverB = new Subtract(new Abs(riverB), new Min(new Invert(___coordinateZ), new Const(0.0)));

                //    ___generator = new Min(riverA, riverB);
                //    ___generator = new Min(originalBranch, ___generator);


                //    break;

                //case MapDesignerSettings.RiverStyle.Fork:
                //    Log.Message("[Map Designer] Forking rivers");

                //    originalBranch = new Rotate(0.0, angle, 0.0, originalBranch);
                //    originalBranch = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), originalBranch);
                //    originalBranch = new Subtract(new Abs(originalBranch), new Min(new Invert(___coordinateZ), new Const(0.0)));

                //    riverA = new Rotate(0.0, angle - 30, 0.0, riverA);
                //    riverA = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverA);
                //    riverA = new Subtract(new Abs(riverA), new Min(___coordinateZ, new Const(0.0)));

                //    riverB = new Rotate(0.0, angle + 30, 0.0, riverB);
                //    riverB = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverB);
                //    riverB = new Subtract(new Abs(riverB), new Min(___coordinateZ, new Const(0.0)));

                //    ___generator = new Min(riverA, riverB);
                //    ___generator = new Min(originalBranch, ___generator);

                //    break;

                //case MapDesignerSettings.RiverStyle.Oxbow:
                //    Log.Message("[Map Designer] Winding rivers");

                //    ModuleBase oxbow = new AxisAsValueX();

                //    ModuleBase x = new Perlin(0.008, 0.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
                //    ModuleBase z = new Perlin(0.015, 0.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
                //    //ModuleBase scaling = new Const(40.0);
                //    x = new Multiply(x, new Const(50.0));
                //    z = new Multiply(z, new Const(30.0));
                //    oxbow = new Displace(oxbow, x, new Const(0.0), z);

                //    oxbow = new Rotate(0.0, 0.0 - angle, 0.0, oxbow);
                //    oxbow = new Translate(0.0 - center.x, 0.0, 0.0 - center.z, oxbow);
                //    ___coordinateX = oxbow;
                //    ___generator = oxbow;

                //    break;

                default:
                    break;
            }

        }

    }



    /// <summary>
    /// Passability is guaranteed for springs, so there is no need to check it.
    /// Checking passability for springs results in an error, this prevents the error
    /// This does not change map generation
    /// </summary>
    [HarmonyPatch(typeof(RimWorld.RiverMaker), "ValidatePassage")]
    static class River_ValidatePassage_Patch
    {
        static bool Prefix()
        {
            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            if (settings.selRiverStyle == MapDesignerSettings.RiverStyle.Spring)
            {
                return false;
            }
            return true;
        }
    }

}
