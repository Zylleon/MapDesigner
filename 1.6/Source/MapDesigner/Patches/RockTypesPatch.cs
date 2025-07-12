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

    /*
     * TOOD: Re-enable rock types picker
    [HarmonyPatch(typeof(RimWorld.Planet.World))]
    [HarmonyPatch(nameof(RimWorld.Planet.World.NaturalRockTypesIn))]
    internal static class RockTypesPatch
    {
        static void Finalizer(int tile, ref IEnumerable<ThingDef> __result)
        {
            Rand.PushState();
            Rand.Seed = tile;
            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            IntRange rockTypeRange = settings.rockTypeRange;

            int num = Rand.RangeInclusive(rockTypeRange.min, rockTypeRange.max);

            // If it's a modded biome with special stone, check that the user allows this to change before continuing
            if (__result.Count() == 1 && !settings.flagBiomeRocks)
            {
                Rand.PopState();
                return;
            }

            List<ThingDef> tileRocks = __result.ToList();

            //List<ThingDef> list = (from d in DefDatabase<ThingDef>.AllDefs
            //                       where d.category == ThingCategory.Building && d.building.isNaturalRock && !d.building.isResourceRock && !d.IsSmoothed 
            //                       select d).ToList<ThingDef>();

            List<ThingDef> list = HelperMethods.GetRockList();

            List<ThingDef> allowedRocks = list.Where(t => settings.allowedRocks[t.defName]).ToList();

            // if nothing is selected, all rock types are allowed
            if(allowedRocks.Count == 0)
            {
                allowedRocks = list;
            }

            tileRocks.RemoveAll(t => !allowedRocks.Contains(t));
            allowedRocks.RemoveAll(t => tileRocks.Contains(t));

            // shorten if the list is already long enough
            if (tileRocks.Count() >= num)
            {
                __result = tileRocks.Take(num);
            }
            else
            {
                while (tileRocks.Count() < num && allowedRocks.Count() > 0)
                {
                    ThingDef item = allowedRocks.RandomElement<ThingDef>();
                    allowedRocks.Remove(item);
                    tileRocks.Add(item);
                }
                __result = tileRocks.ToList();
            }

            Rand.PopState();

        }
    }

    */
}
