using FB.Game.Hose;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    internal static class SplashPrefabBuilder
    {
        public static void Build(Material waterMaterial)
        {
            ParticleSystem particles = WaterParticleFactory.CreateSplash(null, waterMaterial);
            WaterSplash splash = particles.gameObject.AddComponent<WaterSplash>();
            EditorAssetUtility.SetReference(splash, "_particles", particles);

            EditorAssetUtility.EnsureFolder(AssetPaths.Prefabs);
            PrefabUtility.SaveAsPrefabAsset(particles.gameObject, AssetPaths.SplashPrefab);
            Object.DestroyImmediate(particles.gameObject);
        }
    }
}
