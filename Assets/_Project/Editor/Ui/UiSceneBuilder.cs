using FB.Game.Installers;
using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>Creates the UI canvas (HUD, feed, briefing, report), the EventSystem and the UiInstaller.</summary>
    internal static class UiSceneBuilder
    {
        public static UiInstaller Build(GameObject contextObject, UiConfig config)
        {
            TMP_FontAsset font = TmpFontBuilder.Ensure();
            UiFactory ui = new UiFactory(font, UiSpriteLibrary.Ensure());
            FeedEntryView entryPrefab = FeedEntryPrefabBuilder.Build(ui);

            Transform canvas = CreateCanvas();
            HudView hud = HudUiBuilder.Hud(ui, canvas);
            FeedView feed = HudUiBuilder.Feed(ui, canvas);
            ShockOverlayView shock = ShockOverlay(ui, canvas);
            BriefingView briefing = HudUiBuilder.Briefing(ui, canvas);
            ReportView report = ReportUiBuilder.Build(ui, canvas);
            CreateEventSystem();

            UiInstaller installer = contextObject.AddComponent<UiInstaller>();
            EditorAssetUtility.SetReference(installer, "_config", config);
            EditorAssetUtility.SetReference(installer, "_briefing", briefing);
            EditorAssetUtility.SetReference(installer, "_hud", hud);
            EditorAssetUtility.SetReference(installer, "_feed", feed);
            EditorAssetUtility.SetReference(installer, "_report", report);
            EditorAssetUtility.SetReference(installer, "_shock", shock);
            EditorAssetUtility.SetReference(installer, "_feedEntryPrefab", entryPrefab);
            return installer;
        }

        private static ShockOverlayView ShockOverlay(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Stretch("ShockOverlay", canvas);
            CanvasGroup group = ui.Group(root, false);
            Image flash = ui.Fill(ui.Stretch("Flash", root), new Color(0.55f, 0.85f, 1f, 0f));

            RectTransform boltsRect = ui.Stretch("Bolts", root);
            boltsRect.offsetMin = new Vector2(-40f, -40f);
            boltsRect.offsetMax = new Vector2(40f, 40f);
            RawImage bolts = boltsRect.gameObject.AddComponent<RawImage>();
            bolts.texture = ui.Sprites.Lightning;
            bolts.raycastTarget = false;

            Volume volume = Building.PostProcessBuilder.CreateVolume("ShockVolume", Building.PostProcessBuilder.ShockProfile(),
                20f, 0f);
            ShockOverlayView view = root.gameObject.AddComponent<ShockOverlayView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_bolts", bolts);
            EditorAssetUtility.SetReference(view, "_flash", flash);
            EditorAssetUtility.SetReference(view, "_volume", volume);
            return view;
        }

        private static Transform CreateCanvas()
        {
            GameObject go = new GameObject("UI");
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return go.transform;
        }

        private static void CreateEventSystem()
        {
            GameObject go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
        }
    }
}
