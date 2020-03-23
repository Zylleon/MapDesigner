using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDesigner
{
    public class BiomeDefault
    {
        public float animalDensity;

        public float plantDensity;
        public float wildPlantRegrowDays;
        public List<TerrainThreshold> terrainsByFertility = new List<TerrainThreshold>();
        public List<TerrainPatchMaker> terrainPatchMakers = new List<TerrainPatchMaker>();
    }
}
