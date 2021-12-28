using System;
using System.Collections.Generic;
using ItemChanger;

namespace RecentItemsDisplay
{
    public class SaveData
    {
        public Queue<DisplayData> latestItems = new();

        internal void Save(UIDef def, string text)
        {
            latestItems.Enqueue(new DisplayData(def, text));
            if (latestItems.Count > Display.MaxDisplayableItems)
            {
                latestItems.Dequeue();
            }
        }

        internal void SendAll()
        {
            foreach (DisplayData data in latestItems)
            {
                ItemDisplayMethods.ShowItemInternal(data.spriteHolder, data.text);
            }
        }
    }

    [Serializable]
    public struct DisplayData
    {
        public UIDef spriteHolder;
        public string text;

        public DisplayData(UIDef spriteHolder, string text)
        {
            this.spriteHolder = spriteHolder;
            this.text = text;
        }
    }
}
