using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;


namespace MapDesigner.Patches
{
    [HarmonyPatch(typeof(RimWorld.GenStep_AnimaTrees))]
    [HarmonyPatch(nameof(RimWorld.GenStep_AnimaTrees.DesiredTreeCountForMap))]
    static class AnimaTreePatch
    {
        static void Postfix(ref int __result)
        {
            float animaCount = MapDesigner_Mod.mod.settings.animaCount;
            __result *= (int)animaCount;
        }
    }


}
