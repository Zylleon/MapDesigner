using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using MapReroll;
using MapDesigner.Feature;

namespace MapDesigner.Patches
{
    /// <summary>
    /// Splitting feature grids
    /// </summary>
    //[HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility), "Generate")]
    //static class FeatureElevation
    //{
    //    [HarmonyPriority(500)]
    //    static void Postfix(Map map, GenStepParams parms)
    //    {
    //        if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.None)
    //        {
    //            return;
    //        }
    //        else if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.Lake)
    //        {
    //            Log.Message("[Map Desigher] Filling lakes...");
    //            new Lake().Generate(map, parms);
    //            return;
    //        }

    //        else if (MapDesignerMod.mod.settings.selectedFeature == MapDesignerSettings.Features.RoundIsland)
    //        {
    //            if (map.Biome.defName.Contains("BiomesIsland"))
    //            {
    //                Log.Message("Can't make round islands, this is already an island!");
    //                return;
    //            }

    //            Log.Message("[Map Desigher] Carving islands...");

    //            new RoundIsland().Generate(map, parms);
    //            //RoundIsland.AdjustMapGrids(map);

    //            return;
    //        }

    //    }
    //}
}
