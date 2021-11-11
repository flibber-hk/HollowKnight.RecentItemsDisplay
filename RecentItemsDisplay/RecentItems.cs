using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using Modding;
using MonoMod.ModInterop;
using UnityEngine;

namespace RecentItemsDisplay
{
    public class RecentItems : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<SaveData>, IMenuMod
    {
        internal static RecentItems instance;

        public static GlobalSettings globalSettings { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings s)
        {
            s.MaxItems = Mathf.Clamp(s.MaxItems, 1, Display.MaxDisplayableItems);
            globalSettings = s;
        }
        public GlobalSettings OnSaveGlobal() => globalSettings;

        public static SaveData saveData { get; set; } = new SaveData();
        public void OnLoadLocal(SaveData s) => saveData = s;
        public SaveData OnSaveLocal() => saveData;

        #region Menu
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();

            entries.Add(new IMenuMod.MenuEntry()
            {
                Name = "Show Display",
                Description = string.Empty,
                Values = new string[] { "True", "False" },
                Saver = opt => globalSettings.ShowDisplay = opt == 0,
                Loader = () => globalSettings.ShowDisplay ? 0 : 1
            });

            entries.Add(new IMenuMod.MenuEntry()
            {
                Name = "Max Displayable Items",
                Description = string.Empty,
                Values = Enumerable.Range(1, Display.MaxDisplayableItems).Select(x => x.ToString()).ToArray(),
                Saver = opt => globalSettings.MaxItems = opt + 1,
                Loader = () => globalSettings.MaxItems - 1
            });

            entries.Add(new IMenuMod.MenuEntry()
            {
                Name = "Show Refreshed Items",
                Description = "Toggle whether to send items to the display when it's not your first time picking them up",
                Values = new string[] { "True", "False" },
                Saver = opt => globalSettings.ShowRefreshedItems = opt == 0,
                Loader = () => globalSettings.ShowRefreshedItems ? 0 : 1
            });

            return entries;
        }
        public bool ToggleButtonInsideMenu => false;
        #endregion

        public override void Initialize()
        {
            Log("Initializing...");
            instance = this;

            Display.Hook();
            AreaName.LoadData();

            AbstractItem.AfterGiveGlobal += SendItemToDisplay;
            typeof(ItemDisplayMethods).ModInterop();
        }


        private void SendItemToDisplay(ReadOnlyGiveEventArgs obj)
        {
            ItemDisplayMethods.ShowItem(new ItemDisplayArgs(obj));
        }


        public override string GetVersion()
        {
            return "0.2";
        }
    }
}
