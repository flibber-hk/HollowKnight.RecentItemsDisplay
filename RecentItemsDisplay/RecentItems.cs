using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

        /// <summary>
        /// Manually show an item on the recent items display
        /// </summary>
        /// <param name="args">The ItemDisplayArgs object that represents the item to display</param>
        [PublicAPI]
        public void SendItemToDisplay(Events.ItemDisplayArgs args)
        {
            Events.ModifyDisplayItemInvoke(null, args);
            Display.AddItem(args);
        }

        /// <summary>
        /// Manually show an item on the recent items display
        /// </summary>
        /// <param name="message">The text to display</param>
        /// <param name="sprite">The item sprite</param>
        [PublicAPI]
        public void SendItemToDisplay(string message, Sprite sprite)
        {
            Events.ItemDisplayArgs args = new Events.ItemDisplayArgs(string.Empty, string.Empty, sprite)
            {
                DisplayMessage = message
            };
            SendItemToDisplay(args);
        }

        /// <summary>
        /// Manually show an item on the recent items display
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="source">The source of the item (e.g. area name)</param>
        /// <param name="sprite">The sprite to display</param>
        [PublicAPI]
        public void SendItemToDisplay(string name, string source, Sprite sprite)
        {
            Events.ItemDisplayArgs args = new Events.ItemDisplayArgs(name, source, sprite);
            SendItemToDisplay(args);
        }

        public override string GetVersion()
        {
            return "0.1";
        }
    }
}
