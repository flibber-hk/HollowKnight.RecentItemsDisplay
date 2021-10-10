using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RecentItemsDisplay
{
    public static class AreaName
    {
        private static readonly List<string> suffixes = new List<string>() { "_boss_defeated", "_boss", "_preload" };

        private static Dictionary<string, string> sceneToArea;

        public static void LoadData()
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

        public static string CleanAreaName(string scene)
        {
            if (string.IsNullOrEmpty(scene)) return string.Empty;

            foreach (string suffix in suffixes)
            {
                if (scene.EndsWith(suffix))
                {
                    scene = scene.Substring(0, scene.Length - suffix.Length);
                }
            }

            switch (scene)
            {
                // We can treat shops as special cases
                case "Room_mapper": return "Iselda";
                case "Room_shop": return "Sly";
                case "Room_Charm_Shop": return "Salubra";
                case "Fungus2_26": return "Leg Eater";
                case "Crossroads_38": return "Grubfather";
                case "RestingGrounds_07": return "Seer";
                case "Room_Ouiji": return "Jiji";
                case "Room_Jinn": return "Jinn";

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

    }
}
