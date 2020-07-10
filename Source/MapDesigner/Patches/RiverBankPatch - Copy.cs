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
    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    static class DebugTerrainPatch
    {
        static void Postfix(Map map, GenStepParams parms)
        {
            float min = 999f;
            float max = -999f;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            foreach (IntVec3 current2 in map.AllCells)
            {
                if(fertility[current2] < min)
                {
                    min = fertility[current2];
                }
                if(fertility[current2] > max)
                {
                    max = fertility[current2];
                }
            }
            
            Log.Message(String.Format("--------- Min: {0} | Max: {1}", Math.Round(min, 2), Math.Round(max, 2)));
        }
    }


}
