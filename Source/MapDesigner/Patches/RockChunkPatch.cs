using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDesigner.Patches
{

    [HarmonyPatch(typeof(RimWorld.GenStep_RockChunks), "Generate")]
    internal static class RockChunkPatch
    {
        static bool Prefix()
        {
            return MapDesignerMod.mod.settings.flagRockChunks;
        }


    }
}
