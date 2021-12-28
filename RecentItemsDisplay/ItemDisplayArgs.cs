using System;
using ItemChanger;
using JetBrains.Annotations;
using UnityEngine;

namespace RecentItemsDisplay
{
    [PublicAPI]
    public class ItemDisplayArgs : EventArgs
    {
        public ItemDisplayArgs(string name, string source, Sprite sprite) : this(name, source, new BoxedSprite(sprite)) { }

        public ItemDisplayArgs(string name, string source, ISprite sprite)
        {
            DisplayName = name;
            DisplaySource = source;
            SpriteSource = new ItemChanger.UIDefs.MsgUIDef()
            {
                sprite = sprite
            };
        }

        public ItemDisplayArgs(ReadOnlyGiveEventArgs args)
        {
            GiveEventArgs = args;

            if (args?.Item?.UIDef == null)
            {
                DisplayName = null;
                DisplaySource = null;
                SpriteSource = null;
                IgnoreItem = true;

                return;
            }

            DisplayName = args.Item.UIDef.GetPostviewName();
            if (args.Placement is ItemChanger.Placements.IPrimaryLocationPlacement locpmt)
            {
                if (locpmt.Location is ItemChanger.Locations.StartLocation)
                {
                    DisplaySource = "Start";
                }
                else
                {
                    DisplaySource = AreaName.CleanAreaName(locpmt.Location.sceneName);
                }
            }
            SpriteSource = args.Item.UIDef;

            // Don't show item if they already obtained it
            if (!RecentItems.GS.ShowRefreshedItems && args.OriginalState != ObtainState.Unobtained)
            {
                IgnoreItem = true;
            }
        }

        /// <summary>
        /// The message shown. If the DisplayMessage property is set, then it will be that;
        /// otherwise will default to "DisplayName\nfrom DisplaySource"
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
        /// The ReadOnlyGiveEventArgs associated with the message. This will be null if the message was not sent by ItemChanger.
        /// </summary>
        public ReadOnlyGiveEventArgs GiveEventArgs { get; }
        /// <summary>
        /// The name of the item given
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// The source of the item given (by default, the area name)
        /// </summary>
        public string DisplaySource { get; set; } = string.Empty;
        /// <summary>
        /// Set this value to completely overwrite the text displayed
        /// </summary>
        public string DisplayMessage { get; set; } = string.Empty;
        /// <summary>
        /// A UIDef whose GetSprite method returns the sprite to display.
        /// </summary>
        public UIDef SpriteSource { get; set; }
        /// <summary>
        /// If this bool is set to true, the item will not be displayed
        /// </summary>
        public bool IgnoreItem { get; set; } = false;
    }
}
