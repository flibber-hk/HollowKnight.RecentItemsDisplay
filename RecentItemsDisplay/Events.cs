using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger;
using JetBrains.Annotations;
using UnityEngine;

namespace RecentItemsDisplay
{
    public static class Events
    {
        [PublicAPI]
        public static event Action<ReadOnlyGiveEventArgs, ItemDisplayArgs> ModifyDisplayItem;
        internal static void ModifyDisplayItemInvoke(ReadOnlyGiveEventArgs giveArgs, ItemDisplayArgs displayArgs)
        {
            Delegate[] invocationList = ModifyDisplayItem.GetInvocationList();

            foreach (Action<ReadOnlyGiveEventArgs, ItemDisplayArgs> toInvoke in invocationList)
            {
                try
                {
                    toInvoke(giveArgs, displayArgs);
                }
                catch (Exception ex)
                {
                    RecentItems.instance.LogError("Error modifying display item\n" + ex);
                }
            }
        }

        public class ItemDisplayArgs : EventArgs
        {
            public ItemDisplayArgs(string name, string source, Sprite Sprite)
            {
                DisplayName = name;
                DisplaySource = source;
                DisplaySprite = Sprite;
                DisplayMessage = string.Empty;
                IgnoreItem = false;
            }

            /// <summary>
            /// The message shown. If the DisplayMessage property is set, then it will be that;
            /// otherwise will default to DisplayName // from DisplaySource
            /// </summary>
            /// <returns></returns>
            public string GetMessage()
            {
                if (string.IsNullOrEmpty(DisplayMessage))
                {
                    return string.IsNullOrEmpty(DisplaySource) ? DisplayName : DisplayName + "\nfrom " + DisplaySource;
                }
                else
                {
                    return DisplayMessage;
                }
            }
            /// <summary>
            /// The name of the item given
            /// </summary>
            public string DisplayName { get; set; }
            /// <summary>
            /// The source of the item given (by default, the area name)
            /// </summary>
            public string DisplaySource { get; set; }
            /// <summary>
            /// Set this value to completely overwrite the text displayed
            /// </summary>
            public string DisplayMessage { get; set; }
            /// <summary>
            /// The sprite to display
            /// </summary>
            public Sprite DisplaySprite { get; set; } 
            /// <summary>
            /// If this bool is set to true, the item will not be displayed
            /// </summary>
            public bool IgnoreItem { get; set; }
        }
    }
}
