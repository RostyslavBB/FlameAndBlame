using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>Small helpers to build uGUI hierarchies in code with the project font and sprites.</summary>
    internal sealed class UiFactory
    {
        public static readonly Color Ink = new Color(0.13f, 0.12f, 0.14f);
        public static readonly Color Paper = new Color(1f, 0.96f, 0.88f);
        public static readonly Color Dark = new Color(0.08f, 0.08f, 0.1f, 0.85f);

        private readonly TMP_FontAsset _font;

        public UiFactory(TMP_FontAsset font, UiSpriteLibrary sprites)
        {
            _font = font;
            Sprites = sprites;
        }

        public UiSpriteLibrary Sprites { get; }

        public RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
        {
            RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        public RectTransform Stretch(string name, Transform parent)
        {
            RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        public Image Panel(RectTransform rect, Color color, Sprite sprite = null)
        {
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite != null ? sprite : Sprites.Rounded;
            image.type = image.sprite.border.sqrMagnitude > 0f ? Image.Type.Sliced : Image.Type.Simple;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        /// <summary>Plain full-color rectangle (screen dimmers).</summary>
        public Image Fill(RectTransform rect, Color color)
        {
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        public TextMeshProUGUI Text(RectTransform rect, string text, float size, Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.Center, bool bold = false)
        {
            TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.font = _font;
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            label.textWrappingMode = TextWrappingModes.Normal;
            label.raycastTarget = false;
            return label;
        }

        public CanvasGroup Group(RectTransform rect, bool blocksRaycasts)
        {
            CanvasGroup group = rect.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = blocksRaycasts;
            group.interactable = blocksRaycasts;
            return group;
        }
    }
}
