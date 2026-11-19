using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>HUD (timer, pressure, crosshair, hint), briefing card and the feed container.</summary>
    internal static class HudUiBuilder
    {
        public static HudView Hud(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Stretch("Hud", canvas);
            CanvasGroup group = ui.Group(root, false);

            RectTransform crosshair = ui.Rect("Crosshair", root, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(14f, 14f));
            crosshair.pivot = new Vector2(0.5f, 0.5f);
            Image crosshairImage = ui.Panel(crosshair, Color.white, ui.Sprites.Circle);

            RectTransform timerPanel = ui.Rect("TimerPanel", root, new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(220f, 72f));
            ui.Panel(timerPanel, UiFactory.Dark);
            TextMeshProUGUI timer = ui.Text(ui.Stretch("Timer", timerPanel), "2:30", 46f, Color.white, bold: true);

            RectTransform pressure = ui.Rect("Pressure", root, new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(460f, 30f));
            ui.Panel(pressure, UiFactory.Dark);
            RectTransform fillRect = ui.Stretch("Fill", pressure);
            fillRect.offsetMin = new Vector2(4f, 4f);
            fillRect.offsetMax = new Vector2(-4f, -4f);
            Image fill = ui.Panel(fillRect, Color.white);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 0f;
            RectTransform label = ui.Rect("Label", root, new Vector2(0.5f, 0f), new Vector2(0f, 70f), new Vector2(460f, 30f));
            ui.Text(label, "ТИСК", 22f, Color.white, bold: true);

            RectTransform hintRect = ui.Rect("Hint", root, new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(700f, 50f));
            hintRect.pivot = new Vector2(0.5f, 0.5f);
            TextMeshProUGUI hint = ui.Text(hintRect, string.Empty, 32f, new Color(1f, 0.9f, 0.35f), bold: true);

            HudView view = root.gameObject.AddComponent<HudView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_timer", timer);
            EditorAssetUtility.SetReference(view, "_pressureFill", fill);
            EditorAssetUtility.SetReference(view, "_crosshair", crosshairImage);
            EditorAssetUtility.SetReference(view, "_hint", hint);
            return view;
        }

        public static BriefingView Briefing(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Stretch("Briefing", canvas);
            CanvasGroup group = ui.Group(root, false);
            ui.Fill(root, new Color(0f, 0f, 0f, 0.55f));

            RectTransform card = ui.Rect("Card", root, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(940f, 300f));
            card.pivot = new Vector2(0.5f, 0.5f);
            ui.Panel(card, UiFactory.Paper);
            RectTransform titleRect = ui.Rect("Title", card, new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(860f, 90f));
            TextMeshProUGUI title = ui.Text(titleRect, "ВИКЛИК", 72f, new Color(0.85f, 0.15f, 0.12f), bold: true);
            RectTransform bodyRect = ui.Rect("Body", card, new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(860f, 140f));
            TextMeshProUGUI body = ui.Text(bodyRect, string.Empty, 40f, UiFactory.Ink);

            BriefingView view = root.gameObject.AddComponent<BriefingView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_title", title);
            EditorAssetUtility.SetReference(view, "_body", body);
            return view;
        }

        public static FeedView Feed(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Rect("Feed", canvas, new Vector2(1f, 1f), new Vector2(-28f, -28f), new Vector2(580f, 640f));
            CanvasGroup group = ui.Group(root, false);
            RectTransform container = ui.Stretch("Lines", root);
            VerticalLayoutGroup layout = container.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.UpperRight;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            FeedView view = root.gameObject.AddComponent<FeedView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_container", container);
            return view;
        }
    }
}
