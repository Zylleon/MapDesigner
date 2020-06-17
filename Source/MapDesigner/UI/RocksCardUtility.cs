using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;


namespace MapDesigner.UI
{
    public class RocksCardUtility
    {
        public MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

        private Vector2 scrollPosition;
        private bool testBool = true;

        public void DrawRocksCard(Rect rect)
        {


            Rect test = rect.ContractedBy(4f);

            Rect viewRect = new Rect(0f, 0f, test.width - 16f, 900f);

            Widgets.BeginScrollView(test, ref scrollPosition, viewRect, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            listing.Label("ZMD_rocksTabInfo".Translate());

            List<ThingDef> rockList = (from d in DefDatabase<ThingDef>.AllDefs
                                   where d.category == ThingCategory.Building && d.building.isNaturalRock && !d.building.isResourceRock && !d.IsSmoothed
                                   select d).ToList<ThingDef>();
            foreach (ThingDef rock in rockList)
            {
                listing.CheckboxLabeled(rock.label, ref testBool);
            }




            listing.End();

            viewRect.height = listing.CurHeight;

            Widgets.EndScrollView();
        }


    }
}
