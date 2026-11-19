using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>The damage bill card: counters, top chains, verdict, rubber stamp and the restart button.</summary>
    internal static class ReportUiBuilder
    {
        private static readonly Color FireColor = new Color(0.86f, 0.2f, 0.15f);
        private static readonly Color FirefighterColor = new Color(0.15f, 0.45f, 0.85f);

        public static ReportView Build(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Stretch("Report", canvas);
            CanvasGroup group = ui.Group(root, true);
            ui.Fill(root, new Color(0f, 0f, 0f, 0.7f));

            RectTransform card = ui.Rect("Card", root, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1240f, 900f));
            card.pivot = new Vector2(0.5f, 0.5f);
            ui.Panel(card, UiFactory.Paper);
            ui.Text(ui.Rect("Title", card, new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(1100f, 80f)),
                "РАХУНОК ЗА ВИКЛИК", 60f, UiFactory.Ink, bold: true);

            TextMeshProUGUI fire = Row(ui, card, "Збитки від пожежі", -140f, FireColor);
            TextMeshProUGUI firefighter = Row(ui, card, "Збитки від пожежників", -205f, FirefighterColor);
            TextMeshProUGUI total = Row(ui, card, "Разом", -280f, UiFactory.Ink);

            ui.Text(ui.Rect("ChainsTitle", card, new Vector2(0f, 1f), new Vector2(70f, -350f), new Vector2(1100f, 44f)),
                "Найдорожчі ланцюжки", 32f, UiFactory.Ink, TextAlignmentOptions.Left, true);
            TextMeshProUGUI chains = ui.Text(
                ui.Rect("Chains", card, new Vector2(0f, 1f), new Vector2(70f, -400f), new Vector2(1100f, 290f)),
                string.Empty, 27f, UiFactory.Ink, TextAlignmentOptions.TopLeft);

            TextMeshProUGUI verdict = ui.Text(
                ui.Rect("Verdict", card, new Vector2(0f, 0f), new Vector2(70f, 120f), new Vector2(760f, 80f)),
                string.Empty, 50f, UiFactory.Ink, TextAlignmentOptions.Left, true);

            RectTransform stamp = ui.Rect("Stamp", card, new Vector2(1f, 0f), new Vector2(-170f, 190f), new Vector2(300f, 300f));
            stamp.pivot = new Vector2(0.5f, 0.5f);
            stamp.localRotation = Quaternion.Euler(0f, 0f, -14f);
            Image stampImage = ui.Panel(stamp, FireColor, ui.Sprites.Stamp);
            TextMeshProUGUI stampLabel = ui.Text(ui.Stretch("Label", stamp), "ВИННИЙ", 54f, FireColor, bold: true);

            Button restart = RestartButton(ui, card);

            ReportView view = root.gameObject.AddComponent<ReportView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_fireDamage", fire);
            EditorAssetUtility.SetReference(view, "_firefighterDamage", firefighter);
            EditorAssetUtility.SetReference(view, "_total", total);
            EditorAssetUtility.SetReference(view, "_chains", chains);
            EditorAssetUtility.SetReference(view, "_verdict", verdict);
            EditorAssetUtility.SetReference(view, "_stamp", stamp);
            EditorAssetUtility.SetReference(view, "_stampImage", stampImage);
            EditorAssetUtility.SetReference(view, "_stampLabel", stampLabel);
            EditorAssetUtility.SetReference(view, "_restart", restart);
            root.gameObject.SetActive(false);
            return view;
        }

        private static TextMeshProUGUI Row(UiFactory ui, RectTransform card, string label, float y, Color color)
        {
            ui.Text(ui.Rect(label, card, new Vector2(0f, 1f), new Vector2(70f, y), new Vector2(700f, 56f)),
                label, 38f, UiFactory.Ink, TextAlignmentOptions.Left);
            return ui.Text(ui.Rect(label + " Value", card, new Vector2(1f, 1f), new Vector2(-70f, y), new Vector2(400f, 56f)),
                "$0", 44f, color, TextAlignmentOptions.Right, true);
        }

        private static Button RestartButton(UiFactory ui, RectTransform card)
        {
            RectTransform rect = ui.Rect("Restart", card, new Vector2(0f, 0f), new Vector2(70f, 30f), new Vector2(360f, 76f));
            Image image = ui.Panel(rect, new Color(1f, 0.55f, 0.1f));
            image.raycastTarget = true;
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ui.Text(ui.Stretch("Label", rect), "ЩЕ РАЗ  [R]", 34f, Color.white, bold: true);
            return button;
        }
    }
}
