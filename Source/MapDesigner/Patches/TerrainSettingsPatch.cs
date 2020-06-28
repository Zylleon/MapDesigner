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

    //[HarmonyPatch(typeof(RimWorld.GenStep_Terrain))]
    //[HarmonyPatch(nameof(RimWorld.GenStep_Terrain.Generate))]
    //static class TerrainSettingsPatch
    //{
    //    static void Prefix(out List<TerrainThreshold> __state, Map map)
    //    {
    //        __state = map.Biome.terrainsByFertility;
    //        List<TerrainThreshold> oldThreshes = map.Biome.terrainsByFertility;
    //        List<TerrainThreshold> newThreshes = new List<TerrainThreshold>();

    //        MapGenFloatGrid elevation = MapGenerator.Elevation;

    //        float min = 999f;
    //        float max = -999f;
    //        foreach (IntVec3 current in map.AllCells)
    //        {
    //            max = Math.Max(max, elevation[current]);
    //            min = Math.Min(min, elevation[current]);
    //        }
    //        float rangeSize = max - min;

    //        List<TBF> listTbf = new List<TBF>();

    //        oldThreshes.OrderBy(t => t.terrain.fertility);

    //        foreach (TerrainThreshold t in oldThreshes)
    //        {

    //        }








    //        map.Biome.terrainsByFertility = newThreshes;

    //    }




    //    static void Postfix(List<TerrainThreshold> __state, Map map)
    //    {
    //        map.Biome.terrainsByFertility = __state;

    //        Log.Message("First terrain " + map.Biome.terrainsByFertility.FirstOrDefault().terrain.defName);
    //    }



    //    private class TBF
    //    {
    //        TerrainThreshold thresh;
    //        int fertRank;
    //        float size;
    //        //float size = thresh.max - thresh.min;



    //    }

    //}


}
