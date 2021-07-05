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
    public class MapDesignerMod : Mod
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
        private MapDesignerMod.InfoCardTab tab;

        private Vector2 scrollPosition = Vector2.zero;

        public MapDesignerSettings settings;
        public static MapDesignerMod mod;

        public MapDesignerMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<MapDesignerSettings>();
            mod = this;
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

            TabRecord generalTab = new TabRecord("ZMD_generalTab".Translate(), delegate
            {
                this.tab = MapDesignerMod.InfoCardTab.General;
            }, this.tab == MapDesignerMod.InfoCardTab.General);
            list.Add(generalTab);

            TabRecord mountainTab = new TabRecord("ZMD_mountainTab".Translate(), delegate
            {
                this.tab = MapDesignerMod.InfoCardTab.Mountains;
            }, this.tab == MapDesignerMod.InfoCardTab.Mountains);
            list.Add(mountainTab);

            TabRecord terrainTab = new TabRecord("ZMD_terrainTab".Translate(), delegate
            {
                this.tab = MapDesignerMod.InfoCardTab.Terrain;
            }, this.tab == MapDesignerMod.InfoCardTab.Terrain);
            list.Add(terrainTab);

            TabRecord ThingsTab = new TabRecord("ZMD_thingsTab".Translate(), delegate
            {
                this.tab = MapDesignerMod.InfoCardTab.Things;
            }, this.tab == MapDesignerMod.InfoCardTab.Things);
            list.Add(ThingsTab);

            TabRecord riverTab = new TabRecord("ZMD_riverTab".Translate(), delegate
            {
                this.tab = MapDesignerMod.InfoCardTab.Rivers;
            }, this.tab == MapDesignerMod.InfoCardTab.Rivers);
            list.Add(riverTab);

            TabRecord featureTab = new TabRecord("ZMD_featureTab".Translate(), delegate
            {
                this.tab = MapDesignerMod.InfoCardTab.Feature;
            }, this.tab == MapDesignerMod.InfoCardTab.Feature);
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
                case MapDesignerMod.InfoCardTab.General:
                    UI.GeneralCard.DrawGeneralCard(cardRect);
                    break;
                case MapDesignerMod.InfoCardTab.Mountains:
                    UI.MountainCard.DrawMountainCard(cardRect);
                    break;
                case MapDesignerMod.InfoCardTab.Terrain:
                    UI.TerrainCard.DrawTerrainCard(cardRect);
                    break;
                case MapDesignerMod.InfoCardTab.Things:
                    UI.ThingsCard.DrawThingsCard(cardRect);
                    break;
                case MapDesignerMod.InfoCardTab.Rivers:
                    UI.RiversCard.DrawRiversCard(cardRect);
                    break;
                case MapDesignerMod.InfoCardTab.Feature:
                    UI.FeatureCard.DrawFeaturesCard(cardRect);
                    break;
                //case MapDesigner_Mod.InfoCardTab.Beta:
                //    UI.TerrainCardUtility.DrawBetaCard(cardRect);
                //    break;
                default:
                    tab = MapDesignerMod.InfoCardTab.General;
                    //UI.GeneralCardUtility.DrawGeneralCard(cardRect);
                    break;


            }
        }

    }

}
