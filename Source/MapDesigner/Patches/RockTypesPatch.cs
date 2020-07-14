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
    [HarmonyPatch(typeof(RimWorld.Planet.World))]
    [HarmonyPatch(nameof(RimWorld.Planet.World.NaturalRockTypesIn))]
    internal static class RockTypesPatch
    {
        static void Finalizer(int tile, ref IEnumerable<ThingDef> __result)
        {
            Rand.PushState();
            Rand.Seed = tile;
            //IntRange rockTypeRange = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().rockTypeRange;

            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();
            IntRange rockTypeRange = settings.rockTypeRange;

            int num = Rand.RangeInclusive(rockTypeRange.min, rockTypeRange.max);

            // If it's a modded biome with special stone, check that the user allows this to change before continuing
            if (__result.Count() == 1 && !MapDesignerSettings.flagBiomeRocks)
            {
                Rand.PopState();
                return;
            }

            List<ThingDef> rocks = __result.ToList();

            List<ThingDef> list = (from d in DefDatabase<ThingDef>.AllDefs
                                   where d.category == ThingCategory.Building && d.building.isNaturalRock && !d.building.isResourceRock && !d.IsSmoothed && !rocks.Contains(d)
                                   select d).ToList<ThingDef>();


            // shorten if the list is already long enough
            if (__result.Count() >= num)
            {
                __result = __result.Take(num);
            }
            else
            {
                
                while (rocks.Count() < num && list.Count() > 0)
                {
                    ThingDef item = list.RandomElement<ThingDef>();
                    list.Remove(item);
                    rocks.Add(item);
                }
                __result = rocks.ToList();
            }

            Rand.PopState();

        }
    }


}
