using System;
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
            Terrain,
            Things,
            Rivers,
            Feature,
            Beta,
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

            //TabRecord generalTab = new TabRecord("ZMD_generalTab".Translate(), delegate
            //{
            //    this.tab = MapDesigner_Mod.InfoCardTab.General;
            //}, this.tab == MapDesigner_Mod.InfoCardTab.General);
            //list.Add(generalTab);

            TabRecord mountainTab = new TabRecord("ZMD_mountainTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Mountains;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Mountains);
            list.Add(mountainTab);

            TabRecord terrainTab = new TabRecord("ZMD_terrainTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Terrain;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Terrain);
            list.Add(terrainTab);

            TabRecord ThingsTab = new TabRecord("ZMD_thingsTab".Translate(), delegate
            {
                this.tab = MapDesigner_Mod.InfoCardTab.Things;
            }, this.tab == MapDesigner_Mod.InfoCardTab.Things);
            list.Add(ThingsTab);

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

            //if(Prefs.DevMode)
            //{
            //    TabRecord betaTab = new TabRecord("ZMD_betaTab".Translate(), delegate
            //    {
            //        this.tab = MapDesigner_Mod.InfoCardTab.Beta;
            //    }, this.tab == MapDesigner_Mod.InfoCardTab.Beta);
            //    list.Add(betaTab);
            //}

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
                case MapDesigner_Mod.InfoCardTab.Terrain:
                    UI.TerrainCardUtility.DrawTerrainCard(cardRect);
                    break;
                case MapDesigner_Mod.InfoCardTab.Things:
                    UI.ThingsCardUtility.DrawThingsCard(cardRect);
                    break;
                case MapDesigner_Mod.InfoCardTab.Rivers:
                    UI.RiversCardUtility.DrawRiversCard(cardRect);
                    break;
                case MapDesigner_Mod.InfoCardTab.Feature:
                    UI.FeatureCardUtility.DrawFeaturesCard(cardRect);
                    break;
                //case MapDesigner_Mod.InfoCardTab.Beta:
                //    UI.TerrainCardUtility.DrawBetaCard(cardRect);
                //    break;
                default:
                    tab = MapDesigner_Mod.InfoCardTab.Mountains;
                    //UI.GeneralCardUtility.DrawGeneralCard(cardRect);
                    break;


            }
        }

    }

}
