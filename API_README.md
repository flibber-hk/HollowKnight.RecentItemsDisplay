There are several ways to interact with the Recent Items Display.

* Use MonoMod ModInterop. To do this, create a class capable of importing the exported methods in Export.cs; 
an example of a safe way to do this would be how it's done for DebugMod in DebugImport.cs. This allows you
to send items to the display and show/hide the display.

* Use the methods in ItemDisplayMethods.cs to send a message to the display. These give more options for
the message to send, but require either a reference to recent items or reflection.

* Add an IInteropTag to items and placements in ItemChanger. By default, all obtained items will be shown
on the display (except for respawned items if the user excluded them). An IInteropTag with message equal
to `RecentItems` applied to an item can modify the displayed sprite, name or message; an IInteropTag applied
to a placement can modify the source or message. In either case the item can be ignored as well. See [here](https://github.com/flibber-hk/HollowKnight.RecentItemsDisplay/blob/f06413a35f0e37dba552e6f9025600912528c871/RecentItemsDisplay/ItemDisplayArgs.cs#L58-L74)
for supported properties. This procedure does not require a reference to RecentItems and RecentItems does
not need to be installed.

* Subscribe to the `Events.ModifyDisplayItem` event. This procedure requires a reference to RecentItems,
or reflection.
