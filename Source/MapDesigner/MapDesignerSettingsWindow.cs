﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace MapDesigner
{
    public class MapDesigner_Mod : Mod
    {
        private enum InfoCardTab : byte
        {
            General,
            Mountains,
            Things,
            //Rocks,
            Rivers,
            Feature,
        }
        private MapDesigner_Mod.InfoCardTab tab;

        private Vector2 scrollPosition = Vector2.zero;

        MapDesignerSettings settings;

        public MapDesigner_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MapDesignerSettings>();
        }

        public override string SettingsCategory()
        {
            return "ZMD_ModName".Translate();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            HelperMethods.ApplyBiomeSettings();
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            Rect rect3 = new Rect(inRect);

            List<TabRecord> list = new List<TabRecord>();

            //TabRecord generaltab = new TabRecord("ZMD_generalTab".Translate(), delegate
            //{
            //    this.tab = MapDesigner_Mod.InfoCardTab.General;
            //}, this.tab == MapDesigner_Mod.InfoCardTab.General);
            //list.Add(generaltab);

            TabRecord mountainTab = new TabRecord("ZMD_mountainTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Mountains;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Mountains);
            list.Add(mountainTab);

            TabRecord ThingsTab = new TabRecord("ZMD_thingsTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Things;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Things);
            list.Add(ThingsTab);

            //TabRecord rockTab = new TabRecord("ZMD_rocksTab".Translate(), delegate
            //{
            //    this.tab = MapDesigner_Mod.InfoCardTab.Rocks;
            //}, this.tab == MapDesigner_Mod.InfoCardTab.Rocks);
            //list.Add(rockTab);

            TabRecord riverTab = new TabRecord("ZMD_riverTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Rivers;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Rivers);
            list.Add(riverTab);

            TabRecord featureTab = new TabRecord("ZMD_featureTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Feature;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Feature);
            list.Add(featureTab);

            TabDrawer.DrawTabs(rect3, list, 150f);
            this.FillCard(rect3.ContractedBy(18f));

            listingStandard.End();

        }


        protected void FillCard(Rect cardRect)
        {
            cardRect.height -= 20f;
            switch(tab)
            {
                case MapDesigner_Mod.InfoCardTab.Mountains:
                    UI.MountainCardUtility.DrawMountainCard(cardRect);
                    break;
                case MapDesigner_Mod.InfoCardTab.Things:
                    UI.ThingsCardUtility.DrawThingsCard(cardRect);
                    break;
                //case MapDesigner_Mod.InfoCardTab.Rocks:
                //    UI.RocksCardUtility.DrawRocksCard(cardRect);
                //    break;
                case MapDesigner_Mod.InfoCardTab.Rivers:
                    UI.RiversCardUtility.DrawRiversCard(cardRect);
                    break;
                case MapDesigner_Mod.InfoCardTab.Feature:
                    UI.FeatureCardUtility.DrawFeaturesCard(cardRect);
                    break;
                default:
                    tab = MapDesigner_Mod.InfoCardTab.Mountains;
                    break;
            }
        }

    }

}