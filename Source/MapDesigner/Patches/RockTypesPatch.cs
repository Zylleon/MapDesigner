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

    
     // TOOD: Re-enable rock types picker
    [HarmonyPatch(typeof(RimWorld.Planet.World))]
    [HarmonyPatch(nameof(RimWorld.Planet.World.NaturalRockTypesIn))]
    internal static class RockTypesPatch
    {
        static void Finalizer(PlanetTile tile, ref IEnumerable<ThingDef> __result)
        {
            Rand.PushState();
            Rand.Seed = tile.GetHashCode();
            MapDesignerSettings settings = MapDesignerMod.mod.settings;
            IntRange rockTypeRange = settings.rockTypeRange;

            int num = Rand.RangeInclusive(rockTypeRange.min, rockTypeRange.max);
            List<ThingDef> tileRocks = __result.ToList();
            List<ThingDef> list = HelperMethods.GetRockList();
            List<ThingDef> allowedRocks = list.Where(t => settings.allowedRocks[t.defName]).ToList();

            // remove biome-specific rocks from the list as appropriate
            if (!settings.flagBiomeRocks)
            {
                allowedRocks.RemoveAll(x => x.building.biomeSpecific && !tile.Tile?.PrimaryBiome?.extraRockTypes?.NotNullAndContains(x) == true);
            }

            // if nothing is selected, all rock types are allowed
            if (allowedRocks.Count == 0)
            {
                allowedRocks = list;
            }

            tileRocks.RemoveAll(t => !allowedRocks.Contains(t));
            allowedRocks.RemoveAll(t => tileRocks.Contains(t));

            // shorten if the list is too long
            if (tileRocks.Count() >= num)
            {
                __result = tileRocks.Take(num);
            }
            //pad if the list is too short
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

    
}
