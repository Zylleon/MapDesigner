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
    /*
    [HarmonyPatch(typeof(RimWorld.GenStep_Terrain), "GenerateRiverLookupTexture ")]
    static class River_ForkFlow
    {
        static bool Prefix(Map map, RiverMaker riverMaker)
        {
			if(true)
            {
				ForkedRiverTexture.GenerateForkedRiverTexture(map, riverMaker);
				return false;
			}

            return true;
        }
	}
    */



	

    //[HarmonyPatch(typeof(RimWorld.RiverMaker), "WaterCoordinateAt")]
    static class River_Flow_Patch
    {
        static bool Prefix(IntVec3 loc, ref Vector3 __result, ModuleBase ___coordinateX, ModuleBase ___coordinateZ)
        {

            if (MapDesignerMod.mod.settings.selRiverStyle == MapDesignerSettings.RiverStyle.Confluence)
            {
                //__result = new Vector3(___coordinateX.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));
                //__result = new Vector3(___coordinateX.GetValue(loc), 0f, Math.Abs(___coordinateZ.GetValue(loc)));
                //__result = new Vector3(___coordinateZ.GetValue(loc) + ___coordinateX.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));


                // these return the correct angles for the L and R branches of a forked river
                //__result = new Vector3(___coordinateX.GetValue(loc) + 0.6f * ___coordinateZ.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));
                //__result = new Vector3(___coordinateX.GetValue(loc) - 0.6f * ___coordinateZ.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));


                #region messy confluence
                // this makes debug data in approx. the right shape for a confluence, but the forked part is messy

                //if (___coordinateZ.GetValue(loc) > 0f)
                //{
                //    __result = new Vector3(___coordinateX.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));
                //}

                //else
                //{
                //    if (___coordinateX.GetValue(loc) > 0f)
                //    {
                //        __result = new Vector3(___coordinateX.GetValue(loc) + 0.6f * ___coordinateZ.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));
                //    }
                //    else
                //    {
                //        __result = new Vector3(___coordinateX.GetValue(loc) - 0.6f * ___coordinateZ.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));

                //    }
                //}

                #endregion


                #region messy confluence 2
                // this makes debug data in approx. the right shape for a confluence, but the forked part is messy
                if (___coordinateZ.GetValue(loc) > 0f)
                {
                    __result = new Vector3(___coordinateX.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));
                }

                else
                {
                    if (___coordinateX.GetValue(loc) > 0f)
                    {
                        __result = new Vector3(___coordinateX.GetValue(loc) + 0.6f * ___coordinateZ.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));
                    }
                    else
                    {
                        __result = new Vector3(___coordinateX.GetValue(loc) - 0.6f * ___coordinateZ.GetValue(loc), 0f, ___coordinateZ.GetValue(loc));

                    }
                }

                #endregion

                return false;

            }

            return true;
        }



    }
  
}
