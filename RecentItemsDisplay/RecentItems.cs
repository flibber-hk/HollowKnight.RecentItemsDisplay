using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;

namespace RecentItemsDisplay
{
    public class RecentItems : Mod, IGlobalSettings<GlobalSettings>, IMenuMod
    {
        internal static RecentItems instance;

        public static GlobalSettings globalSettings { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;

        #region Menu
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>()
            {
                new IMenuMod.MenuEntry
                {
                    Name = "Show Display:",
                    Description = string.Empty,
                    Values = new string[]{ "True", "False" },
                    Saver = opt => globalSettings.ShowDisplay = opt == 0,
                    Loader = () => globalSettings.ShowDisplay ? 0 : 1
                }
            };
        }
        public bool ToggleButtonInsideMenu => false;
        #endregion

        public override void Initialize()
        {
            Log("Initializing...");
            instance = this;

            Display.Hook();
            AreaName.LoadData();

            // Unsafe add
            ItemChanger.Events.AfterGive += SendItemToDisplay;
        }


        private void SendItemToDisplay(ItemChanger.ReadOnlyGiveEventArgs obj)
        {
            string item = obj.Item.UIDef.GetPostviewName();
            string scene = obj.Placement.Location.sceneName;
            string source = AreaName.CleanAreaName(scene);

            Sprite sprite = obj.Item.UIDef.GetSprite();

            Events.ItemDisplayArgs args = new Events.ItemDisplayArgs(item, source, sprite);
            Events.ModifyDisplayItemInvoke(obj, args);

            Display.AddItem(args);
        }

        public override string GetVersion()
        {
            return "0.1";
        }
    }
}
