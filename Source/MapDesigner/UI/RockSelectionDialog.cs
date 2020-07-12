using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace MapDesigner.UI
{
    public class RockSelectionDialog : Window
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public RockSelectionDialog()
        {
            //this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
            this.closeOnAccept = false;
            this.optionalTitle = "ZMD_chooseRockTypes".Translate();
            this.doCloseX = true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            MapDesignerSettings settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();
            List<ThingDef> list = (from d in DefDatabase<ThingDef>.AllDefs
                                   where d.category == ThingCategory.Building && d.building.isNaturalRock && !d.building.isResourceRock && !d.IsSmoothed
                                   select d).ToList<ThingDef>();

            //Rect viewRect = new Rect(0f, 0f, inRect.width - 18f, 100 + 24 * list.Count());
            //Rect viewRect = new Rect(0f, 0f, inRect.width - 38f, 100 + 24 * list.Count());

            //Rect viewRect = inRect.ContractedBy(4f);
            Rect viewRect = inRect.ContractedBy(4f);

            viewRect.width = 200f;
            viewRect.height = 24 * list.Count();

            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect, true);

            Listing_Standard listing = new Listing_Standard();
            //listing.ColumnWidth = 200f;
            listing.Begin(viewRect);

            foreach(ThingDef rock in list)
            {
                bool allowed = settings.allowedRocks[rock.defName];
                listing.CheckboxLabeled(rock.label, ref allowed);
                settings.allowedRocks[rock.defName] = allowed;
            }

            listing.End();

            Widgets.EndScrollView();

        }

    }
}
