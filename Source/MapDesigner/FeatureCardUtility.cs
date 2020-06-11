using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace MapDesigner
{
    public class FeatureCardUtility
    {
        public MapDesignerSettings settings;

        public void DrawFeatureOptions(Listing_Standard listing)
        {
            //this.settings = GetSettings<MapDesignerSettings>();
            this.settings = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>();

            MapDesignerSettings.Features selectedFeature = settings.selectedFeature;

            if(settings.selectedFeature == MapDesignerSettings.Features.None)
            {
                DrawNoOptions(listing);
            }
            else if (settings.selectedFeature == MapDesignerSettings.Features.RoundIsland)
            {
                DrawRoundIslandOptions(listing);
            }
            else if (settings.selectedFeature == MapDesignerSettings.Features.Lake)
            {
                DrawLakeOptions(listing);
            }

        }

        public void DrawNoOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featureNoneInfo".Translate());

        }

        public void DrawRoundIslandOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featurePRIInfo".Translate());

            listing.Label("ZMD_priIslandSizeLabel".Translate());
            this.settings.priIslandSize = listing.Slider(this.settings.priIslandSize, 5f, 45f);

            listing.Label("ZMD_priBeachSizeLabel".Translate());
            this.settings.priBeachSize = listing.Slider(this.settings.priBeachSize, 1f, 18f);

            listing.CheckboxLabeled("ZMD_priMultiSpawnLabel".Translate(), ref MapDesignerSettings.priMultiSpawn);
        }


        public void DrawLakeOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featureLakeInfo".Translate());

            listing.Label("ZMD_lakeSize".Translate());
            this.settings.lakeSize = listing.Slider(this.settings.lakeSize, 0.04f, 1.0f);

            listing.Label(lakeRoundnessLabel);
            this.settings.lakeRoundness = listing.Slider(this.settings.lakeRoundness, 0f, 3.5f);

            listing.Label("ZMD_lakeBeachSize".Translate());
            this.settings.lakeBeachSize = listing.Slider(this.settings.lakeBeachSize, 0f, 30f);

            listing.Label(lakeDepthLabel);
            this.settings.lakeDepth = listing.Slider(this.settings.lakeDepth, 0f, 1f);

            listing.CheckboxLabeled("ZMD_flagLakeSalty".Translate(), ref MapDesignerSettings.flagLakeSalty, "ZMD_flagLakeSalty".Translate());

            List<TerrainDef> shoreOptions = new List<TerrainDef>();

            shoreOptions.Add(TerrainDefOf.Soil);
            shoreOptions.Add(TerrainDef.Named("SoilRich"));
            shoreOptions.Add(TerrainDefOf.Sand);
            shoreOptions.Add(TerrainDef.Named("MarshyTerrain"));
            shoreOptions.Add(TerrainDef.Named("Mud"));
            shoreOptions.Add(TerrainDefOf.Ice);

            if (listing.ButtonTextLabeled("ZMD_lakeShore".Translate(), TerrainDef.Named(settings.lakeShore).label))
            {
                List<FloatMenuOption> shoreTerrList = new List<FloatMenuOption>();

                foreach (TerrainDef terr in shoreOptions)
                {
                    shoreTerrList.Add(new FloatMenuOption(terr.label, delegate { settings.lakeShore = terr.defName; }, MenuOptionPriority.Default));
                }

                Find.WindowStack.Add(new FloatMenu(shoreTerrList));
            }

        }

        #region labels

        public string GetFeatureLabel(MapDesignerSettings.Features feature)
        {
            if(feature == MapDesignerSettings.Features.None)
            {
                return "ZMD_featureNone".Translate();
            }
            if (feature == MapDesignerSettings.Features.RoundIsland)
            {
                return "ZMD_featurePRI".Translate();
            }
            if (feature == MapDesignerSettings.Features.Lake)
            {
                return "ZMD_featureLake".Translate();
            }
            return "ZMD_selectFeature".Translate();
        }

        private string lakeRoundnessLabel
        {
            get
            {
                int label = 0;
                if (settings.lakeRoundness > 0.3f)
                {
                    label++;
                }
                if (settings.lakeRoundness > 0.75f)
                {
                    label++;
                }
                if (settings.lakeRoundness > 2f)
                {
                    label++;
                }
                if (settings.lakeRoundness > 2.75f)
                {
                    label++;
                }
                return MapDesigner_Mod.FormatLabel("ZMD_lakeRoundness", "ZMD_lakeRoundness" + label);
            }
        }


        private string lakeDepthLabel
        {
            get
            {
                int label = 0;
                if (settings.lakeDepth > 0.2f)
                {
                    label++;
                }
                if (settings.lakeDepth > 0.4f)
                {
                    label++;
                }
                if (settings.lakeDepth > 0.6f)
                {
                    label++;
                }
                if (settings.lakeDepth > 0.8f)
                {
                    label++;
                }
                return MapDesigner_Mod.FormatLabel("ZMD_lakeDepth", "ZMD_lakeDepth" + label);
            }
        }
        #endregion
    }
}
