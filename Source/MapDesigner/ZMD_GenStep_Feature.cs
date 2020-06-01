using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace MapDesigner
{
    public class ZMD_GenStep_Feature : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1427794901;
            }
        }


        public override void Generate(Map map, GenStepParams parms)
        {
            Log.Message("Genning features");
            (new ZMD_GenStep_Lake()).Generate(map, parms);
           
        }

    }
}
