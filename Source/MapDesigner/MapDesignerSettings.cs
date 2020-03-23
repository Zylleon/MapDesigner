using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;


namespace MapDesigner
{

    public class MapDesignerSettings : ModSettings
    {
        public float hillAmount = 1.0f;
        public float hillSize = 0.021f;
        public float hillSmoothness = 2.0f;

        public static bool flagCaves = true;
        public float densityPlant = 1.0f;
        public float densityAnimal = 1.0f;


        public Dictionary<string, BiomeDefault> biomeDefaults;

        //public Dictionary<string, BiomeDefault> BiomeDefaults
        //{
        //    get
        //    {
        //        if (biomeDefaults == null)
        //        {
        //            biomeDefaults = new Dictionary<string, BiomeDefault>();

        //            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
        //            {
        //                BiomeDefault biodef = new BiomeDefault();
        //                biodef.animalDensity = biome.animalDensity;
        //                biodef.plantDensity = biome.plantDensity;
        //                biodef.wildPlantRegrowDays = biome.wildPlantRegrowDays;
        //                biodef.terrainsByFertility = biome.terrainsByFertility;
        //                biodef.terrainPatchMakers = biome.terrainPatchMakers;

        //                biomeDefaults.Add(biome.defName, biodef);
        //            }
        //        }

        //        return biomeDefaults;
        //    }
        //}


        public override void ExposeData()
        {
            Scribe_Values.Look(ref hillAmount, "hillAmount", 1.0f);
            Scribe_Values.Look(ref hillSize, "hillSize", 0.021f);
            Scribe_Values.Look(ref hillSmoothness, "hillSmoothness", 2.0f);

            Scribe_Values.Look(ref flagCaves, "flagCaves", true);
            Scribe_Values.Look(ref densityPlant, "densityPlant", 1.0f);
            Scribe_Values.Look(ref densityAnimal, "densityAnimal", 1.0f);
        }
    }


    public class MapDesigner_Mod : Mod
    {
        MapDesignerSettings settings;

        public MapDesigner_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MapDesignerSettings>();
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {
            //inRect.width = 450f;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            int hillAmountLabel = GetHillAmountLabel(settings.hillAmount);
            listingStandard.Label(("ZMD_hillAmount" + hillAmountLabel).Translate());
            settings.hillAmount = listingStandard.Slider(settings.hillAmount, 0f, 2.5f);

            listingStandard.Label(("ZMD_hillSize" + GetHillSizeLabel(settings.hillSize)).Translate());
            settings.hillSize = listingStandard.Slider(settings.hillSize, 0.01f, 0.10f);

            listingStandard.Label(("ZMD_hillSmoothness" + GetHillSmoothnesstLabel(settings.hillSmoothness)).Translate());
            settings.hillSmoothness = listingStandard.Slider(settings.hillSmoothness, 0f, 5f);

            listingStandard.CheckboxLabeled("ZMD_flagCaves".Translate(), ref MapDesignerSettings.flagCaves, "ZMD_FlagCavesTooltip".Translate());

            listingStandard.GapLine();

            listingStandard.Label(String.Format("{0} : {1}","ZMD_densityPlant".Translate(), ("ZMD_scale" + GetDensityLabel(settings.densityPlant)).Translate()));
            settings.densityPlant = listingStandard.Slider(settings.densityPlant, 0f, 2.5f);

            listingStandard.Label(String.Format("{0} : {1}", "ZMD_densityAnimal".Translate(), ("ZMD_scale" + GetDensityLabel(settings.densityAnimal)).Translate()));
            settings.densityAnimal = listingStandard.Slider(settings.densityAnimal, 0f, 2.5f);

            if(listingStandard.ButtonText("ZMD_Reset".Translate()))
            {
                ResetAllSettings();
            }


            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            HelperMethods.ApplyBiomeSettings();
        }

        public override string SettingsCategory()
        {
            return "ZMD_ModName".Translate();
        }

        
        private void ResetAllSettings()
        {
            settings.hillAmount = 1.0f;
            settings.hillSize = 0.021f;
            settings.hillSmoothness = 2.0f;

            MapDesignerSettings.flagCaves = true;
            settings.densityPlant = 1.0f;
            settings.densityAnimal = 1.0f;
        }

        #region labels

        private static int GetDensityLabel(float density)
        {
            int output = 0;
            if(density > 0.1f) //very low
            {
                output ++;
            }
            if (density > 0.4f)  //low
            {
                output++;
            }
            if (density > 0.85f)  //average
            {
                output++;
            }
            if (density > 1.2f) //high
            {
                output++;
            }
            if (density > 1.7f) //very high
            {
                output++;
            }
            if (density > 2.3f) //extremely high
            {
                output++;
            }
            return output;
        }




        private static int GetHillSizeLabel(float hillSize)
        {
            int label = 0;

            if(hillSize > 0.013f)   // vanilla
            {
                label++;
            }
            if (hillSize > 0.033f)  // small
            {
                label++;
            }
            if(hillSize > 0.055f)    // tiny
            {
                label++;
            }
            if (hillSize > 0.085f)   // very tiny
            {
                label++;
            }

            return label;

        }

        private static int GetHillAmountLabel(float hillAmount)
        {
            int label = 0;
            if(hillAmount > 0.5f)
            {
                label++;
            }
            if (hillAmount > 0.8f)
            {
                label++;
            }
            if (hillAmount > 1.2f)
            {
                label++;
            }
            if (hillAmount > 1.5f)
            {
                label++;
            }
            if (hillAmount > 1.75f)
            {
                label++;
            }
            return label;
            //return 3;
        }

        private static int GetHillSmoothnesstLabel(float hillSmoothness)
        {
            int label = 0;
            if (hillSmoothness > 0.8f)
            {
                label++;
            }
            if (hillSmoothness > 1.5f)
            {
                label++;
            }
            if (hillSmoothness > 2.5f)
            {
                label++;
            }
            if (hillSmoothness > 3.8f)
            {
                label++;
            }
            return label;
        }
       
        
        


        
        #endregion
    }
}
