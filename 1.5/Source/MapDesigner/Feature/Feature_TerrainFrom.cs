using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using MapDesigner;

namespace MapDesigner.Feature
{
    /*
         if fert less than -2000, feature terrain overrides rivers
         else if fert less than -1000, rivers override feature
         else if fert >= -999, no feature terrain
         Terrains:
         000 deep water
         010 deep ocean water
         020 shallow water
         030 shallow ocean water
         040 marshy soil
         050 mud
         060 ice
         070 sand
         080 soil
         090 rich soil
    */
    [HarmonyPatch(typeof(RimWorld.GenStep_Terrain), "TerrainFrom")]
    [HarmonyBefore("GeologicalLandforms.Main")]
    public static class Feature_TerrainFrom
    {
        static bool Prefix(IntVec3 c, Map map, RiverMaker river, float fertility, ref TerrainDef __result)
        {
            if(fertility >= -1000)
            {
                return true;
            }
            if (fertility < -2000)
            {
                __result = TerrainFromValue(fertility);
                return false;
            }
            else if (river?.TerrainAt(c, true) == null)
            {
                __result = TerrainFromValue(fertility);
                return false;
            }

            return true;
        }

        public static float ValueFromTerrain(string terrain, bool riverOverride = false)
        {
            float fert = -1005;

            switch (terrain)
            {
                case "WaterDeep":
                    fert = -1005;
                    break;
                case "WaterOceanDeep":
                    fert = -1015;
                    break;
                case "WaterShallow":
                    fert = -1025;
                    break;
                case "WaterOceanShallow":
                    fert = -1035;
                    break;
                case "MarshyTerrain":
                    fert = -1045;
                    break;
                case "Mud":
                    fert = -1055;
                    break;
                case "Ice":
                    fert = -1065;
                    break;
                case "Sand":
                    fert = -1075;
                    break;
                case "Soil":
                    fert = -1085;
                    break;
                case "SoilRich":
                    fert = -1095;
                    break;
            }

            if (riverOverride)
            {
                fert -= 1000f;
            }

            return fert;
        }

        public static TerrainDef TerrainFromValue(float fert)
        {
            TerrainDef terr = TerrainDefOf.Sand;

            if (fert < -2000)
            {
                fert += 1000;
            }

            if (fert < -1000)
            {
                terr = TerrainDefOf.WaterDeep;
            }
            if (fert < -1010)
            {
                terr = TerrainDefOf.WaterOceanDeep;
            }
            if (fert < -1020)
            {
                terr = TerrainDefOf.WaterShallow;
            }
            if (fert < -1030)
            {
                terr = TerrainDefOf.WaterOceanShallow;
            }
            if (fert < -1040)
            {
                terr = TerrainDef.Named("MarshyTerrain");
            }
            if (fert < -1050)
            {
                terr = TerrainDef.Named("Mud");
            }
            if (fert < -1060)
            {
                terr = TerrainDefOf.Ice;
            }
            if (fert < -1070)
            {
                terr = TerrainDefOf.Sand;
            }
            if (fert < -1080)
            {
                terr = TerrainDefOf.Soil;
            }
            if (fert < -1090)
            {
                terr = TerrainDef.Named("SoilRich");
            }
            return terr;
        }
    }

}
