using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;

namespace RecentItemsDisplay
{
    public class RecentItems : Mod
    {
        internal static RecentItems instance;

        public override void Initialize()
        {
            Log("Initializing...");
            instance = this;

            Display.Hook();
            AreaName.LoadData();

            // Unsafe add
            ItemChanger.Events.AfterGive += SendItemToDisplay;
        }

        private void SendItemToDisplay(ItemChanger.ReadOnlyGiveEventArgs obj)
        {
            string item = obj.Item.UIDef.GetPostviewName();
            string scene = obj.Placement.Location.sceneName;

            Sprite sprite = obj.Item.UIDef.GetSprite();

            Display.AddItem(item, scene, sprite);
        }

        public override string GetVersion()
        {
            return "0.1";
        }
    }
}
