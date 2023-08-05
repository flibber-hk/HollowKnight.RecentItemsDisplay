using System;
using System.Linq;
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
                    DisplaySource = AreaName.LocalizedCleanAreaName(locpmt.Location.sceneName);
                }
            }
            SpriteSource = args.Item.UIDef;

            // Don't show item if they already obtained it
            if (!RecentItems.GS.ShowRefreshedItems && args.OriginalState != ObtainState.Unobtained)
            {
                IgnoreItem = true;
            }

            // Check the placement/location before items, as we can assume that if there's a clash the item tag should take priority
            foreach (ItemChanger.Tags.IInteropTag tag in args.Placement.GetPlacementAndLocationTags().OfType<ItemChanger.Tags.IInteropTag>())
            {
                if (tag.Message != nameof(RecentItems)) continue;

                if (tag.TryGetProperty(nameof(DisplaySource), out string DisplaySourceOverride)) this.DisplaySource = DisplaySourceOverride;
                if (tag.TryGetProperty(nameof(DisplayMessage), out string DisplayMessageOverride)) this.DisplayMessage = DisplayMessageOverride;
                if (tag.TryGetProperty(nameof(IgnoreItem), out bool IgnoreItemOverride)) this.IgnoreItem = IgnoreItemOverride;
            }
            foreach (ItemChanger.Tags.IInteropTag tag in args.Orig.GetTags<ItemChanger.Tags.IInteropTag>())
            {
                if (tag.Message != nameof(RecentItems)) continue;

                if (tag.TryGetProperty(nameof(DisplayName), out string DisplayNameOverride)) this.DisplayName = DisplayNameOverride;
                if (tag.TryGetProperty(nameof(DisplaySource), out string DisplaySourceOverride)) this.DisplaySource = DisplaySourceOverride;
                if (tag.TryGetProperty(nameof(DisplayMessage), out string DisplayMessageOverride)) this.DisplayMessage = DisplayMessageOverride;
                if (tag.TryGetProperty(nameof(SpriteSource), out UIDef SpriteSourceOverride)) this.SpriteSource = SpriteSourceOverride;
                if (tag.TryGetProperty(nameof(IgnoreItem), out bool IgnoreItemOverride)) this.IgnoreItem = IgnoreItemOverride;
            }

        }

        /// <summary>
        /// The message shown. If the DisplayMessage property is set, then it will be that;
        /// otherwise will default to "DisplayName\nfrom DisplaySource"
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            return GetUnformattedMessage().Replace("<br>", "\n");
        }

        private string GetUnformattedMessage()
        {
            if (!string.IsNullOrEmpty(DisplayMessage))
            {
                return DisplayMessage;
            }
            return string.IsNullOrEmpty(DisplaySource)
                ? DisplayName
                : string.Format(Language.Language.Get("DEFAULT_MESSAGE_FORMAT", "RecentItems"), DisplayName, DisplaySource);
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
