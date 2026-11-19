using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>
    /// The damage bill card: red header band, revealable rows (fire, firefighters, total, blame chain cards + top events), verdict,
    /// rubber stamp and the restart button.
    /// </summary>
    internal static class ReportUiBuilder
    {
        private const float Left = 70f;
        private const float RowWidth = 1100f;
        private static readonly Color FirefighterColor = new Color(0.15f, 0.45f, 0.85f);
        private static readonly Vector2 CardSize = new Vector2(1240f, 920f);

        public static ReportView Build(UiFactory ui, Transform canvas)
        {
            RectTransform root = ui.Stretch("Report", canvas);
            CanvasGroup group = ui.Group(root, true);
            ui.Fill(root, new Color(0.05f, 0.02f, 0.02f, 0.78f));

            RectTransform card = ui.Centered("Card", root, new Vector2(0.5f, 0.5f), Vector2.zero, CardSize);
            ui.Card(card, UiFactory.Paper, 0.65f);
            Header(ui, card);

            CanvasGroup fireRow = Row(ui, card, "Збитки від пожежі", -180f, UiFactory.Ember, out TextMeshProUGUI fire);
            CanvasGroup firefighterRow = Row(ui, card, "Збитки від пожежників", -248f, FirefighterColor,
                out TextMeshProUGUI firefighter);
            CanvasGroup totalRow = Row(ui, card, "Разом", -332f, UiFactory.Ink, out TextMeshProUGUI total);
            RectTransform divider = ui.Rect("Divider", totalRow.transform, new Vector2(0f, 1f), new Vector2(0f, 10f), new Vector2(RowWidth, 3f));
            ui.Fill(divider, new Color(UiFactory.Ink.r, UiFactory.Ink.g, UiFactory.Ink.b, 0.25f));
            total.fontSize = 54f;
            total.rectTransform.pivot = new Vector2(1f, 0.5f);

            CanvasGroup chainsRow = Chains(ui, card, out TextMeshProUGUI chains, out ChainLinkView[] links);

            RectTransform verdictRect = ui.Rect("Verdict", card, new Vector2(0f, 0f), new Vector2(Left, 160f), new Vector2(760f, 100f));
            verdictRect.pivot = new Vector2(0f, 0.5f);
            TextMeshProUGUI verdict = ui.Text(verdictRect, string.Empty, 48f, UiFactory.Ink, TextAlignmentOptions.Left, true);

            RectTransform stamp = ui.Centered("Stamp", card, new Vector2(1f, 0f), new Vector2(-180f, 190f), new Vector2(300f, 300f));
            stamp.localRotation = Quaternion.Euler(0f, 0f, -14f);
            Image stampImage = ui.Panel(stamp, UiFactory.Ember, ui.Sprites.Stamp);
            TextMeshProUGUI stampLabel = ui.Text(ui.Stretch("Label", stamp), "ВИННИЙ", 54f, UiFactory.Ember, bold: true);

            Button restart = RestartButton(ui, card);

            ReportView view = root.gameObject.AddComponent<ReportView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_card", card);
            EditorAssetUtility.SetArray(view, "_rows", new Object[] { fireRow, firefighterRow, totalRow, chainsRow });
            EditorAssetUtility.SetReference(view, "_fireDamage", fire);
            EditorAssetUtility.SetReference(view, "_firefighterDamage", firefighter);
            EditorAssetUtility.SetReference(view, "_total", total);
            EditorAssetUtility.SetReference(view, "_chains", chains);
            EditorAssetUtility.SetArray(view, "_links", links);
            EditorAssetUtility.SetReference(view, "_verdict", verdict);
            EditorAssetUtility.SetReference(view, "_stamp", stamp);
            EditorAssetUtility.SetReference(view, "_stampImage", stampImage);
            EditorAssetUtility.SetReference(view, "_stampLabel", stampLabel);
            EditorAssetUtility.SetReference(view, "_restart", restart);
            root.gameObject.SetActive(false);
            return view;
        }

        /// <summary>Red rounded band across the top; a plain strip squares off its lower corners.</summary>
        private static void Header(UiFactory ui, RectTransform card)
        {
            RectTransform band = ui.Rect("Header", card, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(CardSize.x, 120f));
            ui.Panel(band, UiFactory.Ember);
            RectTransform square = ui.Rect("HeaderBase", card, new Vector2(0.5f, 1f), new Vector2(0f, -80f), new Vector2(CardSize.x, 40f));
            ui.Fill(square, UiFactory.Ember);
            ui.Text(ui.Stretch("Title", band), "РАХУНОК ЗА ВИКЛИК", 60f, Color.white, bold: true, overWorld: true);
        }

        private static CanvasGroup Row(UiFactory ui, RectTransform card, string label, float y, Color color,
            out TextMeshProUGUI value)
        {
            RectTransform row = ui.Rect(label, card, new Vector2(0f, 1f), new Vector2(Left, y), new Vector2(RowWidth, 60f));
            CanvasGroup group = ui.Group(row, false);
            ui.Text(ui.Rect("Label", row, new Vector2(0f, 0.5f), Vector2.zero, new Vector2(700f, 60f)),
                label, 36f, UiFactory.Ink, TextAlignmentOptions.Left);
            value = ui.Text(ui.Rect("Value", row, new Vector2(1f, 0.5f), Vector2.zero, new Vector2(420f, 64f)),
                "$0", 44f, color, TextAlignmentOptions.Right, true);
            return group;
        }

        private static CanvasGroup Chains(UiFactory ui, RectTransform card, out TextMeshProUGUI chains,
            out ChainLinkView[] links)
        {
            RectTransform block = ui.Rect("ChainsBlock", card, new Vector2(0f, 1f), new Vector2(Left, -410f), new Vector2(RowWidth, 300f));
            CanvasGroup group = ui.Group(block, false);
            ui.Text(ui.Rect("Title", block, new Vector2(0f, 1f), Vector2.zero, new Vector2(RowWidth, 40f)),
                "ЛАНЦЮЖОК ПРОВИНИ", 28f, UiFactory.Ember, TextAlignmentOptions.Left, true);
            RectTransform linkRow = ui.Rect("Links", block, new Vector2(0f, 1f), new Vector2(0f, -46f), new Vector2(RowWidth, 96f));
            links = ChainUiBuilder.Build(ui, linkRow, RowWidth);
            chains = ui.Text(ui.Rect("Chains", block, new Vector2(0f, 1f), new Vector2(0f, -160f), new Vector2(RowWidth, 140f)),
                string.Empty, 24f, UiFactory.Ink, TextAlignmentOptions.TopLeft);
            return group;
        }

        private static Button RestartButton(UiFactory ui, RectTransform card)
        {
            RectTransform rect = ui.Centered("Restart", card, new Vector2(0f, 0f), new Vector2(Left + 190f, 72f), new Vector2(380f, 82f));
            Image face = ui.Card(rect, UiFactory.Fire, 0.45f, ui.Sprites.Pill);
            face.raycastTarget = true;
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = face;
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1f, 0.92f, 0.8f);
            colors.pressedColor = new Color(0.85f, 0.7f, 0.6f);
            button.colors = colors;
            ui.Text(ui.Stretch("Label", rect), "ЩЕ РАЗ  [R]", 34f, Color.white, bold: true, overWorld: true);
            return button;
        }
    }
}
