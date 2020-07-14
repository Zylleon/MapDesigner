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
    //[HarmonyPatch(typeof(RimWorld.GenStep_Terrain))]
    [HarmonyPatch(typeof(RimWorld.GenStep_Terrain), "GenerateRiver")]
    internal static class RiverDirectionPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            ConstructorInfo rivermaker = AccessTools.Constructor(typeof(RiverMaker), new Type[] { typeof(Vector3), typeof(float), typeof(RiverDef) });
            int riverIndex = -1;
            int replaceIndex = -1;


            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Newobj)
                {
                    if (codes[i].OperandIs(rivermaker))
                    {
                        Log.Message(String.Format("Found rivermaker on line {0}", i));
                        riverIndex = i;
                        break;
                    }
                }
                if (codes[i].opcode == OpCodes.Ldloc_1)
                {
                    replaceIndex = i;
                    Log.Message(String.Format("Found argument on line {0}", i));
                }
            }



            //for (int i = 0; i < codes.Count; i++)
            //{
            //    if (codes[i].opcode == OpCodes.Newobj)
            //    {
            //        if(codes[i].OperandIs(rivermaker))
            //        {
            //            Log.Message(String.Format("Found rivermaker on line {0}", i));
            //            riverIndex = i;
            //        }
            //    }
            //    if(riverIndex != -1 && replaceIndex == -1 &&  codes[i].opcode == OpCodes.Ldarg_1)
            //    {
            //        replaceIndex = i;
            //        Log.Message(String.Format("Found argument on line {0}", i));
            //    }
            //}

            //codes.Insert(replaceIndex + 1, new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetRiverDirection))));

            Log.Message(String.Format("----- Argument on line {0} ---- rivermaker on line {1}", replaceIndex, riverIndex));

            codes.Insert(replaceIndex + 1, new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetRiverDirection))));

            //codes.Insert(replaceIndex + 1, new CodeInstruction(opcode: OpCodes.Callvirt, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetRiverDirection))));


            //codes[index] = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetRiverDirection)));

            return codes.AsEnumerable();

        }
    }
    //[HarmonyPatch(typeof(RimWorld.RiverMaker), MethodType.Constructor, new Type[] { typeof(Vector3), typeof(float), typeof(RiverDef) })]
    //internal static class RiverDirectionPatch
    //{
    //    //static bool Prefix(ref Vector3 __center, ref float __angle, ref RiverDef __riverDef)
    //    static bool Prefix(Vector3 center, float angle, RiverDef riverDef)
    //    {
    //        Log.Message("RIVER PATCH TEST");
    //        __angle = 0f;

    //        return true;
    //    }
    //}

}
