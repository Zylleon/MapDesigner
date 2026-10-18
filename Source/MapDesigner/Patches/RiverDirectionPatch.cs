using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;

namespace MapDesigner.Patches
{

    [HarmonyPatch(typeof(RimWorld.TileMutatorWorker_River), "IsFlowingAToB")]
    static class RiverDirectionPatch_Flowing
    {
        static bool Prefix(Vector3 a, Vector3 b, ref float angle)
        {
            angle = HelperMethods.GetRiverDirection(angle);

            return true;
        }
    }

    [HarmonyPatch(typeof(RimWorld.TileMutatorWorker_River), "GetMapEdgeNodes")]
    static class RiverDirectionPatch_EdgeNodes
    {
        static bool Prefix(Map map, ref float angle)
        {
            angle = HelperMethods.GetRiverDirection(angle);

            return true;
        }
    }


}
