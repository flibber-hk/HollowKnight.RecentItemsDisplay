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

    // In order to subscribe to the ModifyDisplayItem event without requiring RecentItems to be installed, we can do so using the following code:
    /*
            Type recentItemsEvents = Type.GetType("RecentItemsDisplay.Events, RecentItemsDisplay");
            if (recentItemsEvents == null) { Log("Did not hook RecentItemsDisplay"); return; }
            recentItemsEvents.GetEvent("ModifyDisplayItem").AddEventHandler(null, (Action<EventArgs>)Events_ModifyDisplayItem);
     */
    // We then define the function Events_ModifyDisplayItem as follows:
    /*
            private void Events_ModifyDisplayItem(EventArgs args)
            {
                RecentItemsDisplay.ItemDisplayArgs displayArgs = args as RecentItemsDisplay.ItemDisplayArgs;

                // Code goes here
            }
     */
    // This procedure requires a reference to RecentItemsDisplay, although it will not need to be installed by the user.

}
