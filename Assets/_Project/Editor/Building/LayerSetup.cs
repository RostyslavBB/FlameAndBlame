using FB.Game.Common;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>Registers project physics layers in ProjectSettings/TagManager.asset.</summary>
    internal static class LayerSetup
    {
        public static void Ensure()
        {
            Object tagManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            SerializedObject serialized = new SerializedObject(tagManager);
            SerializedProperty layer = serialized.FindProperty("layers").GetArrayElementAtIndex(GameLayers.Player);
            if (layer.stringValue == GameLayers.PlayerName)
            {
                return;
            }

            layer.stringValue = GameLayers.PlayerName;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetLayerRecursively(GameObject root, int layer)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = layer;
            }
        }
    }
}
