using FB.Game.Effects;
using FB.Game.Fire;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>Pooled effect prefabs: fire (flames + smoke + light), steam puff, spark burst, water splash.</summary>
    internal static class FxPrefabBuilder
    {
        public static void BuildAll(Material water)
        {
            Material flame = ParticleMaterialFactory.Create(AssetPaths.Materials + "/M_Flame.mat",
                TextureLibrary.FlameFlipbook(), Color.white);
            Texture2D softCircle = TextureLibrary.SoftCircle();
            Material smoke = ParticleMaterialFactory.Create(AssetPaths.Materials + "/M_Smoke.mat", softCircle, Color.white);
            Material spark = ParticleMaterialFactory.Create(AssetPaths.Materials + "/M_Spark.mat", softCircle,
                new Color(1f, 0.9f, 0.4f));

            BuildFire(flame, smoke);
            BuildOneShot<SteamPuff>(FireFxFactory.Steam(smoke), AssetPaths.SteamPrefab);
            BuildOneShot<SparkBurst>(FireFxFactory.Sparks(spark), AssetPaths.SparksPrefab);
            BuildOneShot<WaterSplash>(WaterParticleFactory.CreateSplash(null, water), AssetPaths.SplashPrefab);
        }

        private static void BuildFire(Material flame, Material smoke)
        {
            GameObject root = new GameObject("FireVfx");
            ParticleSystem flames = FireFxFactory.Flames(root.transform, flame);
            ParticleSystem smokeParticles = FireFxFactory.Smoke(root.transform, smoke);
            smokeParticles.transform.localPosition = new Vector3(0f, 0.4f, 0f);

            GameObject lightObject = new GameObject("FireLight");
            lightObject.transform.SetParent(root.transform, false);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.55f, 0.2f);
            light.range = 4f;
            light.intensity = 2.2f;
            light.shadows = LightShadows.None;
            light.enabled = false;

            FireVfx vfx = root.AddComponent<FireVfx>();
            EditorAssetUtility.SetReference(vfx, "_flames", flames);
            EditorAssetUtility.SetReference(vfx, "_smoke", smokeParticles);
            EditorAssetUtility.SetReference(vfx, "_light", light);
            Save(root, AssetPaths.FireVfxPrefab);
        }

        private static void BuildOneShot<T>(ParticleSystem particles, string path) where T : OneShotEffect
        {
            T effect = particles.gameObject.AddComponent<T>();
            EditorAssetUtility.SetReference(effect, "_particles", particles);
            Save(particles.gameObject, path);
        }

        private static void Save(GameObject root, string path)
        {
            EditorAssetUtility.EnsureFolder(AssetPaths.Prefabs);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
