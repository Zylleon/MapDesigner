/*
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MapDesigner
{
    public class SettingsTabWindow : Mod
    {
        private enum InfoCardTab : byte
        {
            Mountains,
            Misc,
            Feature,
            //Records
        }

        MapDesignerSettings settings;

        public SettingsTabWindow(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MapDesignerSettings>();
        }

        private SettingsTabWindow.InfoCardTab tab;

        //private void Setup()
        //{
        //    this.forcePause = true;
        //    this.doCloseButton = true;
        //    this.doCloseX = true;
        //    this.absorbInputAroundWindow = true;
        //    this.closeOnClickedOutside = true;
        //    this.soundAppear = SoundDefOf.InfoCard_Open;
        //    this.soundClose = SoundDefOf.InfoCard_Close;
        //}

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect);
            rect = rect.ContractedBy(18f);
            rect.height = 34f;
            Text.Font = GameFont.Medium;
            //Widgets.Label(rect, this.GetTitle());
            Rect rect2 = new Rect(inRect);
            rect2.yMin = rect.yMax;
            rect2.yMax -= 38f;
            Rect rect3 = rect2;
            rect3.yMin += 45f;
            List<TabRecord> list = new List<TabRecord>();
            TabRecord mountainTab = new TabRecord("ZMD_MountainTab".Translate(), delegate
            {
                this.tab = SettingsTabWindow.InfoCardTab.Mountains;
            }, this.tab == SettingsTabWindow.InfoCardTab.Mountains);
            list.Add(mountainTab);

            TabRecord miscTab = new TabRecord("ZMD_MiscTab".Translate(), delegate
            {
                this.tab = SettingsTabWindow.InfoCardTab.Mountains;
            }, this.tab == SettingsTabWindow.InfoCardTab.Mountains);
            list.Add(miscTab);

            TabRecord featureTab = new TabRecord("ZMD_FeatureTab".Translate(), delegate
            {
                this.tab = SettingsTabWindow.InfoCardTab.Mountains;
            }, this.tab == SettingsTabWindow.InfoCardTab.Mountains);
            list.Add(featureTab);

            TabRecord betaTab = new TabRecord("ZMD_BetaTab".Translate(), delegate
            {
                this.tab = SettingsTabWindow.InfoCardTab.Mountains;
            }, this.tab == SettingsTabWindow.InfoCardTab.Mountains);
            list.Add(betaTab);

            TabDrawer.DrawTabs(rect3, list, 200f);
            this.FillCard(rect3.ContractedBy(18f));
        }

        public void Tab_Mountain(Rect inRect)
        {

        }

        public void Tab_Misc(Rect inRect)
        {

        }

        public void Tab_Feature(Rect inRect)
        {

        }


        protected void FillCard(Rect cardRect)
        {
            if (this.tab == SettingsTabWindow.InfoCardTab.Mountains)
            {
                if (this.thing != null)
                {
                    Thing innerThing = this.thing;
                    MinifiedThing minifiedThing = this.thing as MinifiedThing;
                    if (minifiedThing != null)
                    {
                        innerThing = minifiedThing.InnerThing;
                    }
                    StatsReportUtility.DrawStatsReport(cardRect, innerThing);
                }
                else if (this.worldObject != null)
                {
                    StatsReportUtility.DrawStatsReport(cardRect, this.worldObject);
                }
                else
                {
                    StatsReportUtility.DrawStatsReport(cardRect, this.def, this.stuff);
                }
            }
            else if (this.tab == SettingsTabWindow.InfoCardTab.Misc)
            {
                CharacterCardUtility.DrawCharacterCard(cardRect, (Pawn)this.thing, null, default(Rect));
            }
            else if (this.tab == SettingsTabWindow.InfoCardTab.Feature)
            {
                cardRect.yMin += 8f;
                HealthCardUtility.DrawPawnHealthCard(cardRect, (Pawn)this.thing, false, false, null);
            }
            else if (this.tab == SettingsTabWindow.InfoCardTab.Records)
            {
                RecordsCardUtility.DrawRecordsCard(cardRect, (Pawn)this.thing);
            }
        }
    }
}
*/