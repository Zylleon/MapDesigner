using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MapDesigner.Patches
{

    [HarmonyPatch(typeof(RimWorld.TileMutatorWorker_Caves))]
    [HarmonyPatch(nameof(RimWorld.TileMutatorWorker_Caves.GeneratePostElevationFertility))]
    static class CaveSettingsPatch
    {
        static bool Prefix(Map map)
        {
            if (!MapDesignerMod.mod.settings.flagCaves)
            {
                return false;
            }
            return true;
        }
    }

}
