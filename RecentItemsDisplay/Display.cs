using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;

namespace RecentItemsDisplay
{
    internal static class Display
    {
        public static int MaxDisplayableItems { get; internal set; } = 10;

        private static readonly Queue<GameObject> items = new();

        private static GameObject canvas;
        public static void Create()
        {
            if (canvas != null) return;
            // Create base canvas
            canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));

            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            Object.DontDestroyOnLoad(canvas);

            CanvasUtil.CreateTextPanel(canvas, "Recent Items", 24, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(200, 100), Vector2.zero,
                RecentItems.GS.AnchorPoint + new Vector2(-0.025f, 0.05f), 
                RecentItems.GS.AnchorPoint + new Vector2(-0.025f, 0.05f)));

            if (invPanels <= 0 || !RecentItems.GS.HideDisplayWhilePaused) Show();
        }

        public static void Destroy()
        {
            if (canvas != null) Object.Destroy(canvas);
            canvas = null;

            items.Clear();
        }

        public static void Redraw()
        {
            Destroy();
            Create();
            RecentItems.SD.SendAll();
        }

        internal static void AddItem(Sprite sprite, string msg)
        {
            if (canvas == null)
            {
                Create();
            }

            GameObject basePanel = CanvasUtil.CreateBasePanel(canvas,
                new CanvasUtil.RectData(new Vector2(200, 50), Vector2.zero,
                RecentItems.GS.AnchorPoint, RecentItems.GS.AnchorPoint));

            if (sprite != null)
            {
                CanvasUtil.CreateImagePanel(basePanel, sprite,
                    new CanvasUtil.RectData(new Vector2(50, 50), Vector2.zero, new Vector2(-0.1f, 0.5f),
                        new Vector2(-0.1f, 0.5f)));
            }
            CanvasUtil.CreateTextPanel(basePanel, msg, 24, TextAnchor.MiddleLeft,
                new CanvasUtil.RectData(new Vector2(400, 100), Vector2.zero,
                new Vector2(1.1f, 0.5f), new Vector2(1.1f, 0.5f)),
                CanvasUtil.GetFont("Perpetua"));

            items.Enqueue(basePanel);
            if (items.Count > MaxDisplayableItems)
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
                Vector2 newPos = RecentItems.GS.AnchorPoint + new Vector2(0, - 0.06f * i--);
                item.GetComponent<RectTransform>().anchorMin = newPos;
                item.GetComponent<RectTransform>().anchorMax = newPos;
                item.SetActive(i < RecentItems.GS.MaxItems - 1);
            }
        }

        public static void Show()
        {
            if (canvas == null) return;
            canvas.SetActive(RecentItems.GS.ShowDisplay);
        }

        public static void Hide()
        {
            if (canvas == null) return;
            canvas.SetActive(false);
        }

        // Hacky solution to the problem where multiple panels are opened at once
        private static int invPanels = 0;

        internal static void Hook()
        {
            UnHook();

            On.GameManager.FinishedEnteringScene += OnFinishedEnteringScene;
            On.QuitToMenu.Start += OnQuitToMenu;
            On.InvAnimateUpAndDown.AnimateUp += OnInventoryOpen;
            On.InvAnimateUpAndDown.AnimateDown += OnInventoryClose;
            On.UIManager.UIGoToPauseMenu += OnPause;
            On.UIManager.UIClosePauseMenu += OnUnpause;
            On.GameMap.SetupMapMarkers += OnShowMap;
            On.GameMap.DisableMarkers += OnHideMap;
        }

        internal static void UnHook()
        {
            On.GameManager.FinishedEnteringScene -= OnFinishedEnteringScene;
            On.QuitToMenu.Start -= OnQuitToMenu;
            On.InvAnimateUpAndDown.AnimateUp -= OnInventoryOpen;
            On.InvAnimateUpAndDown.AnimateDown -= OnInventoryClose;
            On.UIManager.UIGoToPauseMenu -= OnPause;
            On.UIManager.UIClosePauseMenu -= OnUnpause;
            On.GameMap.SetupMapMarkers -= OnShowMap;
            On.GameMap.DisableMarkers -= OnHideMap;
        }

        private static void TryShow()
        {
            if (invPanels <= 0)
            {
                if (invPanels < 0)
                {
                    invPanels = 0;
                    RecentItems.instance.LogWarn("invPanels less than 0");
                }
                Show();
            }
        }

        private static void OnShowMap(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);
            Hide();
        }
        private static void OnHideMap(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            orig(self);
            TryShow();
        }

        private static bool SentItemsFromSave = false;
        private static void OnFinishedEnteringScene(On.GameManager.orig_FinishedEnteringScene orig, GameManager self)
        {
            orig(self);
            if (!SentItemsFromSave)
            {
                RecentItems.SD.SendAll();
                SentItemsFromSave = true;
            }
        }

        private static IEnumerator OnQuitToMenu(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            Destroy();
            SentItemsFromSave = false;
            return orig(self);
        }

        private static void OnInventoryOpen(On.InvAnimateUpAndDown.orig_AnimateUp orig, InvAnimateUpAndDown self)
        {
            orig(self);
            invPanels++;
            if (RecentItems.GS.HideDisplayWhilePaused) Hide();
        }

        private static void OnInventoryClose(On.InvAnimateUpAndDown.orig_AnimateDown orig, InvAnimateUpAndDown self)
        {
            orig(self);
            invPanels--;
            TryShow();
        }

        private static void OnPause(On.UIManager.orig_UIGoToPauseMenu orig, UIManager self)
        {
            // Failsafe
            if (invPanels != 0)
            {
                RecentItems.instance.LogWarn("invPanels not equal to 0 on pause");
                invPanels = 0;
            }
            if (RecentItems.GS.HideDisplayWhilePaused) Hide();
            orig(self);
        }

        private static void OnUnpause(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
        {
            orig(self);
            TryShow();
            UpdatePositions();
        }
    }
}
