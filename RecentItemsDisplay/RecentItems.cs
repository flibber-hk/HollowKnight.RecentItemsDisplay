using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ItemChanger;
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

            AbstractItem.AfterGiveGlobal += SendItemToDisplay;
        }


        private void SendItemToDisplay(ReadOnlyGiveEventArgs obj)
        {
            string item = obj.Item.UIDef.GetPostviewName();
            string scene = GetSceneFromPlacement(obj.Placement);

            string source = AreaName.CleanAreaName(scene);

            Sprite sprite = obj.Item.UIDef.GetSprite();

            Events.ItemDisplayArgs args = new Events.ItemDisplayArgs(item, source, sprite);
            Events.ModifyDisplayItemInvoke(obj, args);

            Display.AddItem(args);
        }

        private string GetSceneFromPlacement(AbstractPlacement placement)
        {
            if (placement is ItemChanger.Placements.AutoPlacement apmt)
            {
                return apmt.Location.sceneName;
            }
            else if (placement is ItemChanger.Placements.MutablePlacement mpmt)
            {
                return mpmt.Location.sceneName;
            }
            else if (placement is ItemChanger.Placements.DualPlacement dpmt)
            {
                return dpmt.Location.sceneName;
            }
            else if (placement is ItemChanger.Placements.EggShopPlacement epmt)
            {
                return epmt.Location.sceneName;
            }
            else if (placement is ItemChanger.Placements.ShopPlacement spmt)
            {
                return spmt.Location.sceneName;
            }
            else if (placement is ItemChanger.Placements.YNShinyPlacement ypmt)
            {
                return ypmt.Location.sceneName;
            }

            return string.Empty;
        }

        #region API
        /// <summary>
        /// Manually show an item on the recent items display
        /// </summary>
        /// <param name="args">The ItemDisplayArgs object that represents the item to display</param>
        [PublicAPI]
        public static void ShowItem(Events.ItemDisplayArgs args)
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
        public static void ShowItem(string message, Sprite sprite)
        {
            Events.ItemDisplayArgs args = new Events.ItemDisplayArgs(string.Empty, string.Empty, sprite)
            {
                DisplayMessage = message
            };
            ShowItem(args);
        }

        /// <summary>
        /// Manually show an item on the recent items display
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="source">The source of the item (e.g. area name)</param>
        /// <param name="sprite">The sprite to display</param>
        [PublicAPI]
        public static void ShowItem(string name, string source, Sprite sprite)
        {
            Events.ItemDisplayArgs args = new Events.ItemDisplayArgs(name, source, sprite);
            ShowItem(args);
        }

        // In order to directly send an item to the display without requiring RecentItems to be installed, we can use a pattern like this.
        // First, we store a reference to the function we want to be using. We can do this efficiently by using the FastReflectionDelegate in
        // MonoMod.Utils (requires a reference to that dll).
        /*
            private static readonly FastReflectionDelegate ShowItem_string_string_Sprite = Type.GetType("RecentItemsDisplay.RecentItems, RecentItemsDisplay")?
                                     .GetMethod("ShowItem", BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Any,
                                            new Type[] { typeof(string), typeof(string), typeof(Sprite) }, null)
                                     .GetFastDelegate();
        */
        // We then create a wrapper for this function which matches ShowItem from this mod.
        /*    
            internal static void ShowItem(string name, string source, Sprite sprite)
            {
                if (ShowItem_string_string_Sprite != null)
                {
                    ShowItem_string_string_Sprite(null, new object[] { name, source, sprite });
                }
            }
        */
        // We can now use ShowItem as normal, and nothing will happen if RecentItems is not installed.
        #endregion

        public override string GetVersion()
        {
            return "0.1";
        }
    }
}
