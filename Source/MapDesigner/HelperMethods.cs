using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MapDesigner
{

    public static class HelperMethods
    {
        public static float GetHillSize()
        {
            return LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSize;
        }

        public static float GetHillSmoothness()
        {
            return LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSmoothness;
        }


        public static void InitBiomeDefaults()
        {
            Dictionary<string, BiomeDefault>  biomeDefaults = new Dictionary<string, BiomeDefault>();

            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                BiomeDefault biodef = new BiomeDefault();
                biodef.animalDensity = biome.animalDensity;
                biodef.plantDensity = biome.plantDensity;
                biodef.wildPlantRegrowDays = biome.wildPlantRegrowDays;

                biomeDefaults.Add(biome.defName, biodef);
            }

            LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().biomeDefaults = biomeDefaults;

            Dictionary<string, FloatRange> densityDefaults = new Dictionary<string, FloatRange>();

            GenStepDef step = DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple");
            densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

            step = DefDatabase<GenStepDef>.GetNamed("SteamGeysers");
            densityDefaults.Add(step.defName, (step.genStep as GenStep_Scatterer).countPer10kCellsRange);

            LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityDefaults = densityDefaults;
        }


        public static void ApplyBiomeSettings()
        {
            // densities
            Dictionary<string, BiomeDefault> biomeDefaults = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().biomeDefaults;

            float densityPlant = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityPlant;
            float densityAnimal = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityAnimal;

            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                biome.animalDensity = biomeDefaults[biome.defName].animalDensity * densityAnimal;

                biome.plantDensity = biomeDefaults[biome.defName].plantDensity * densityPlant;

                if (biome.plantDensity > 1f)
                {
                    biome.wildPlantRegrowDays = biomeDefaults[biome.defName].wildPlantRegrowDays / biome.plantDensity;
                    biome.plantDensity = 1f;
                }
            }

            // geysers and ruins
            Dictionary<string, FloatRange> densityDefaults = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityDefaults;
            float densityRuins = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityRuins;
            if (densityRuins > 1)
            {
                densityRuins = (float)Math.Pow(densityRuins, 3);
            }
            (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterRuinsSimple"].min * densityRuins;
            (DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterRuinsSimple"].max * densityRuins;

            float densityGeyser = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityGeyser;

            (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.min = densityDefaults["ScatterRuinsSimple"].min * densityGeyser;
            (DefDatabase<GenStepDef>.GetNamed("SteamGeysers").genStep as GenStep_Scatterer).countPer10kCellsRange.max = densityDefaults["ScatterRuinsSimple"].max * densityGeyser;


            //rivers

            float sizeRiver = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().sizeRiver;
            float widthOnMap = 6;
            foreach (RiverDef river in DefDatabase<RiverDef>.AllDefs)
            {
               switch(river.defName)
               {
                    case "HugeRiver":
                        widthOnMap = 30f;
                        break;
                    case "LargeRiver":
                        widthOnMap = 14f;
                        break;
                    case "River":
                        widthOnMap = 6f;
                        break;
                    case "Creek":
                        widthOnMap = 4f;
                        break;
               }

                river.widthOnMap = widthOnMap * sizeRiver;

            }
        }

    }
}
