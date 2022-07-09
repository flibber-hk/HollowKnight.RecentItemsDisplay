using System.Collections.Generic;
using System.IO;
using ItemChanger;
using ItemChanger.Util;
using Newtonsoft.Json;
using Lang = Language.Language;

namespace RecentItemsDisplay
{
    public static class AreaName
    {
        internal static Dictionary<string, string> sceneToArea;

        internal static void LoadData()
        {
            JsonSerializer js = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            Stream json = typeof(AreaName).Assembly.GetManifestResourceStream("RecentItemsDisplay.Resources.SceneToArea.json");
            StreamReader sr = new StreamReader(json);
            JsonTextReader jtr = new JsonTextReader(sr);
            sceneToArea = js.Deserialize<Dictionary<string, string>>(jtr);
        }

        /// <summary>
        /// Returns a cleaned area name for the given scene, not localized.
        /// </summary>
        public static string CleanAreaName(string scene)
        {
            if (string.IsNullOrEmpty(scene)) return string.Empty;

            if (SceneUtil.TryGetSuperScene(scene, out string super))
            {
                scene = super;
            }

            switch (scene)
            {
                // We can treat shops and scenes with just an NPC as special cases
                case SceneNames.Room_mapper: return "Iselda";
                case SceneNames.Room_shop: return "Sly";
                case SceneNames.Room_Charm_Shop: return "Salubra";
                case SceneNames.Fungus2_26: return "Leg Eater";
                case SceneNames.Crossroads_38: return "Grubfather";
                case SceneNames.RestingGrounds_07: return "Seer";
                case SceneNames.Room_Ouiji: return "Jiji";
                case SceneNames.Room_Jinn: return "Jinn";
                case SceneNames.Grimm_Divine: return "Divine";
                // Forward compatibility
                case SceneNames.Room_nailsmith: return "Nailsmith";
                case SceneNames.Room_Mask_Maker: return "Mask Maker";
                case SceneNames.Room_Tram: return "Upper Tram";
                case SceneNames.Room_Tram_RG: return "Lower Tram";

                default:
                    if (sceneToArea.TryGetValue(scene, out string area))
                    {
                        return area;
                    }
                    else if (scene.StartsWith("GG_"))
                    {
                        // GG_Waterways, Pipeway, Shortcut and Lurker are in the JSON
                        return "Godhome";
                    }
                    else
                    {
                        return string.Empty;
                    }
            }
        }

        public static string LocalizedCleanAreaName(string sceneName) => Localize(CleanAreaName(sceneName));

