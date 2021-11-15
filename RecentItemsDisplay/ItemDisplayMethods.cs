using System;
using ItemChanger;
using MonoMod.ModInterop;
using UnityEngine;
using JetBrains.Annotations;

namespace RecentItemsDisplay
{
    /// <summary>
    /// A collection of methods to send an item to the display
    /// </summary>
    [ModExportName("RecentItemsDisplay")]
    public static class ItemDisplayMethods
    {
        #region API
        /// <summary>
        /// Show an item on the recent items display
        /// </summary>
        /// <param name="args">The ItemDisplayArgs object that represents the item to display</param>
        [PublicAPI]
        public static void ShowItem(ItemDisplayArgs args)
        {
            Events.ModifyDisplayItemInvoke(args);

            if (args.IgnoreItem) return;

            try
            {
                RecentItems.saveData.Save(args.DisplaySprite, args.GetMessage());
                ShowItemInternal(args.DisplaySprite, args.GetMessage());
            }
            catch (Exception ex)
            {
                RecentItems.instance.LogError("Error displaying item\n" + ex);
            }
        }

        /// <summary>
        /// Show an item on the recent items display, given a message and a sprite
        /// The sprite will not be properly displayed when reloading a file.
        /// </summary>
        /// <param name="message">The text to display</param>
        /// <param name="sprite">The sprite to display</param>
        [PublicAPI]
        public static void ShowItem(string message, Sprite sprite)
        {
            ItemDisplayArgs args = new ItemDisplayArgs(string.Empty, string.Empty, sprite)
            {
                DisplayMessage = message
            };
            ShowItem(args);
        }

        /// <summary>
        /// Show an item on the recent items display, given a name, source and sprite. 
        /// The message will be formatted as name // from source
        /// The sprite will not be properly displayed when reloading a file.
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="source">The source of the item (e.g. area name)</param>
        /// <param name="sprite">The sprite to display</param>
        [PublicAPI]
        public static void ShowItem(string name, string source, Sprite sprite)
        {
            ItemDisplayArgs args = new ItemDisplayArgs(name, source, sprite);
            ShowItem(args);
        }

        /// <summary>
        /// Manually show an item on the recent items display, given a message and an ISprite.
        /// If the ISprite is serializable, this will be properly displayed when reloading a file.
        /// </summary>
        /// <param name="message">The text to display</param>
        /// <param name="sprite">The sprite to display</param>
        [PublicAPI]
        public static void ShowItem(string message, ISprite sprite)
        {
            ItemDisplayArgs args = new ItemDisplayArgs(string.Empty, string.Empty, sprite)
            {
                DisplayMessage = message
            };
            ShowItem(args);
        }

        /// <summary>
        /// Show an item on the recent items display, given a name, source and sprite. 
        /// The message will be formatted as name // from source
        /// If the ISprite is serializable, this will be properly displayed when reloading a file.
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="source">The source of the item (e.g. area name)</param>
        /// <param name="sprite">The sprite to display</param>
        [PublicAPI]
        public static void ShowItem(string name, string source, ISprite sprite)
        {
            ItemDisplayArgs args = new ItemDisplayArgs(name, source, sprite);
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
        // A similar procedure will work for any of the methods here; simply replace the name of the method (if necessary),
        // and the types of the input parameters (if necessary)
        #endregion

        public static void ShowItemInternal(ISprite sprite, string text)
        {
            Sprite spriteValue;
            try
            {
                spriteValue = sprite.Value;
            }
            catch (Exception ex)
            {
                RecentItems.instance.LogError("Failed to get sprite: " + ex);
                spriteValue = new EmptySprite().Value;
            }

            Display.AddItem(spriteValue, text);        // Display.AddItem(Sprite.GetValue, text);
        }
    }
}
