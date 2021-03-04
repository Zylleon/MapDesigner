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
                        riverIndex = i;
                        Log.Message("River index: " + riverIndex);
                        break;
                    }
                }
                if (codes[i].opcode == OpCodes.Ldloc_1)
                {
                    replaceIndex = i;
                    Log.Message("replace index: " + replaceIndex);
                }
            }
            codes.Insert(replaceIndex + 1, new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(HelperMethods), name: nameof(HelperMethods.GetRiverDirection))));

            return codes.AsEnumerable();
        }
    }

}
