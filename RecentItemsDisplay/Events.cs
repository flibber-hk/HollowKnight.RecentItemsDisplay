using System;
using ItemChanger;
using JetBrains.Annotations;
using UnityEngine;

namespace RecentItemsDisplay
{
    public static class Events
    {
        /// <summary>
        /// Modify the message that will be sent to the display
        /// </summary>
        [PublicAPI]
        public static event Action<ItemDisplayArgs> ModifyDisplayItem;
        internal static void ModifyDisplayItemInvoke(ItemDisplayArgs displayArgs)
        {
            if (ModifyDisplayItem == null) return;

            Delegate[] invocationList = ModifyDisplayItem.GetInvocationList();

            foreach (Action<ItemDisplayArgs> toInvoke in invocationList)
            {
                try
                {
                    toInvoke(displayArgs);
                }
                catch (Exception ex)
                {
                    RecentItems.instance.LogError("Error modifying display item\n" + ex);
                }
            }
        }
    }

    [PublicAPI]
    public class ItemDisplayArgs : EventArgs
    {
        public ItemDisplayArgs(string name, string source, Sprite sprite) : this(name, source, new BoxedSprite(sprite)) { }

        public ItemDisplayArgs(string name, string source, ISprite sprite)
        {
            DisplayName = name;
            DisplaySource = source;
            DisplaySprite = sprite;
        }

        public ItemDisplayArgs(ReadOnlyGiveEventArgs args)
        {
            GiveEventArgs = args;

            if (args?.Item?.UIDef == null)
            {
                DisplayName = null;
                DisplaySource = null;
                DisplaySprite = new EmptySprite();
                IgnoreItem = true;

                RecentItems.instance.Log("Incomplete item def sent to display");

                return;
            }

            DisplayName = args.Item.UIDef.GetPostviewName();

            if (args.Placement is ItemChanger.Placements.IPrimaryLocationPlacement locpmt)
            {
                DisplaySource = AreaName.CleanAreaName(locpmt.Location.sceneName);
            }

            if (args.Item.UIDef is ItemChanger.UIDefs.MsgUIDef msgUIDef)
            {
                DisplaySprite = msgUIDef.sprite;
            }
            else
            {
                DisplaySprite = new BoxedSprite(args.Item.UIDef.GetSprite());
            }

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
        /// The sprite to display
        /// </summary>
        public ISprite DisplaySprite { get; set; }
        /// <summary>
        /// If this bool is set to true, the item will not be displayed
        /// </summary>
        public bool IgnoreItem { get; set; } = false;
    }

    // In order to subscribe to the ModifyDisplayItem event without requiring RecentItems to be installed, we can do so using the following code:
    /*
            Type recentItemsEvents = Type.GetType("RecentItemsDisplay.Events, RecentItemsDisplay");
            if (recentItemsEvents == null) { Log("Did not Hook RID"); return; }
            recentItemsEvents.GetEvent("ModifyDisplayItem").AddEventHandler(null, (Action<EventArgs, EventArgs>)Events_ModifyDisplayItem);
     */
    // We then define the function Events_ModifyDisplayItem as follows:
    /*
            private void Events_ModifyDisplayItem(EventArgs arg1, EventArgs arg2)
            {
                ItemChanger.ReadOnlyGiveEventArgs giveArgs = arg1 as ItemChanger.ReadOnlyGiveEventArgs;
                RecentItemsDisplay.ItemDisplayArgs displayArgs = arg2 as RecentItemsDisplay.ItemDisplayArgs;

                // Code goes here
            }
     */
    // This procedure requires a reference to RecentItemsDisplay, although it will not need to be installed by the user.

}
