using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using static MapDesigner.MapDesignerSettings;

namespace MapDesigner.Patches
{
    [HarmonyPatch(typeof(RimWorld.Planet.World), "CoastDirectionAt")]
    static class CoastDirPatch
    {
        static void Postfix(int tileID, ref Rot4 __result)
        {
            if(MapDesignerMod.mod.settings.coastDir == CoastDirection.Vanilla)
            {
                return;
            }
            if(__result == Rot4.Invalid)
            {
                return;
            }
            switch(MapDesignerMod.mod.settings.coastDir)
            {
                case CoastDirection.North:
                    __result = Rot4.North;
                    break;
                case CoastDirection.East:
                    __result = Rot4.East;
                    break;
                case CoastDirection.South:
                    __result = Rot4.South;
                    break;
                case CoastDirection.West:
                    __result = Rot4.West;
                    break;
            }
        }
    }
}
