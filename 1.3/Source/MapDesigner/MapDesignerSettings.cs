using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using System.Reflection;
using System.Globalization;

namespace MapDesigner
{

    public class MapDesignerSettings : ModSettings
    {
        #region Enums
        public enum RiverStyle : byte
        {
            Vanilla,
            Spring,
            Canal,
            Confluence,
            Fork,
            Oxbow,
        }

        public enum CoastDirection : byte
        {
            Vanilla,
            North,
            East,
            South,
            West
        }

        public enum Features : byte
        {
            None,
            RoundIsland,
            Lake,
            NatIsland
        }

        public enum PriStyle : byte
        {
            Single,
            Multi
        }

        public enum NiStyle : byte
        {
            Round,
            Crescent,
            Square,
            Ring
        }

        #endregion


        #region settings
        // General
        public static bool flagHomeMapOnly = true;
        public string configName = "";

        // Mountains
        public float hillAmount = 1.0f;
        public float hillSize = 0.021f;
        public float hillSmoothness = 2.0f;
        public bool flagMtnExit = false;
        public bool flagCaves = true;

        public bool flagHillClumping = false;

        public bool flagHillRadial = false;
        public float hillRadialAmt = 1.0f;
        public float hillRadialSize = 0.65f;

        public bool flagHillSplit = false;
        public float hillSplitAmt = 1.5f;
        public float hillSplitDir = 90f;
        public float hillSplitSize = 0.35f;

        public bool flagHillSide = false;
        public float hillSideAmt = 1.0f;
        public float hillSideDir = 180f;

        // Things
        public float densityPlant = 1.0f;
        public float densityAnimal = 1.0f;
        public float densityRuins = 1.0f;
        public float densityDanger = 1.0f;

        public float densityGeyser = 1.0f;
        public float densityOre = 1.0f;
        public float animaCount = 1.0f;
        public bool flagRockChunks = true;

        public Dictionary<string, float> oreCommonality = new Dictionary<string, float>();

        // Ideology
        public bool flagRoadDebris = true;
        public bool flagCaveDebris = true;
        public bool flagAncientUtilityBuilding = true;
        public bool flagAncientTurret = true;
        public bool flagAncientMechs = true;
        public bool flagAncientLandingPad = true;
        public bool flagAncientFences = true;
        public int countMechanoidRemains = 1;
        public float densityAncientPipelineSection = 1f;
        public float densityAncientJunkClusters = 1f;

        // Vanilla Power Expanded
        public IntRange vpe_HelixienVents = new IntRange(1, 2);
        public IntRange vpe_ChemfuelPonds = new IntRange(1, 3);

        // Rocks
        public IntRange rockTypeRange = new IntRange(2, 3);
        public bool flagBiomeRocks = false;
        public Dictionary<string, bool> allowedRocks = new Dictionary<string, bool>();

        // Rivers
        public float sizeRiver = 1.0f;
        public bool flagRiverBeach = false;
        public string riverShore = "SoilRich";
        public float riverBeachSize = 10f;                // in tiles
        public bool flagRiverDir = false;
        public float riverDir = 180f;

        public bool flagRiverLoc = false;               // for whether the location is picked at all
        public bool flagRiverLocAbs= false;
        public Vector3 riverCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);

        public MapDesignerSettings.RiverStyle selRiverStyle = RiverStyle.Vanilla;

        // Beaches
        public CoastDirection coastDir = CoastDirection.Vanilla;
        public string beachTerr = "Vanilla";


        // Terrain
        public float terrainFert = 1f;
        public float terrainWater = 1f;
        public bool flagTerrainWater = false;


        // Features
        public MapDesignerSettings.Features selectedFeature = Features.None;

