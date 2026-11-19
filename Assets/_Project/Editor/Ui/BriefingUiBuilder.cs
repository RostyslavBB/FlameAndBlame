using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>Briefing: dimmed screen, pulsing siren glow, paper card with a red stripe, title, text and countdown.</summary>
    internal static class BriefingUiBuilder
    {
        private static readonly Vector2 CardSize = new Vector2(1000f, 460f);

        private static readonly (string Key, string Action)[] Controls =
        {
            ("ЛКМ", "вода"), ("WASD", "рух"), ("E", "щиток"), ("ПРОБІЛ", "стрибок")
        };

        public static BriefingView Build(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Stretch("Briefing", canvas);
            CanvasGroup group = ui.Group(root, false);
            ui.Fill(root, new Color(0.05f, 0.02f, 0.02f, 0.6f));

            RectTransform sirenRect = ui.Centered("Siren", root, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1700f, 1000f));
            Image siren = ui.Panel(sirenRect, new Color(UiFactory.Fire.r, UiFactory.Fire.g, UiFactory.Fire.b, 0f), ui.Sprites.Glow);

            RectTransform card = ui.Centered("Card", root, new Vector2(0.5f, 0.5f), Vector2.zero, CardSize);
            ui.Card(card, UiFactory.Paper, 0.6f);
            RectTransform stripe = ui.Centered("Stripe", card, new Vector2(0.5f, 1f), new Vector2(0f, -22f), new Vector2(CardSize.x - 60f, 12f));
            ui.Panel(stripe, UiFactory.Ember, ui.Sprites.Pill);

            TextMeshProUGUI title = ui.Text(ui.Centered("Title", card, new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(900f, 110f)),
                "ВИКЛИК", 92f, UiFactory.Ember, bold: true, overWorld: true);
            TextMeshProUGUI body = ui.Text(ui.Centered("Body", card, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(880f, 120f)),
                string.Empty, 40f, UiFactory.Ink);

            ControlsRow(ui, card);

            RectTransform track = ui.Centered("Countdown", card, new Vector2(0.5f, 0f), new Vector2(0f, 46f), new Vector2(CardSize.x - 140f, 26f));
            ui.Panel(track, new Color(0.3f, 0.18f, 0.14f, 0.35f), ui.Sprites.Pill);
            RectTransform fillRect = ui.Stretch("Fill", track);
            fillRect.offsetMin = new Vector2(4f, 4f);
            fillRect.offsetMax = new Vector2(-4f, -4f);
            Image countdown = ui.Panel(fillRect, UiFactory.Fire);
            countdown.type = Image.Type.Filled;
            countdown.fillMethod = Image.FillMethod.Horizontal;
            countdown.fillAmount = 1f;
            TextMeshProUGUI countdownLabel = ui.Text(
                ui.Centered("CountdownLabel", card, new Vector2(0.5f, 0f), new Vector2(0f, 88f), new Vector2(600f, 36f)),
                string.Empty, 28f, UiFactory.Ember, bold: true);

            BriefingView view = root.gameObject.AddComponent<BriefingView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_card", card);
            EditorAssetUtility.SetReference(view, "_title", title);
            EditorAssetUtility.SetReference(view, "_body", body);
            EditorAssetUtility.SetReference(view, "_siren", siren);
            EditorAssetUtility.SetReference(view, "_countdown", countdown);
            EditorAssetUtility.SetReference(view, "_countdownLabel", countdownLabel);
            return view;
        }

        /// <summary>Keycap chips ("[ЛКМ] вода") so a first-time player knows the controls before the round starts.</summary>
        private static void ControlsRow(UiFactory ui, RectTransform card)
        {
            const float groupWidth = 215f;
            RectTransform row = ui.Centered("Controls", card, new Vector2(0.5f, 0f), new Vector2(0f, 140f),
                new Vector2(groupWidth * Controls.Length, 44f));
            for (int i = 0; i < Controls.Length; i++)
            {
                RectTransform key = ui.Rect(Controls[i].Key, row, new Vector2(0f, 0.5f), new Vector2(i * groupWidth, 0f), new Vector2(104f, 44f));
                key.pivot = new Vector2(0f, 0.5f);
                ui.Card(key, UiFactory.Ink, 0.3f, ui.Sprites.Pill);
                TextMeshProUGUI keyText = ui.Text(ui.Stretch("Key", key), Controls[i].Key, 22f, UiFactory.Gold, bold: true);
                keyText.enableAutoSizing = true;
                keyText.fontSizeMin = 12f;
                keyText.fontSizeMax = 22f;
                keyText.margin = new Vector4(10f, 0f, 10f, 0f);

                RectTransform action = ui.Rect("Action", row, new Vector2(0f, 0.5f), new Vector2(i * groupWidth + 114f, 0f), new Vector2(100f, 44f));
                action.pivot = new Vector2(0f, 0.5f);
                ui.Text(action, Controls[i].Action, 24f, UiFactory.Ink, TextAlignmentOptions.Left);
            }
        }
    }
}
