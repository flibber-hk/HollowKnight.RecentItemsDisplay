using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;

namespace RecentItemsDisplay
{
    internal static class Display
    {
        private static Queue<GameObject> items = new Queue<GameObject>();

        private static GameObject canvas;
        public static void Create()
        {
            if (canvas != null) return;
            // Create base canvas
            canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
            Object.DontDestroyOnLoad(canvas);

            CanvasUtil.CreateTextPanel(canvas, "Recent Items", 24, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(200, 100), Vector2.zero,
                new Vector2(0.87f, 0.95f), new Vector2(0.87f, 0.95f)));

            if (invPanels <= 0) Show();
        }

        public static void Destroy()
        {
            if (canvas != null) Object.Destroy(canvas);
            canvas = null;

            items.Clear();
        }

        public static void AddItem(Events.ItemDisplayArgs args)
        {
            if (args.IgnoreItem) return;

            string msg = args.GetMessage();
            Sprite sprite = args.DisplaySprite;

            if (canvas == null)
            {
                Create();
            }

            GameObject basePanel = CanvasUtil.CreateBasePanel(canvas,
                new CanvasUtil.RectData(new Vector2(200, 50), Vector2.zero,
                new Vector2(0.9f, 0.9f), new Vector2(0.9f, 0.9f)));

            CanvasUtil.CreateImagePanel(basePanel, sprite,
                new CanvasUtil.RectData(new Vector2(50, 50), Vector2.zero, new Vector2(0f, 0.5f),
                    new Vector2(0f, 0.5f)));
            CanvasUtil.CreateTextPanel(basePanel, msg, 24, TextAnchor.MiddleLeft,
                new CanvasUtil.RectData(new Vector2(400, 100), Vector2.zero,
                new Vector2(1.2f, 0.5f), new Vector2(1.2f, 0.5f)),
                CanvasUtil.GetFont("Perpetua"));

            items.Enqueue(basePanel);
            if (items.Count > RecentItems.globalSettings.MaxItems)
            {
                Object.Destroy(items.Dequeue());
            }

            UpdatePositions();
        }

        private static void UpdatePositions()
        {
            int i = items.Count - 1;
            foreach (GameObject item in items)
            {
                Vector2 newPos = new Vector2(0.9f, 0.9f - 0.06f * i--);
                item.GetComponent<RectTransform>().anchorMin = newPos;
                item.GetComponent<RectTransform>().anchorMax = newPos;
            }
        }

        public static void Show()
        {
            if (canvas == null) return;
            canvas.SetActive(RecentItems.globalSettings.ShowDisplay);
        }

        public static void Hide()
        {
            if (canvas == null) return;
            canvas.SetActive(false);
        }

        // Hacky solution to the problem where we open multiple panels at once, which happens when showing lore dialogue in a shop.
        // In future, a less lazy implementation of shop lore which closes the shop menu (temporarily, perhaps) rather than yeeting 
        // it off screen would deal with this problem more sensibly.
        private static int invPanels = 0;

        internal static void Hook()
        {
            UnHook();

            //ModHooks.Instance.AfterSavegameLoadHook += OnLoad; 
            On.QuitToMenu.Start += OnQuitToMenu;
            On.InvAnimateUpAndDown.AnimateUp += OnInventoryOpen;
            On.InvAnimateUpAndDown.AnimateDown += OnInventoryClose;
            On.UIManager.GoToPauseMenu += OnPause;
            On.UIManager.UIClosePauseMenu += OnUnpause;
        }

        internal static void UnHook()
        {
            //ModHooks.Instance.AfterSavegameLoadHook -= OnLoad;
            On.QuitToMenu.Start -= OnQuitToMenu;
            On.InvAnimateUpAndDown.AnimateUp -= OnInventoryOpen;
            On.InvAnimateUpAndDown.AnimateDown -= OnInventoryClose;
            On.UIManager.GoToPauseMenu -= OnPause;
            On.UIManager.UIClosePauseMenu -= OnUnpause;
        }

        // It probably doesn't fit to show a recent items popup on load in non-multi
        //private static void OnLoad(SaveGameData data)
        //{
        //    Create();
        //}

        private static IEnumerator OnQuitToMenu(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            Destroy();
            return orig(self);
        }

        private static void OnInventoryOpen(On.InvAnimateUpAndDown.orig_AnimateUp orig, InvAnimateUpAndDown self)
        {
            orig(self);
            invPanels++;
            Hide();
        }

        private static void OnInventoryClose(On.InvAnimateUpAndDown.orig_AnimateDown orig, InvAnimateUpAndDown self)
        {
            orig(self);
            invPanels--;
            if (invPanels <= 0) Show();
        }

        private static IEnumerator OnPause(On.UIManager.orig_GoToPauseMenu orig, UIManager self)
        {
            //yield return orig(self);

            // Failsafe
            if (invPanels != 0)
            {
                RecentItems.instance.LogWarn("Warning: invPanels not equal to 0 on pause");
                invPanels = 0;
            }
            Hide();
            return orig(self);
        }

        private static void OnUnpause(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
        {
            orig(self);
            Show();
        }
    }
}