        // Perfectly Round Islands
        public float priIslandSize = 40f;
        public float priBeachSize = 5f;
        public PriStyle priStyle = PriStyle.Single;
        public Vector3 priSingleCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);
        public bool flagPriSalty = false;

        // Lake
        public float lakeSize = 0.20f;                  // proportion of map size
        public float lakeBeachSize = 10f;                // in tiles
        public float lakeRoundness = 1.5f;              // 0 = perfectly round
        public float lakeDepth = 0.5f;                  // proportion of deep water
        public bool flagLakeSalty = false;
        public string lakeShore = "Sand";
        public Vector3 lakeCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);

        // Natural Islands
        public float niSize = 0.75f;                  // proportion of map size
        public float niBeachSize = 20f;                // in tiles
        public float niRoundness = 1.5f;              // 0 = perfectly round
        public bool flagNiSalty = false;
        public string niShore = "Sand";
        public Vector3 niCenterDisp = new Vector3(0.0f, 0.0f, 0.0f);
        public NiStyle niStyle = NiStyle.Round;


        // Helper stuff
        public Dictionary<string, BiomeDefault> biomeDefaults;
        public Dictionary<string, FloatRange> densityDefaults;
        public Dictionary<string, float> riverDefaults;
        public Dictionary<string, float> oreDefaults;

        #endregion


        public List<string> configs = new List<string>();


        public bool SaveNewConfig()
        {
            if (configName == "")
            {
                Messages.Message("ZMD_saveFail2".Translate(), MessageTypeDefOf.RejectInput);
                return false;
            }

            if(!configs.NullOrEmpty())
            {
                if(configs.Count >= 15)
                {
                    Messages.Message("ZMD_saveFail3".Translate(), MessageTypeDefOf.RejectInput);
                    return false;
                }

                foreach (string config in configs)
                {
                    if(config.Split(':')[0] == configName)
                    {
                        Messages.Message("ZMD_saveFail1".Translate(), MessageTypeDefOf.RejectInput);
                        return false;
                    }
                }
            }

            if (configs == null)
            {
                configs = new List<string>();
            }
            configs.Add(this.ToString());
            Messages.Message(string.Format("ZMD_saveSuccess".Translate(), configName), MessageTypeDefOf.TaskCompletion);
            return true;

        }

        public void LoadConfig(string configName)
        {
            string configString = configs.Where(c => c.Split(':')[0] == configName).FirstOrDefault();
            MapDesignerMod.mod.settings.ParseConfigString(configString);

        }


        public override string ToString()
        {
            string configString = configName + ":";
            var fields = typeof(MapDesignerSettings).GetFields();
            List<string> toIgnore = new List<string>
            {
                "configs",                  // list of configs, don't save them recursively

                "biomeDefaults",            // these are default data for reference when changing various defs
                "densityDefaults",          // they don't get saved between sessions and aren't settings
                "riverDefaults",
                "oreDefaults",

                "oreCommonality",           // oreCommonality and allowedRocks are dictionaries that get special handling
                "allowedRocks"
            };
            foreach (var field in fields)
            {
                if (field.Name == "oreCommonality")
                {
                    configString += field.Name + "=" + HelperMethods.DictToString((Dictionary<string, float>)field.GetValue(MapDesignerMod.mod.settings));
                }
                else if (field.Name == "allowedRocks")
                {
                    configString += field.Name + "=" + HelperMethods.DictToString((Dictionary<string, bool>)field.GetValue(MapDesignerMod.mod.settings));
                }
                else if(!toIgnore.Contains(field.Name))
                {
                    configString += field.Name + "=" + field.GetValue(MapDesignerMod.mod.settings).ToString() + ";";
                }
            }
            return configString;
        }


        // this was previously the constructor
        public void ParseConfigString(string configString)
        {
            var nameAndSettings = configString.Split(':');
            configName = nameAndSettings[0];
            var settings = nameAndSettings[1];
            var settingValues = settings.Split(';');
            var fields = typeof(MapDesignerSettings).GetFields();
            foreach (var setting in settingValues)
            {
                var nameAndValue = setting.Split('=');
                var settingName = nameAndValue[0];
                Log.Message("... " + settingName);

                if (!settingName.NullOrEmpty())
                {
                    var settingValue = nameAndValue[1];

                    FieldInfo field = fields.Single(fi => fi.Name == settingName);
                    if (field.FieldType == typeof(string))
                    {
                        field.SetValue(this, settingValue); //settingValue will need to be un-ToString'd
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        if (settingValue == "1" || settingValue.ToLower() == "true")
                        {
                            field.SetValue(this, true);
                        }
                        else
                        {
                            field.SetValue(this, false);
                        }
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        float fval = float.Parse(settingValue);
                        field.SetValue(this, fval);
                    }
                    else if (field.FieldType == typeof(Vector3))
                    {
                        field.SetValue(this, ParseHelper.FromStringVector3(settingValue));
                    }
                    else if (field.FieldType == typeof(IntRange))
                    {
                        field.SetValue(this, ParseHelper.ParseIntRange(settingValue));
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        field.SetValue(this, Int32.Parse(settingValue));
                    }
                    else if (field.FieldType == typeof(Dictionary<string, float>))
                    {
                        Dictionary<string, float> val = new Dictionary<string, float>();
                        string[] pairs = settingValue.Split(',');
                        foreach(var pair in pairs)
                        {
                            if(!pair.NullOrEmpty())
                            {
                                var kv = pair.Split('-');
                                val.Add(kv[0], float.Parse(kv[1]));
                            }
                            
                        }
                        field.SetValue(this, val);
                    }
                    else if (field.FieldType == typeof(Dictionary<string, bool>))
                    {
                        Dictionary<string, bool> val = new Dictionary<string, bool>();
                        string[] pairs = settingValue.Split(',');
                        foreach (var pair in pairs)
                        {
                            if (!pair.NullOrEmpty())
                            {
                                var kv = pair.Split('-');
                                if(kv[1] == "1")
                                {
                                    val.Add(kv[0], true);
                                }
                                else
                                {
                                    val.Add(kv[0], false);
                                }
                            }
                        }
                        field.SetValue(this, val);
                    }
                    // enums
                    else if (field.FieldType == typeof(RiverStyle) || field.FieldType == typeof(CoastDirection) || field.FieldType == typeof(Features) || field.FieldType == typeof(PriStyle) || field.FieldType == typeof(NiStyle))
                    {
                        Log.Message(field.Name + " is an enum");
                        field.SetValue(this, Enum.Parse(field.FieldType, settingValue));
                    }
                    else
                    {
                        Log.Message("Skipping " + field.Name + " because it is a " + field.FieldType);
                    }

                }
            }
        }

        public MapDesignerSettings() : base() { }


        public override void ExposeData()
        {
            // General
            Scribe_Values.Look(ref flagHomeMapOnly, "flagHomeMapOnly", true);

            // Saving and Loading
            Scribe_Collections.Look(ref configs, "configs");

            // Mountains
            Scribe_Values.Look(ref hillAmount, "hillAmount", 1.0f);
            Scribe_Values.Look(ref hillSize, "hillSize", 0.021f);
            Scribe_Values.Look(ref hillSmoothness, "hillSmoothness", 2.0f);
            Scribe_Values.Look(ref flagMtnExit, "flagMtnExit", false);
            Scribe_Values.Look(ref flagCaves, "flagCaves", true);

            Scribe_Values.Look(ref flagHillClumping, "flagHillClumping", false);
            Scribe_Values.Look(ref flagHillRadial, "flagHillRadial", false);
            Scribe_Values.Look(ref hillRadialAmt, "hillRadialAmt", 0.0f);
            Scribe_Values.Look(ref hillRadialSize, "hillRadialSize", 0.65f);

            Scribe_Values.Look(ref flagHillSplit, "flagHillSplit", false);
            Scribe_Values.Look(ref hillSplitAmt, "hillSplitAmt", 1.5f);
            Scribe_Values.Look(ref hillSplitDir, "hillSplitDir", 90f);
            Scribe_Values.Look(ref hillSplitSize, "hillSplitSize", 0.35f);

            Scribe_Values.Look(ref flagHillSide, "flagHillSide", false);
            Scribe_Values.Look(ref hillSideAmt, "hillSideAmt", 1.0f);
            Scribe_Values.Look(ref hillSideDir, "hillSideDir", 180f);


            // Things
            Scribe_Values.Look(ref densityPlant, "densityPlant", 1.0f);
            Scribe_Values.Look(ref densityAnimal, "densityAnimal", 1.0f);
            Scribe_Values.Look(ref densityRuins, "densityRuins", 1.0f);
            Scribe_Values.Look(ref densityDanger, "densityDanger", 1.0f);
            Scribe_Values.Look(ref densityGeyser, "densityGeyser", 1.0f);
            Scribe_Values.Look(ref densityOre, "densityOre", 1.0f);
            Scribe_Values.Look(ref animaCount, "animaCount", 1.0f);
            Scribe_Values.Look(ref flagRockChunks, "flagRockChunks", true);

            Scribe_Collections.Look(ref oreCommonality, "oreCommonality");

            // Ideology
            Scribe_Values.Look(ref flagRoadDebris, "flagRoadDebris", true);
            Scribe_Values.Look(ref flagCaveDebris, "flagCaveDebris", true);
            Scribe_Values.Look(ref flagAncientUtilityBuilding, "flagAncientUtilityBuilding", true);
            Scribe_Values.Look(ref flagAncientTurret, "flagAncientTurret", true);
            Scribe_Values.Look(ref flagAncientMechs, "flagAncientMechs", true);
            Scribe_Values.Look(ref flagAncientLandingPad, "flagAncientLandingPad", true);
            Scribe_Values.Look(ref flagAncientFences, "flagAncientFences", true);
            Scribe_Values.Look(ref countMechanoidRemains, "countMechanoidRemains", 1);
            Scribe_Values.Look(ref densityAncientPipelineSection, "densityAncientPipelineSection", 1f);
            Scribe_Values.Look(ref densityAncientJunkClusters, "densityAncientJunkClusters", 1f);

            // Vanilla Power Expanded
            Scribe_Values.Look(ref vpe_HelixienVents, "vpe_HelixienVents", new IntRange(1, 2));
            Scribe_Values.Look(ref vpe_ChemfuelPonds, "vpe_ChemfuelPonds", new IntRange(1, 3));

            // Rocks
            Scribe_Values.Look(ref rockTypeRange, "rockTypeRange", new IntRange(2, 3));
            Scribe_Values.Look(ref flagBiomeRocks, "flagBiomeRocks", false);
            Scribe_Collections.Look(ref allowedRocks, "allowedRocks", LookMode.Value, LookMode.Value);


            // Rivers
            Scribe_Values.Look(ref sizeRiver, "sizeRiver", 1.0f);
            Scribe_Values.Look(ref flagRiverBeach, "flagFertileRivers", false);
            Scribe_Values.Look(ref riverShore, "riverShore", "SoilRich");
            Scribe_Values.Look(ref riverBeachSize, "riverBeachSize", 10f);
            Scribe_Values.Look(ref flagRiverDir, "flagRiverDir", false);
            Scribe_Values.Look(ref riverDir, "riverDir", 180f);
            Scribe_Values.Look(ref selRiverStyle, "selRiverStyle", RiverStyle.Vanilla);

            Scribe_Values.Look(ref flagRiverLoc, "flagRiverLoc", false);
            Scribe_Values.Look(ref flagRiverLocAbs, "flagRiverLocAbs", false);
            Scribe_Values.Look(ref riverCenterDisp, "riverCenterDisp", new Vector3(0.0f, 0.0f, 0.0f));

            // Beaches
            Scribe_Values.Look(ref coastDir, "coastDir", CoastDirection.Vanilla);
            Scribe_Values.Look(ref beachTerr, "beachTerr", "Vanilla");


            // Terrain
            Scribe_Values.Look(ref terrainFert, "terrainFert", 1.0f);
            Scribe_Values.Look(ref terrainWater, "terrainWater", 1.0f);
            Scribe_Values.Look(ref flagTerrainWater, "flagTerrainWater", false);


            // Features
            Scribe_Values.Look(ref selectedFeature, "selectedFeature", Features.None);

            // Round islands
            Scribe_Values.Look(ref priIslandSize, "priIslandSize", 40f);
            Scribe_Values.Look(ref priBeachSize, "priBeachSize", 5f);
            Scribe_Values.Look(ref priStyle, "priStyle", PriStyle.Single);
            Scribe_Values.Look(ref priSingleCenterDisp, "priSingleCenterDisp", new Vector3(0.0f, 0.0f, 0.0f));
            Scribe_Values.Look(ref flagPriSalty, "flagPriSalty", false);

            // Lake
            Scribe_Values.Look(ref lakeSize, "lakeSize", 0.20f);
            Scribe_Values.Look(ref lakeBeachSize, "lakeBeachSize", 10f);
            Scribe_Values.Look(ref lakeRoundness, "lakeRoundness", 1.5f);
            Scribe_Values.Look(ref lakeDepth, "lakeDepth", 0.5f);
            Scribe_Values.Look(ref flagLakeSalty, "flagLakeSalty", false);
            Scribe_Values.Look(ref lakeShore, "lakeShore", "Sand");
            Scribe_Values.Look(ref lakeCenterDisp, "lakeCenterDisp", new Vector3(0.0f, 0.0f, 0.0f));

            // Natural Islands
            Scribe_Values.Look(ref niSize, "niSize", 0.20f);
            Scribe_Values.Look(ref niBeachSize, "niBeachSize", 20f);
            Scribe_Values.Look(ref niRoundness, "niRoundness", 1.5f);
            Scribe_Values.Look(ref flagNiSalty, "flagNiSalty", false);
            Scribe_Values.Look(ref niShore, "niShore", "Sand");
            Scribe_Values.Look(ref niCenterDisp, "niCenterDisp", new Vector3(0.0f, 0.0f, 0.0f));
            Scribe_Values.Look(ref niStyle, "niStyle", NiStyle.Round);

        }

    }



}
