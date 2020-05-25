using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace MapDesigner
{
    public class ZMD_LakeSettings : ModSettings
    {
        public float lakeSize = 10f;
        public float lakeHor = 0.5f;
        public float lakeVert = 0.5f;


        public override void ExposeData()
        {
            Scribe_Values.Look(ref lakeVert, "lakeSize", 10f);
            Scribe_Values.Look(ref lakeHor, "lakeHor", 0.5f);
            Scribe_Values.Look(ref lakeVert, "lakeVert", 0.5f);
        }


        //public IEnumerable<Widgets.DropdownMenuElement


    }
}