        /// <summary>
        /// Given an area name from the SceneToArea.json, localizes it (using internal TC names where possible).
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public static string Localize(string areaName)
        {
            string localized = areaName switch
            {
                "Grubfather" => Lang.Get("GRUBFATHER_MAIN", "RecentItems"),

                "Iselda" => Lang.Get("ISELDA_MAIN", "Titles"),
                "Sly" => Lang.Get("SLY_MAIN", "Titles"),
                "Salubra" => Lang.Get("CHARM_SLUG_MAIN", "Titles"),
                "Leg Eater" => Lang.Get("LEGEATER_MAIN", "Titles"),
                "Seer" => Lang.Get("DREAM_MOTH_MAIN", "Titles"),
                "Jiji" => Lang.Get("JIJI_MAIN", "Titles"),
                "Jinn" => Lang.Get("JINN_MAIN", "Titles"),
                "Divine" => Lang.Get("DIVINE_MAIN", "Titles"),
                "Nailsmith" => Lang.Get("NAILSMITH_MAIN", "Titles"),
                "Mask Maker" => Lang.Get("MASKMAKER_MAIN", "Titles"),

                "Lower Tram" => Lang.Get("TRAM_LOWER", "Map Zones"),
                "Upper Tram" => Lang.Get("TRAM_UPPER", "Map Zones"),
                "Godhome" => Lang.Get("GODHOME_MAIN", "CP3"),

                "Junk Pit" => Lang.Get("GODSEEKER_WASTE", "Map Zones"),
                "Blue Lake" => Lang.Get("BLUE_LAKE", "Map Zones"),
                "Teacher's Archives" => Lang.Get("MONOMON_ARCHIVE", "Map Zones"),
                "Watcher's Spire" => Lang.Get("LURIENS_TOWER", "Map Zones"),
                "Tower of Love" => Lang.Get("LOVE_TOWER", "Map Zones"),
                "King's Pass" => Lang.Get("KINGS_PASS", "Map Zones"),
                "Spirits' Glade" => Lang.Get("GLADE", "Map Zones"),
                "Holy Land" => Lang.Get("BONE_FOREST", "Map Zones"),
                "King's Station" => Lang.Get("KINGS_STATION", "Map Zones"),
                "Howling Cliffs" => Lang.Get("CLIFFS", "Map Zones"),
                "Crystallized Mound" => Lang.Get("CRYSTAL_MOUND", "Map Zones"),
                "Ancient Basin" => Lang.Get("ABYSS", "Map Zones"),
                "Forgotten Crossroads" => Lang.Get("CROSSROADS", "Map Zones"),
                "Distant Village" => Lang.Get("DISTANT_VILLAGE", "Map Zones"),
                "Hive" => Lang.Get("HIVE", "Map Zones"),
                "Dirtmouth" => Lang.Get("TOWN", "Map Zones"),
                "Fungal Wastes" => Lang.Get("WASTES", "Map Zones"),
                "City of Tears" => Lang.Get("CITY", "Map Zones"),
                "The Royal Quarter" => Lang.Get("ROYAL_QUARTER", "Map Zones"),
                "Stone Sanctuary" => Lang.Get("NOEYES_TEMPLE", "Map Zones"),
                "Palace Grounds" => Lang.Get("PALACE_GROUNDS", "Map Zones"),
                "Failed Tramway" => Lang.Get("RUINED_TRAMWAY", "Map Zones"),
                "Isma's Grove" => Lang.Get("ISMAS_GROVE", "Map Zones"),
                "Queen's Station" => Lang.Get("QUEENS_STATION", "Map Zones"),
                "Lake of Unn" => Lang.Get("ACID_LAKE", "Map Zones"),
                "Mantis Village" => Lang.Get("MANTIS_VILLAGE", "Map Zones"),
                "Soul Sanctum" => Lang.Get("SOUL_SOCIETY", "Map Zones"),
                "Colosseum" => Lang.Get("COLOSSEUM", "Map Zones"),
                "White Palace" => Lang.Get("WHITE_PALACE", "Map Zones"),
                "Royal Waterways" => Lang.Get("WATERWAYS", "Map Zones"),
                "Queen's Gardens" => Lang.Get("ROYAL_GARDENS", "Map Zones"),
                "Beast's Den" => Lang.Get("BEASTS_DEN", "Map Zones"),
                "Hallownest's Crown" => Lang.Get("PEAK", "Map Zones"),
                "Ancestral Mound" => Lang.Get("SHAMAN_TEMPLE", "Map Zones"),
                "Deepnest" => Lang.Get("DEEPNEST", "Map Zones"),
                "Crystal Peak" => Lang.Get("MINES", "Map Zones"),
                "Dream" => Lang.Get("DREAM_WORLD", "Map Zones"),
                "Greenpath" => Lang.Get("GREEN_PATH", "Map Zones"),
                "Overgrown Mound" => Lang.Get("OVERGROWN_MOUND", "Map Zones"),
                "Kingdom's Edge" => Lang.Get("OUTSKIRTS", "Map Zones"),
                "Abyss" => Lang.Get("ABYSS_DEEP", "Map Zones"),
                "Resting Grounds" => Lang.Get("RESTING_GROUNDS", "Map Zones"),
                "Cast Off Shell" => Lang.Get("WYRMSKIN", "Map Zones"),
                "Fog Canyon" => Lang.Get("FOG_CANYON", "Map Zones"),

                "Pleasure House" => Lang.Get("BATHHOUSE_FULL", "Titles"),
                "Stag Nest" => Lang.Get("STAG_NEST", "StagMenu"),
                "Fungal Core" => string.Join(" ", Lang.Get("FUNGUS_CORE_SUPER", "Titles"), Lang.Get("FUNGUS_CORE_MAIN", "Titles"), Lang.Get("FUNGUS_CORE_SUB", "Titles")),
                "Weavers' Den" => string.Join(" ", Lang.Get("WEAVERS_DEN_SUPER", "Titles"), Lang.Get("WEAVERS_DEN_MAIN", "Titles"), Lang.Get("WEAVERS_DEN_SUB", "Titles")),
                
                "Path of Pain" => string.Join(" ", Lang.Get("PATH_OF_SACRIFICE_SUPER", "Titles"), 
                                                   Lang.Get("PATH_OF_SACRIFICE_MAIN", "Titles"), 
                                                   Lang.Get("PATH_OF_SACRIFICE_SUB", "Titles")),

                "Black Egg Temple" => Lang.Get("KEY_BLACKEGG", "UI"),

                _ => areaName,
            };

            return localized.Replace("<br>", " ").Trim();
        }

    }
}
