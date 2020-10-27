using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace MapDesigner.Patches
{

    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    internal static class MountainSettingsPatch
    {
        /// <summary>
        /// Mountain size and smoothness
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            int hillSizeIndex = -1;
            int hillSmoothnessIndex = -1;
            float result = -1f;

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R8)
                {
                    if (float.TryParse(codes[i].operand.ToString(), out result))
                    {
                        if (hillSmoothnessIndex == -1 && result == 2f)
                        {
                            hillSmoothnessIndex = i;
                        }
                        if (hillSizeIndex == -1 && result == 0.021f)
                        {
                            hillSizeIndex = i;
                        }
                    }
                }
                if (hillSizeIndex != -1 && hillSmoothnessIndex != -1)
                {
                    break;
                }
            }
            //codes[8] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSize)));
            //codes[9] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSmoothness)));
            codes[hillSizeIndex] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSize)));
            codes[hillSmoothnessIndex] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetHillSmoothness)));

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Mountain amount
        /// Natural hill distribution
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        static void Postfix(Map map, GenStepParams parms)
        {
            // Map Reroll uses a map stub to generate previews, which is missing this info. 
            // Checking  map.info.parent.def  filters Map Reroll previews and categorizes them as "player home" maps
            bool isPlayerHome = false;
            if (map.info.parent.def == null)
            {
                isPlayerHome = true;
            }
            else if(map.IsPlayerHome)
            {
                isPlayerHome = true;
            }
            if (MapDesignerSettings.flagHomeMapOnly && !isPlayerHome)
            {
                return;
            }

            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            MapGenFloatGrid elevation = MapGenerator.Elevation;

            // pushes hills away from center
            if (MapDesignerMod.mod.settings.flagHillRadial)
            {
                IntVec3 center = map.Center;
                int size = map.Size.x / 2;
                float centerSize = settings.hillRadialSize * size;
                foreach (IntVec3 current in map.AllCells)
                {
                    float distance = (float)Math.Sqrt(Math.Pow(current.x - center.x, 2) + Math.Pow(current.z - center.z, 2));
                    elevation[current] += (settings.hillRadialAmt * (distance - centerSize) / size);
                }
            }

            // hills to both sides
            if (MapDesignerMod.mod.settings.flagHillSplit)
            {
                float angle = settings.hillSplitDir;

                int mapSize = map.Size.x;
                float gapSize = 0.5f * mapSize * settings.hillSplitSize;
                float skew = settings.hillSplitAmt;

                ModuleBase slope = new AxisAsValueX();
                slope = new Rotate(0.0, 180.0 - angle, 0.0, slope);

                slope = new Translate(0.0 - map.Center.x, 0.0, 0.0 - map.Center.z, slope);

                float multiplier = skew / mapSize;

                foreach (IntVec3 current in map.AllCells)
                {
                    float value = slope.GetValue(current);
                    //float num = size - Math.Abs(value);
                    float num = Math.Abs(value) - gapSize;

                    //num = 1 + (skew * num / mapSize);
                    //num = 1 + num * multiplier;
                    elevation[current] += num * multiplier;
                    //elevation[current] *= num;
                    //elevation[current] += num - 1;
                }
            }

            // hills to one side
            if (MapDesignerMod.mod.settings.flagHillSide)
            {
                float angle = settings.hillSideDir;
                float skew = settings.hillSideAmt;

                ModuleBase slope = new AxisAsValueX();
                slope = new Rotate(0.0, 180.0 - angle, 0.0, slope);
                slope = new Translate(0.0 - map.Center.x, 0.0, 0.0 - map.Center.z, slope);
                float multiplier = skew / map.Size.x;
                foreach (IntVec3 current in map.AllCells)
                {
                    //elevation[current] *= (1 + slope.GetValue(current) * multiplier);
                    //elevation[current] += 0.5f * slope.GetValue(current) * multiplier;
                    elevation[current] += slope.GetValue(current) * multiplier;

                }

            }

            // hill amount
            float hillAmount = settings.hillAmount;
            foreach (IntVec3 current in map.AllCells)
            {
                elevation[current] += settings.hillAmount - 1f;
            }

            // natural distribution
            if (MapDesignerMod.mod.settings.flagHillClumping)
            {
                float hillSize = MapDesignerMod.mod.settings.hillSize;

                if (hillSize > 0.022f)       // smaller than vanilla only, else skip this step
                {
                    float clumpSize = Rand.Range(0.01f, Math.Min(0.04f, hillSize));
                    float clumpStrength = Rand.Range(0.3f, 0.7f);

                    ModuleBase hillClumping = new Perlin(clumpSize, 0.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Low);

                    foreach (IntVec3 current in map.AllCells)
                    {
                        elevation[current] += clumpStrength * hillClumping.GetValue(current);
                    }
                }
            }

            // exit path
            if (MapDesignerMod.mod.settings.flagMtnExit)
            {
                ModuleBase exitPath = new AxisAsValueX();
                ModuleBase crossways = new AxisAsValueZ();
                double exitDir = Rand.Range(0, 360);

                //ModuleBase noise = new Perlin(0.021, 3.5, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
                //ModuleBase noise = new Perlin(Rand.Range(0.015f, 0.035f), 2.0, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
                ModuleBase noise = new Perlin(Rand.Range(0.015f, 0.03f), Math.Min(3.5, settings.hillSmoothness), 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);

                noise = new Multiply(noise, new Const(25.0));
                exitPath = new Displace(exitPath, noise, new Const(0.0), new Const(0.0));

                exitPath = new Rotate(0.0, exitDir, 0.0, exitPath);
                crossways = new Rotate(0.0, exitDir, 0.0, crossways);

                exitPath = new Translate((double)(-(double)map.Center.x), 0.0, (double)(-(double)map.Center.z), exitPath);
                crossways = new Translate((double)(-(double)map.Center.x), 0.0, (double)(-(double)map.Center.z), crossways);
                exitPath = new Abs(exitPath);

                foreach (IntVec3 current in map.AllCells)
                {
                    if (crossways.GetValue(current) > 0f)
                    {
                        elevation[current] *= Math.Min(1, 0.1f * exitPath.GetValue(current) - 0.5f);
                    }
                }

            }


            if (isPlayerHome)
            {
                GenerateFeatureGrids(map, parms);
            }

        }

        static void GenerateFeatureGrids(Map map, GenStepParams parms)
        {
            //bool isPlayerHome = false;
            //if (map.info.parent.def == null)
            //{
            //    isPlayerHome = true;
            //}
            //else if (map.IsPlayerHome)
            //{
            //    isPlayerHome = true;
            //}
            //if (!isPlayerHome)
            //{
            //    return;
            //}

            if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.None)
            {
                return;
            }
            else if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.Lake)
            {
                Log.Message("[Map Desigher] Filling lakes...");
                new Feature.Lake().Generate(map, parms);
                return;
            }

            else if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.RoundIsland)
            {
                if (map.Biome.defName.Contains("BiomesIsland"))
                {
                    Log.Message("Can't make round islands, this is already an island!");
                    return;
                }

                Log.Message("[Map Desigher] Carving islands...");

                new Feature.RoundIsland().Generate(map, parms);
                //RoundIsland.AdjustMapGrids(map);

                return;
            }

        }

    }
}
