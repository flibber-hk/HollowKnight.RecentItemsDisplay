using System.Collections.Generic;
using ItemChanger;
using Modding;

namespace RecentItemsDisplay
{
    public class RecentItems : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<SaveData>, IMenuMod
    {
        internal static RecentItems instance;

        public static GlobalSettings globalSettings { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;

        public static SaveData saveData { get; set; } = new SaveData();
        public void OnLoadLocal(SaveData s) => saveData = s;
        public SaveData OnSaveLocal() => saveData;

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

            AbstractItem.AfterGiveGlobal += SendItemToDisplay;
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
