using System;
using System.Collections.Generic;
using ItemChanger;

namespace RecentItemsDisplay
{
    public class SaveData
    {
        public Queue<DisplayData> latestItems = new Queue<DisplayData>();

        internal void Save(ISprite sprite, string text)
        {
            latestItems.Enqueue(new DisplayData(sprite, text));
            if (latestItems.Count > Display.MaxDisplayableItems)
            {
                latestItems.Dequeue();
            }
        }

        internal void SendAll()
        {
            foreach (DisplayData data in latestItems)
            {
                ItemDisplayMethods.ShowItemInternal(data.sprite, data.text);
            }
        }
    }

    [Serializable]
    public struct DisplayData
    {
        public ISprite sprite;
        public string text;

        public DisplayData(ISprite sprite, string text)
        {
            if (sprite.GetType().IsSerializable)
            {
                this.sprite = sprite;
            }
            else
            {
                this.sprite = new EmptySprite();
            }

            this.text = text;
        }
    }
}
