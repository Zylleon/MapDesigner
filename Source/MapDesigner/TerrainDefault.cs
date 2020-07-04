using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDesigner
{
    public class TerrainDefault
    {
        public List<TerrainThreshold> terrainsByFertility = new List<TerrainThreshold>();
        public List<TerrainPatchMaker> terrainPatchMakers = new List<TerrainPatchMaker>();
    }


    public class TBF
    {
        public TerrainThreshold thresh;
        public int fertRank;
        public float size;

    }
}
