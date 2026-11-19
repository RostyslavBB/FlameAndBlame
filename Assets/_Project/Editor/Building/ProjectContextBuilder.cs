using FB.Core.Installers;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace FB.Editor.Building
{
    /// <summary>Resources/ProjectContext.prefab, which Zenject loads automatically before the first scene.</summary>
    internal static class ProjectContextBuilder
    {
        public static void Build()
        {
            GameObject root = new GameObject("ProjectContext");
            ProjectInstaller installer = root.AddComponent<ProjectInstaller>();
            ProjectContext context = root.AddComponent<ProjectContext>();
            context.Installers = new MonoInstaller[] { installer };

            EditorAssetUtility.EnsureFolder(AssetPaths.Resources);
            PrefabUtility.SaveAsPrefabAsset(root, AssetPaths.ProjectContextPrefab);
            Object.DestroyImmediate(root);
        }
    }
}
