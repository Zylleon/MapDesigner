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
    public static class MapGenSize
    {
        public static IntVec3 mapgensize { get; set; }
    }

    [HarmonyPatch(typeof(RimWorld.TileMutatorWorker_River), "GetRiverCenter")]
    static class RiverCenterPatch
    {
        static void Postfix(Map map, ref IntVec3 __result)
        {
            if (MapDesignerMod.mod.settings.flagRiverLoc)
            {

                __result.x = (int)(MapGenSize.mapgensize.x * (0.5f + MapDesignerMod.mod.settings.riverCenterDisp.x));
                __result.z = (int)(MapGenSize.mapgensize.z * (0.5f + MapDesignerMod.mod.settings.riverCenterDisp.z));

                //__result.x -= 0.5f;
                //__result.z -= 0.5f;
            }
        }
    }




    /*
       [HarmonyPatch(typeof(RimWorld.RiverMaker))]
       [HarmonyPatch(MethodType.Constructor)]
       [HarmonyPatch(new Type[] { typeof(Vector3), typeof(float), typeof(RiverDef) })]
       static class RiverStylePatch
       {

           static bool Prefix(ref Vector3 center)
           {
               if (MapDesignerMod.mod.settings.flagRiverLoc)
               {
                   if (MapDesignerMod.mod.settings.flagRiverLocAbs)
                   {
                       center.x = MapGenSize.mapgensize.x * (0.5f + MapDesignerMod.mod.settings.riverCenterDisp.x);
                       center.z = MapGenSize.mapgensize.z * (0.5f + MapDesignerMod.mod.settings.riverCenterDisp.z);
                   }
                   else
                   {
                       center.x += MapGenSize.mapgensize.x * MapDesignerMod.mod.settings.riverCenterDisp.x;
                       center.z += MapGenSize.mapgensize.z * MapDesignerMod.mod.settings.riverCenterDisp.z;
                   }
                   center.x -= 0.5f;
                   center.z -= 0.5f;
               }

               return true;
           }

       }

           static void Postfix(ref ModuleBase ___generator, ref ModuleBase ___coordinateX, ref ModuleBase ___coordinateZ, ModuleBase ___shallowizer, float ___surfaceLevel, List<IntVec3> ___lhs, List<IntVec3> ___rhs, Vector3 center, float angle)
           {
               MapDesignerSettings settings = MapDesignerMod.mod.settings;
               //___generator = new Abs(___generator);
               //___coordinateX = new Abs(___coordinateX);
               ModuleBase originalBranch = new AxisAsValueX();
               ModuleBase riverA = new AxisAsValueX();
               ModuleBase riverB = new AxisAsValueX();

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

                   case MapDesignerSettings.RiverStyle.Canal:
                       ___coordinateX = new AxisAsValueX();
                       ___coordinateZ = new AxisAsValueZ();
                       ___coordinateX = new Rotate(0.0, 0f - angle, 0.0, ___coordinateX);
                       ___coordinateZ = new Rotate(0.0, 0f - angle, 0.0, ___coordinateZ);
                       ___coordinateX = new Translate(0f - center.x, 0.0, 0f - center.z, ___coordinateX);
                       ___coordinateZ = new Translate(0f - center.x, 0.0, 0f - center.z, ___coordinateZ);
                       ___generator = ___coordinateX;
                       break;

                   case MapDesignerSettings.RiverStyle.Confluence:
                       Log.Message("[Map Designer] Merging rivers");

                       ___coordinateX = new AxisAsValueX();
                       ___coordinateZ = new AxisAsValueZ();
                       //___coordinateX = new Rotate(0.0, 0f - angle, 0.0, ___coordinateX);
                       //___coordinateZ = new Rotate(0.0, 0f - angle, 0.0, ___coordinateZ);
                       ___coordinateX = new Rotate(0.0, 0f - angle, 0.0, ___coordinateX);
                       ___coordinateZ = new Rotate(0.0, 0f - angle, 0.0, ___coordinateZ);
                       ___coordinateX = new Translate(0f - center.x, 0.0, 0f - center.z, ___coordinateX);
                       ___coordinateZ = new Translate(0f - center.x, 0.0, 0f - center.z, ___coordinateZ);
                       ModuleBase moduleBase = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Range(0, int.MaxValue), QualityMode.Medium);
                       ModuleBase moduleBase2 = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Range(0, int.MaxValue), QualityMode.Medium);
                       ModuleBase moduleBase3 = new Const(8.0);
                       moduleBase = new Multiply(moduleBase, moduleBase3);
                       moduleBase2 = new Multiply(moduleBase2, moduleBase3);
                       ___coordinateX = new Displace(___coordinateX, moduleBase, new Const(0.0), moduleBase2);
                       ___coordinateZ = new Displace(___coordinateZ, moduleBase, new Const(0.0), moduleBase2);

                       originalBranch = new Rotate(0.0, 180 - angle, 0.0, originalBranch);

                       originalBranch = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), originalBranch);
                       originalBranch = new Subtract(new Abs(originalBranch), new Min(___coordinateZ, new Const(0.0)));

                       riverA = new Rotate(0.0, 30 - angle, 0.0, riverA);
                       riverA = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverA);
                       riverA = new Subtract(new Abs(riverA), new Min(new Invert(___coordinateZ), new Const(0.0)));

                       riverB = new Rotate(0.0, 330 - angle, 0.0, riverB);
                       riverB = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverB);
                       riverB = new Subtract(new Abs(riverB), new Min(new Invert(___coordinateZ), new Const(0.0)));

                       ___generator = new Min(riverA, riverB);
                       ___generator = new Min(originalBranch, ___generator);



                       ___generator = new Displace(___generator, moduleBase, new Const(0.0), moduleBase2);

                       break;


                   case MapDesignerSettings.RiverStyle.Fork:
                       Log.Message("[Map Designer] Forking rivers");

                       ModuleBase forkNoise1 = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Range(0, int.MaxValue), QualityMode.Medium);
                       ModuleBase forkNoise2 = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Range(0, int.MaxValue), QualityMode.Medium);
                       ModuleBase forkNoise3 = new Const(8.0);
                       forkNoise1 = new Multiply(forkNoise1, forkNoise3);
                       forkNoise2 = new Multiply(forkNoise2, forkNoise3);
                       ___coordinateX = new Displace(___coordinateX, forkNoise1, new Const(0.0), forkNoise2);
                       ___coordinateZ = new Displace(___coordinateZ, forkNoise1, new Const(0.0), forkNoise2);

                       originalBranch = new Rotate(0.0, 0 - angle, 0.0, originalBranch);
                       originalBranch = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), originalBranch);
                       originalBranch = new Subtract(new Abs(originalBranch), new Min(new Invert(___coordinateZ), new Const(0.0)));

                       riverA = new Rotate(0.0, 150 - angle, 0.0, riverA);
                       riverA = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverA);
                       riverA = new Subtract(new Abs(riverA), new Min(___coordinateZ, new Const(0.0)));

                       riverB = new Rotate(0.0, -150 - angle, 0.0, riverB);
                       riverB = new Translate((double)(-(double)center.x), 0.0, (double)(-(double)center.z), riverB);
                       riverB = new Subtract(new Abs(riverB), new Min(___coordinateZ, new Const(0.0)));

                       ___generator = new Min(riverA, riverB);
                       ___generator = new Min(originalBranch, ___generator);
                       ___generator = new Displace(___generator, forkNoise1, new Const(0.0), forkNoise2);

                       break;

                   case MapDesignerSettings.RiverStyle.Oxbow:
                       Log.Message("[Map Designer] Winding rivers");

                       ModuleBase oxbow = new AxisAsValueX();

                       ModuleBase x = new Perlin(0.01, 1.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
                       ModuleBase z = new Perlin(0.020, 1.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
                       //ModuleBase scaling = new Const(40.0);
                       x = new Multiply(x, new Const(60.0));
                       z = new Multiply(z, new Const(45.0));
                       oxbow = new Displace(oxbow, x, new Const(0.0), z);

                       oxbow = new Rotate(0.0, 0.0 - angle, 0.0, oxbow);
                       oxbow = new Translate(0.0 - center.x, 0.0, 0.0 - center.z, oxbow);
                       ___coordinateX = oxbow;
                       ___generator = oxbow;

                       break;

                   default:
                       break;
               }

           }

       }

   */






    /// <summary>
    /// Passability is guaranteed for springs, so there is no need to check it.
    /// Checking passability for springs results in an error, this prevents the error
    /// This does not change map generation
    /// </summary>
    /// 
    /*
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
            if (settings.selRiverStyle == MapDesignerSettings.RiverStyle.Confluence)
            {
                return false;
            }
            if (settings.selRiverStyle == MapDesignerSettings.RiverStyle.Fork)
            {
                return false;
            }
            return true;
        }
    }


    */


}
