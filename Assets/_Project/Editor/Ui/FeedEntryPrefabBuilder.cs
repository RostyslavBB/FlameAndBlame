using FB.Game.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>Pooled feed line prefab: dark card with shadow, actor color strip, actor name, text, damage, flash.</summary>
    internal static class FeedEntryPrefabBuilder
    {
        private const float Width = 600f;
        private const float Height = 96f;

        public static FeedEntryView Build(UiFactory ui)
        {
            RectTransform root = new GameObject("FeedEntry", typeof(RectTransform)).GetComponent<RectTransform>();
            root.sizeDelta = new Vector2(Width, Height);
            root.gameObject.AddComponent<LayoutElement>().preferredHeight = Height;
            CanvasGroup group = ui.Group(root, false);

            RectTransform content = ui.Stretch("Content", root);
            content.pivot = new Vector2(1f, 0.5f);
            ui.Card(content, UiFactory.Dark, 0.4f);

            RectTransform badgeRect = ui.Rect("Badge", content, new Vector2(0f, 0.5f), new Vector2(10f, 0f), new Vector2(10f, Height - 20f));
            badgeRect.pivot = new Vector2(0f, 0.5f);
            Image badge = ui.Panel(badgeRect, Color.white, ui.Sprites.Pill);

            TextMeshProUGUI actor = ui.Text(
                ui.Rect("Actor", content, new Vector2(0f, 1f), new Vector2(34f, -8f), new Vector2(400f, 26f)),
                string.Empty, 19f, Color.white, TextAlignmentOptions.Left, true);
            TextMeshProUGUI text = ui.Text(
                ui.Rect("Text", content, new Vector2(0f, 1f), new Vector2(34f, -34f), new Vector2(410f, 56f)),
                string.Empty, 21f, Color.white, TextAlignmentOptions.TopLeft);
            TextMeshProUGUI damage = ui.Text(
                ui.Rect("Damage", content, new Vector2(1f, 0.5f), new Vector2(-16f, 0f), new Vector2(150f, 50f)),
                string.Empty, 30f, UiFactory.Gold, TextAlignmentOptions.Right, true);
            damage.rectTransform.pivot = new Vector2(1f, 0.5f);

            Image flash = ui.Panel(ui.Stretch("Flash", content), new Color(1f, 1f, 1f, 0f));

            FeedEntryView view = root.gameObject.AddComponent<FeedEntryView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_content", content);
            EditorAssetUtility.SetReference(view, "_badge", badge);
            EditorAssetUtility.SetReference(view, "_flash", flash);
            EditorAssetUtility.SetReference(view, "_actor", actor);
            EditorAssetUtility.SetReference(view, "_text", text);
            EditorAssetUtility.SetReference(view, "_damage", damage);

            EditorAssetUtility.EnsureFolder(AssetPaths.Prefabs);
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, AssetPaths.FeedEntryPrefab);
            Object.DestroyImmediate(root.gameObject);
            return AssetDatabase.LoadAssetAtPath<FeedEntryView>(AssetPaths.FeedEntryPrefab);
        }
    }
}
