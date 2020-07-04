using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;


namespace MapDesigner.Patches
{

    [HarmonyPatch(typeof(RimWorld.GenStep_Terrain))]
    [HarmonyPatch(nameof(RimWorld.GenStep_Terrain.Generate))]
    static class TerrainSettingsPatch
    {
        static void Prefix(out TerrainDefault __state, Map map)
        {
            __state = new TerrainDefault()
            {
                terrainsByFertility = map.Biome.terrainsByFertility,
                terrainPatchMakers = map.Biome.terrainPatchMakers
            };
            if (!MapDesignerSettings.flagTerrain)
            {
                return;
            }
            Log.Message("running terrain patch");
            
            TerrainDefault newTerrains = HelperMethods.StretchTerrains(__state, map);

            map.Biome.terrainsByFertility = newTerrains.terrainsByFertility;
        }




        static void Postfix(TerrainDefault __state, Map map)
        {
            map.Biome.terrainsByFertility = __state.terrainsByFertility;
            map.Biome.terrainPatchMakers = __state.terrainPatchMakers;
        }



        static List<TerrainThreshold> StretchTerrains(List<TerrainThreshold> input, float amt, float min, float max)
        {
            List<TerrainThreshold> output = input;




            return output;
        }


        //protected class TerrainDefaults
        //{
        //    public List<TerrainThreshold> terrainsByFertility = new List<TerrainThreshold>();
        //    public List<TerrainPatchMaker> terrainPatchMakers = new List<TerrainPatchMaker>();
        //}


        //protected class TBF
        //{
        //    public TerrainThreshold thresh;
        //    public int fertRank;
        //    public float size;

        //}

    }

}
