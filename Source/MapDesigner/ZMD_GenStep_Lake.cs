using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


namespace MapDesigner
{
    public class ZMD_GenStep_Lake : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791; 
            }
        }

        public ZMD_LakeSettings settings;

        public override void Generate(Map map, GenStepParams parms)
        {
            settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<ZMD_LakeSettings>();


            //settings =
            //float densityPlant = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().densityPlant;
        }

    }
}
