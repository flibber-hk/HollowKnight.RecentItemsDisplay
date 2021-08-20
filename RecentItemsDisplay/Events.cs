using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger;
using UnityEngine;

namespace RecentItemsDisplay
{
    public static class Events
    {
        public static event Action<ReadOnlyGiveEventArgs, ItemDisplayArgs> ModifyDisplayItem;
        internal static void ModifyDisplayItemInvoke(ReadOnlyGiveEventArgs giveArgs, ItemDisplayArgs displayArgs)
        {
            ModifyDisplayItem?.Invoke(giveArgs, displayArgs);
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

            public string DisplayName { get; set; }
            public string DisplaySource { get; set; }
            /// <summary>
            /// Set this value to completely overwrite the text displayed
            /// </summary>
            public string DisplayMessage { get; set; }
            public Sprite DisplaySprite { get; set; } 
            public bool IgnoreItem { get; set; }
        }
    }
}
