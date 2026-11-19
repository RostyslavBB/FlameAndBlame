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
            SerializedProperty layers = serialized.FindProperty("layers");
            bool changed = SetName(layers, GameLayers.Player, GameLayers.PlayerName);
            changed |= SetName(layers, GameLayers.Civilian, GameLayers.CivilianName);
            if (changed)
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public static void SetLayerRecursively(GameObject root, int layer)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = layer;
            }
        }

        private static bool SetName(SerializedProperty layers, int index, string name)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(index);
            if (layer.stringValue == name)
            {
                return false;
            }

            layer.stringValue = name;
            return true;
        }
    }
}
