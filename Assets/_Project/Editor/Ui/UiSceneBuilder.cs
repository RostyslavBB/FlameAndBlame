using FB.Game.Installers;
using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
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
            BriefingView briefing = HudUiBuilder.Briefing(ui, canvas);
            ReportView report = ReportUiBuilder.Build(ui, canvas);
            CreateEventSystem();

            UiInstaller installer = contextObject.AddComponent<UiInstaller>();
            EditorAssetUtility.SetReference(installer, "_config", config);
            EditorAssetUtility.SetReference(installer, "_briefing", briefing);
            EditorAssetUtility.SetReference(installer, "_hud", hud);
            EditorAssetUtility.SetReference(installer, "_feed", feed);
            EditorAssetUtility.SetReference(installer, "_report", report);
            EditorAssetUtility.SetReference(installer, "_feedEntryPrefab", entryPrefab);
            return installer;
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
