using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>Small helpers to build uGUI hierarchies in code with the project fonts, palette and sprites.</summary>
    internal sealed class UiFactory
    {
        public static readonly Color Ink = new Color(0.14f, 0.1f, 0.12f);
        public static readonly Color Paper = new Color(1f, 0.96f, 0.88f);
        public static readonly Color PaperShade = new Color(0.96f, 0.88f, 0.76f);
        public static readonly Color Dark = new Color(0.1f, 0.08f, 0.11f, 0.88f);
        public static readonly Color Fire = new Color(1f, 0.42f, 0.12f);
        public static readonly Color Ember = new Color(0.86f, 0.2f, 0.15f);
        public static readonly Color Gold = new Color(1f, 0.82f, 0.3f);
        public static readonly Color Water = new Color(0.35f, 0.75f, 1f);

        private readonly UiFonts _fonts;

        public UiFactory(UiFonts fonts, UiSpriteLibrary sprites)
        {
            _fonts = fonts;
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

        /// <summary>Like <see cref="Rect"/> but pivoted in the middle, so it scales and rotates around its center.</summary>
        public RectTransform Centered(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
        {
            RectTransform rect = Rect(name, parent, anchor, position, size);
            rect.pivot = new Vector2(0.5f, 0.5f);
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

        /// <summary>Panel with a soft drop shadow behind it (the shadow is a sibling child stretched past the edges).</summary>
        public Image Card(RectTransform rect, Color color, float shadowAlpha = 0.45f, Sprite sprite = null)
        {
            RectTransform shadow = Stretch("Shadow", rect);
            shadow.offsetMin = new Vector2(-26f, -34f);
            shadow.offsetMax = new Vector2(26f, 18f);
            Image shadowImage = Panel(shadow, new Color(0f, 0f, 0f, shadowAlpha), Sprites.Shadow);
            shadowImage.transform.SetAsFirstSibling();

            RectTransform face = Stretch("Face", rect);
            face.SetSiblingIndex(1);
            return Panel(face, color, sprite);
        }

        /// <summary>Plain full-color rectangle (screen dimmers).</summary>
        public Image Fill(RectTransform rect, Color color)
        {
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        /// <summary>
        /// <paramref name="bold"/> picks the display face (Russo One) instead of the body face (Rubik);
        /// <paramref name="overWorld"/> adds the outline/shadow preset for text drawn straight over the 3D view.
        /// </summary>
        public TextMeshProUGUI Text(RectTransform rect, string text, float size, Color color,
            TextAlignmentOptions alignment = TextAlignmentOptions.Center, bool bold = false, bool overWorld = false)
        {
            TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.font = bold ? _fonts.Display : _fonts.Body;
            if (overWorld)
            {
                label.fontSharedMaterial = bold ? _fonts.DisplayOutlined : _fonts.BodyShadow;
            }

            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.fontStyle = FontStyles.Normal;
            label.characterSpacing = bold ? 2f : 0f;
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
