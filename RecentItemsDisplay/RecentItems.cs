using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using Modding;
using MonoMod.ModInterop;
using UnityEngine;
using UnityEngine.UI;

namespace RecentItemsDisplay
{
    public class RecentItems : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<SaveData>, IMenuMod
    {
        internal static RecentItems instance;

        public RecentItems() : base(null)
        {
            instance = this;
            typeof(Export).ModInterop();
        }

        public static GlobalSettings GS { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings s)
        {
            s.MaxItems = Mathf.Clamp(s.MaxItems, 1, Display.MaxDisplayableItems);
            GS = s;
        }
        public GlobalSettings OnSaveGlobal() => GS;

        public static SaveData SD { get; set; } = new SaveData();
        public void OnLoadLocal(SaveData s) => SD = s;
        public SaveData OnSaveLocal() => SD;

        #region Menu
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new();

            entries.Add(new IMenuMod.MenuEntry()
            {
                Name = "Show Display",
                Description = string.Empty,
                Values = new string[] { "True", "False" },
                Saver = opt => GS.ShowDisplay = opt == 0,
                Loader = () => GS.ShowDisplay ? 0 : 1
            });

            entries.Add(new IMenuMod.MenuEntry()
            {
                Name = "Max Displayable Items",
                Description = string.Empty,
                Values = Enumerable.Range(1, Display.MaxDisplayableItems).Select(x => x.ToString()).ToArray(),
                Saver = opt => GS.MaxItems = opt + 1,
                Loader = () => GS.MaxItems - 1
            });

            entries.Add(new IMenuMod.MenuEntry()
            {
                Name = "Show Refreshed Items",
                Description = "Toggle whether to send items to the display when it's not your first time picking them up",
                Values = new string[] { "True", "False" },
                Saver = opt => GS.ShowRefreshedItems = opt == 0,
                Loader = () => GS.ShowRefreshedItems ? 0 : 1
            });

            // Shorten the description of Show Refreshed Items before adding to the menu

            return entries;
        }
        public bool ToggleButtonInsideMenu => false;
        #endregion

        public override void Initialize()
        {
            Log("Initializing...");

            Display.Hook();
            AreaName.LoadData();

            AbstractItem.AfterGiveGlobal += SendItemToDisplay;

            ManualConfig.Setup();
        }


        private void SendItemToDisplay(ReadOnlyGiveEventArgs obj)
        {
            ItemDisplayMethods.ShowItem(new ItemDisplayArgs(obj));
        }


        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }

        public void RefreshMenu()
        {
            MenuScreen screen = ModHooks.BuiltModMenuScreens[this];
            if (screen != null)
            {
                foreach (MenuOptionHorizontal option in screen.GetComponentsInChildren<MenuOptionHorizontal>())
                {
                    option.menuSetting.RefreshValueFromGameSettings();
                }
            }
        }
    }
}
