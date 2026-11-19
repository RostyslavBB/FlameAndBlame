using UnityEditor;
using UnityEngine;

namespace FB.Editor
{
    /// <summary>Idempotent asset helpers: create once, update in place (GUIDs stay stable between rebuilds).</summary>
    internal static class EditorAssetUtility
    {
        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            int slash = path.LastIndexOf('/');
            string parent = path.Substring(0, slash);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, path.Substring(slash + 1));
        }

        public static T LoadOrCreate<T>(string path, out bool created) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            created = asset == null;
            if (created)
            {
                EnsureFolder(path.Substring(0, path.LastIndexOf('/')));
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }

            return asset;
        }

        /// <summary>Saves a generated object, or copies it over the existing asset to keep references intact.</summary>
        public static T SaveOrReplace<T>(T generated, string path) where T : Object
        {
            T existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing == null)
            {
                EnsureFolder(path.Substring(0, path.LastIndexOf('/')));
                AssetDatabase.CreateAsset(generated, path);
                return generated;
            }

            EditorUtility.CopySerialized(generated, existing);
            Object.DestroyImmediate(generated);
            EditorUtility.SetDirty(existing);
            return existing;
        }

        public static void SetReference(Object target, string field, Object value) =>
            Edit(target, field, property => property.objectReferenceValue = value);

        public static void SetString(Object target, string field, string value) =>
            Edit(target, field, property => property.stringValue = value);

        public static void SetInt(Object target, string field, int value) =>
            Edit(target, field, property => property.intValue = value);

        public static void SetArray(Object target, string field, Object[] values) =>
            Edit(target, field, property =>
            {
                property.arraySize = values.Length;
                for (int i = 0; i < values.Length; i++)
                {
                    property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
                }
            });

        private static void Edit(Object target, string field, System.Action<SerializedProperty> edit)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null)
            {
                Debug.LogError($"[Build] Field '{field}' not found on {target.GetType().Name}.");
                return;
            }

            edit(property);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
