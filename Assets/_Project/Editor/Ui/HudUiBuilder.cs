using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>
    /// HUD: timer pill (top center), damage bill (top left), pressure bar with glow (bottom), ring crosshair and
    /// the breaker hint pill; plus the feed container. Top and bottom groups slide in from off-screen.
    /// </summary>
    internal static class HudUiBuilder
    {
        public static HudView Hud(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Stretch("Hud", canvas);
            CanvasGroup group = ui.Group(root, false);
            RectTransform top = ui.Stretch("Top", root);
            RectTransform bottom = ui.Stretch("Bottom", root);
            HudView view = root.gameObject.AddComponent<HudView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_top", top);
            EditorAssetUtility.SetReference(view, "_bottom", bottom);

            Timer(ui, top, view);
            Damage(ui, top, view);
            Pressure(ui, bottom, view);
            Crosshair(ui, root, view);
            Hint(ui, root, view);
            return view;
        }

        public static FeedView Feed(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Rect("Feed", canvas, new Vector2(1f, 1f), new Vector2(-28f, -28f), new Vector2(600f, 660f));
            CanvasGroup group = ui.Group(root, false);
            RectTransform container = ui.Stretch("Lines", root);
            VerticalLayoutGroup layout = container.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 14f;
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

        private static void Timer(UiFactory ui, RectTransform top, HudView view)
        {
            RectTransform timer = ui.Centered("Timer", top, new Vector2(0.5f, 1f), new Vector2(0f, -78f), new Vector2(250f, 96f));
            Image face = ui.Card(timer, UiFactory.Dark, 0.5f, ui.Sprites.Pill);
            ui.Text(ui.Rect("Caption", timer, new Vector2(0.5f, 1f), new Vector2(0f, 26f), new Vector2(250f, 26f)),
                "ДО КІНЦЯ ЗМІНИ", 17f, UiFactory.Gold, bold: true, overWorld: true);
            TextMeshProUGUI text = ui.Text(ui.Stretch("Time", timer), "2:30", 56f, Color.white, bold: true);
            EditorAssetUtility.SetReference(view, "_timerRoot", timer);
            EditorAssetUtility.SetReference(view, "_timerFace", face);
            EditorAssetUtility.SetReference(view, "_timer", text);
        }

        private static void Damage(UiFactory ui, RectTransform top, HudView view)
        {
            RectTransform card = ui.Centered("Damage", top, new Vector2(0f, 1f), new Vector2(218f, -82f), new Vector2(360f, 100f));
            ui.Card(card, UiFactory.Dark, 0.5f);
            RectTransform stripe = ui.Rect("Stripe", card, new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(10f, 72f));
            stripe.pivot = new Vector2(0f, 0.5f);
            ui.Panel(stripe, UiFactory.Fire);
            ui.Text(ui.Rect("Caption", card, new Vector2(0f, 1f), new Vector2(36f, -10f), new Vector2(300f, 28f)),
                "ЗБИТКИ", 20f, UiFactory.Fire, TextAlignmentOptions.Left, true);
            TextMeshProUGUI value = ui.Text(ui.Rect("Value", card, new Vector2(0f, 0f), new Vector2(36f, 8f), new Vector2(310f, 58f)),
                "$0", 44f, Color.white, TextAlignmentOptions.Left, true);
            EditorAssetUtility.SetReference(view, "_damageRoot", card);
            EditorAssetUtility.SetReference(view, "_damage", value);
        }

        private static void Pressure(UiFactory ui, RectTransform bottom, HudView view)
        {
            RectTransform bar = ui.Centered("Pressure", bottom, new Vector2(0.5f, 0f), new Vector2(0f, 64f), new Vector2(540f, 36f));
            RectTransform glowRect = ui.Stretch("Glow", bar);
            glowRect.offsetMin = new Vector2(-70f, -50f);
            glowRect.offsetMax = new Vector2(70f, 50f);
            Image glow = ui.Panel(glowRect, new Color(1f, 1f, 1f, 0f), ui.Sprites.Glow);
            ui.Card(bar, UiFactory.Dark, 0.5f, ui.Sprites.Pill);

            RectTransform fillRect = ui.Stretch("Fill", bar);
            fillRect.offsetMin = new Vector2(6f, 6f);
            fillRect.offsetMax = new Vector2(-6f, -6f);
            Image fill = ui.Panel(fillRect, UiFactory.Water);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 0f;

            TextMeshProUGUI label = ui.Text(ui.Centered("Label", bar, new Vector2(0.5f, 1f), new Vector2(0f, 24f), new Vector2(540f, 30f)),
                "ТИСК ВОДИ", 22f, Color.white, bold: true, overWorld: true);
            EditorAssetUtility.SetReference(view, "_pressureRoot", bar);
            EditorAssetUtility.SetReference(view, "_pressureFill", fill);
            EditorAssetUtility.SetReference(view, "_pressureGlow", glow);
            EditorAssetUtility.SetReference(view, "_pressureLabel", label);
        }

        private static void Crosshair(UiFactory ui, RectTransform root, HudView view)
        {
            RectTransform crosshair = ui.Centered("Crosshair", root, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(34f, 34f));
            Image ring = ui.Panel(crosshair, Color.white, ui.Sprites.Ring);
            RectTransform dotRect = ui.Centered("Dot", crosshair, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(6f, 6f));
            Image dot = ui.Panel(dotRect, Color.white, ui.Sprites.Circle);
            EditorAssetUtility.SetReference(view, "_crosshair", crosshair);
            EditorAssetUtility.SetReference(view, "_crosshairRing", ring);
            EditorAssetUtility.SetReference(view, "_crosshairDot", dot);
        }

        private static void Hint(UiFactory ui, RectTransform root, HudView view)
        {
            RectTransform hint = ui.Centered("Hint", root, new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(640f, 66f));
            CanvasGroup group = ui.Group(hint, false);
            ui.Card(hint, UiFactory.Dark, 0.45f, ui.Sprites.Pill);
            TextMeshProUGUI text = ui.Text(ui.Stretch("Text", hint), string.Empty, 30f, Color.white);
            text.enableAutoSizing = true;
            text.fontSizeMin = 18f;
            text.fontSizeMax = 30f;
            text.margin = new Vector4(26f, 0f, 26f, 0f);
            EditorAssetUtility.SetReference(view, "_hintRoot", hint);
            EditorAssetUtility.SetReference(view, "_hintGroup", group);
            EditorAssetUtility.SetReference(view, "_hint", text);
        }
    }
}
