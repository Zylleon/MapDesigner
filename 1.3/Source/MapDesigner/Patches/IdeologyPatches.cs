using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDesigner.Patches
{
    static class IdeologyPatch
    {
        static bool RoadDebris()
        {
            return MapDesignerMod.mod.settings.flagRoadDebris;
        }

        static bool CaveDebris()
        {
            return MapDesignerMod.mod.settings.flagCaveDebris;
        }

        static bool AncientUtilityBuilding()
        {
            return MapDesignerMod.mod.settings.flagAncientUtilityBuilding;
        }

        static bool AncientTurret()
        {
            return MapDesignerMod.mod.settings.flagAncientTurret;
        }

        static bool AncientMechs()
        {
            return MapDesignerMod.mod.settings.flagAncientLandingPad;
        }

        static bool AncientLandingPad()
        {
            return MapDesignerMod.mod.settings.flagAncientLandingPad;
        }

        static bool AncientFences()
        {
            return MapDesignerMod.mod.settings.flagAncientFences;
        }
    }
}
