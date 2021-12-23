using System;
using UnityEngine;

namespace RecentItemsDisplay
{
    public static class ManualConfig
    {
        public const string CATEGORY = "Recent Items Config";
        public static void Setup()
        {
            DebugMod.AddActionToKeyBindList(Reset, "Default Position", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(-0.0125f, 0), "Move Display Left", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(0.0125f, 0), "Move Display Right", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(0, 0.02f), "Move Display Up", CATEGORY);
            DebugMod.AddActionToKeyBindList(GetMover(0, -0.02f), "Move Display Down", CATEGORY);
        }

        public static void Reset()
        {
            RecentItems.globalSettings.AnchorPoint = GlobalSettings.DefaultAnchor;
            Display.Redraw();
        }

        private static Action GetMover(float x, float y)
        {
            void Move()
            {
                RecentItems.globalSettings.AnchorPoint += new Vector2(x,y);
                Display.Redraw();
            }
            return Move;
        }
    }
}
