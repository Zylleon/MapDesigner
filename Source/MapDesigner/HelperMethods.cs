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
    }
}
