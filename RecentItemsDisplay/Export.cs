using ItemChanger;
using MonoMod.ModInterop;
using UnityEngine;

namespace RecentItemsDisplay
{
    /// <summary>
    /// Methods in this class can be invoked through ModInterop without needing RecentItemsDisplay as a dependency.
    /// See DebugInterop.cs for an example of how to import methods safely.
    /// For other public methods, invoke them with a reference to RecentItemsDisplay or via Reflection.
    /// </summary>
    [ModExportName(nameof(RecentItems))]
    public static class Export
    {
        public static void ToggleDisplay(bool show)
        {
            if (show) Display.Show();
            else Display.Hide();
        }

        public static void ShowItemWithMessage(string message, Sprite sprite)
            => ItemDisplayMethods.ShowItem(message, sprite);

        public static void ShowItemChangerSprite(string message, string spriteKey)
            => ItemDisplayMethods.ShowItem(message, new ItemChangerSprite(spriteKey));
    }
}
