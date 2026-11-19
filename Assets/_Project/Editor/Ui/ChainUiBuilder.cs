using FB.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>
    /// A row of pre-built blame chain cards (no instantiation at runtime): white card with a colored top strip,
    /// actor in the display font, action below, and an arrow towards the next card.
    /// </summary>
    internal static class ChainUiBuilder
    {
        private const int Slots = 6;
        private const float CardWidth = 150f;
        private const float CardHeight = 96f;

        public static ChainLinkView[] Build(UiFactory ui, RectTransform parent, float width)
        {
            float slot = width / Slots;
            ChainLinkView[] links = new ChainLinkView[Slots];
            for (int i = 0; i < Slots; i++)
            {
                links[i] = Link(ui, parent, i, new Vector2(i * slot, 0f), slot);
            }

            return links;
        }

        private static ChainLinkView Link(UiFactory ui, RectTransform parent, int index, Vector2 position, float slot)
        {
            RectTransform root = ui.Rect($"Link{index}", parent, new Vector2(0f, 1f), position, new Vector2(slot, CardHeight));
            CanvasGroup group = ui.Group(root, false);

            RectTransform card = ui.Centered("Card", root, new Vector2(0f, 0.5f), new Vector2(CardWidth * 0.5f, 0f),
                new Vector2(CardWidth, CardHeight));
            ui.Card(card, Color.white, 0.3f);
            RectTransform stripRect = ui.Centered("Strip", card, new Vector2(0.5f, 1f), new Vector2(0f, -9f), new Vector2(CardWidth - 20f, 8f));
            Image strip = ui.Panel(stripRect, Color.white, ui.Sprites.Pill);

            TextMeshProUGUI title = ui.Text(ui.Centered("Title", card, new Vector2(0.5f, 1f), new Vector2(0f, -36f), new Vector2(CardWidth - 12f, 34f)),
                string.Empty, 19f, UiFactory.Ink, bold: true);
            title.textWrappingMode = TextWrappingModes.NoWrap;
            title.enableAutoSizing = true;
            title.fontSizeMin = 12f;
            title.fontSizeMax = 19f;
            TextMeshProUGUI subtitle = ui.Text(ui.Centered("Subtitle", card, new Vector2(0.5f, 0f), new Vector2(0f, 26f), new Vector2(CardWidth - 12f, 40f)),
                string.Empty, 17f, new Color(0.35f, 0.3f, 0.3f));

            RectTransform arrowRect = ui.Rect("Arrow", root, new Vector2(1f, 0.5f), Vector2.zero, new Vector2(slot - CardWidth, CardHeight));
            arrowRect.pivot = new Vector2(1f, 0.5f);
            TextMeshProUGUI arrow = ui.Text(arrowRect, "→", 36f, UiFactory.Ember, bold: true);

            ChainLinkView view = root.gameObject.AddComponent<ChainLinkView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_card", card);
            EditorAssetUtility.SetReference(view, "_strip", strip);
            EditorAssetUtility.SetReference(view, "_title", title);
            EditorAssetUtility.SetReference(view, "_subtitle", subtitle);
            EditorAssetUtility.SetReference(view, "_arrow", arrow);
            return view;
        }
    }
}
