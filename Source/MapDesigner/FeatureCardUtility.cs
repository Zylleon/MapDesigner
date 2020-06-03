﻿using System;
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
            this.settings.priIslandSize = listing.Slider(this.settings.priIslandSize, 20f, 56f);

            listing.Label("ZMD_priBeachSizeLabel".Translate());
            this.settings.priBeachSize = listing.Slider(this.settings.priBeachSize, 1f, 18f);

            //listingStandard.CheckboxLabeled("ZPRI_MarshyBeachesLabel".Translate(), ref ZPRI_Settings.marshyBeaches);


            //listingStandard.CheckboxLabeled("ZPRI_MarshyBeachesLabel".Translate(), ref ZPRI_Settings.marshyBeaches, "ZPRI_MarshyBeachesTooltip".Translate());

            listing.CheckboxLabeled("ZMD_priMultiSpawnLabel".Translate(), ref MapDesignerSettings.priMultiSpawn);




        }

        public void DrawLakeOptions(Listing_Standard listing)
        {
            listing.Label("ZMD_featureLakeInfo".Translate());

        }
    }
}
