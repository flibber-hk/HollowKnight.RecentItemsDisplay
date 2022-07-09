using System;
using UnityEngine;

namespace RecentItemsDisplay
{
    public static class ManualConfig
    {
        public const string CATEGORY = "Recent Items Config";

        /// <summary>
        /// Set up the debug keybinds
        /// </summary>
        public static void Setup()
        {
            DebugMod.AddActionToKeyBindList(Reset, "Default Position", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(-0.0125f, 0), "Move Display Left", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(0.0125f, 0), "Move Display Right", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(0, 0.02f), "Move Display Up", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(0, -0.02f), "Move Display Down", CATEGORY);
            DebugMod.AddActionToKeyBindList(ToggleDisplay, "Toggle Display", CATEGORY);
        }

        public static void Reset()
        {
            RecentItems.GS.AnchorPoint = GlobalSettings.DefaultAnchor;
            Display.Redraw();
        }

        private static Action GetMover(float x, float y)
        {
            void Move()
            {
                RecentItems.GS.AnchorPoint += new Vector2(x,y);
                Display.Redraw();
            }
            return Move;
        }

        private static void ToggleDisplay()
        {
            RecentItems.GS.ShowDisplay = !RecentItems.GS.ShowDisplay;
            RecentItems.instance.RefreshMenu();
            DebugMod.LogToConsole("Toggled Display");
        }
    }
}
