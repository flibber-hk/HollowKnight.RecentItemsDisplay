using UnityEngine;

namespace RecentItemsDisplay
{
    public class GlobalSettings
    {
        public const float DefaultAnchorX = 0.9f;
        public const float DefaultAnchorY = 0.9f;

        public static Vector2 DefaultAnchor => new(DefaultAnchorX, DefaultAnchorY);

        public bool ShowDisplay = true;
        public int MaxItems = 5;
        public bool ShowRefreshedItems = false;
        public Vector2 AnchorPoint = DefaultAnchor;
    }
}
