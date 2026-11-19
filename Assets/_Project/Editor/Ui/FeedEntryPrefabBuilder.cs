using FB.Game.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Editor.Ui
{
    /// <summary>Pooled feed line prefab: dark rounded card, colored actor strip, text and damage.</summary>
    internal static class FeedEntryPrefabBuilder
    {
        private const float Height = 88f;

        public static FeedEntryView Build(UiFactory ui)
        {
            RectTransform root = new GameObject("FeedEntry", typeof(RectTransform)).GetComponent<RectTransform>();
            root.sizeDelta = new Vector2(580f, Height);
            root.gameObject.AddComponent<LayoutElement>().preferredHeight = Height;
            CanvasGroup group = ui.Group(root, false);

            RectTransform content = ui.Stretch("Content", root);
            ui.Panel(content, UiFactory.Dark);

            RectTransform badgeRect = ui.Rect("Badge", content, new Vector2(0f, 0.5f), new Vector2(8f, 0f), new Vector2(12f, Height - 16f));
            badgeRect.pivot = new Vector2(0f, 0.5f);
            Image badge = ui.Panel(badgeRect, Color.white);

            TextMeshProUGUI actor = ui.Text(
                ui.Rect("Actor", content, new Vector2(0f, 1f), new Vector2(30f, -6f), new Vector2(380f, 26f)),
                string.Empty, 20f, new Color(1f, 1f, 1f, 0.7f), TextAlignmentOptions.Left, true);
            TextMeshProUGUI text = ui.Text(
                ui.Rect("Text", content, new Vector2(0f, 1f), new Vector2(30f, -30f), new Vector2(400f, 54f)),
                string.Empty, 21f, Color.white, TextAlignmentOptions.TopLeft);
            TextMeshProUGUI damage = ui.Text(
                ui.Rect("Damage", content, new Vector2(1f, 0.5f), new Vector2(-14f, 0f), new Vector2(140f, 50f)),
                string.Empty, 30f, new Color(1f, 0.6f, 0.15f), TextAlignmentOptions.Right, true);
            damage.rectTransform.pivot = new Vector2(1f, 0.5f);

            FeedEntryView view = root.gameObject.AddComponent<FeedEntryView>();
            EditorAssetUtility.SetReference(view, "_group", group);
            EditorAssetUtility.SetReference(view, "_content", content);
            EditorAssetUtility.SetReference(view, "_badge", badge);
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
