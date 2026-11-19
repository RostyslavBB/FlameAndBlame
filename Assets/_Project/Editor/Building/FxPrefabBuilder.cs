using FB.Game.Effects;
using FB.Game.Fire;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>Pooled effect prefabs: fire (glow, flames, core, embers, smoke, light), steam puff, spark burst, water splash.</summary>
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

            Material flameCore = ParticleMaterialFactory.Create(AssetPaths.Materials + "/M_FlameCore.mat",
                TextureLibrary.FlameFlipbook(), new Color(2.2f, 1.6f, 0.9f), true);
            Material glow = ParticleMaterialFactory.Create(AssetPaths.Materials + "/M_FireGlow.mat", softCircle,
                new Color(1.6f, 0.7f, 0.25f), true);
            Material ember = ParticleMaterialFactory.Create(AssetPaths.Materials + "/M_Ember.mat", softCircle,
                new Color(3f, 1.4f, 0.4f), true);

            BuildFire(flame, flameCore, glow, ember, smoke);
            BuildOneShot<SteamPuff>(FireFxFactory.Steam(smoke), AssetPaths.SteamPrefab);
            BuildOneShot<SparkBurst>(FireFxFactory.Sparks(spark), AssetPaths.SparksPrefab);
            BuildOneShot<WaterSplash>(WaterParticleFactory.CreateSplash(null, water), AssetPaths.SplashPrefab);
        }

        private static void BuildFire(Material flame, Material core, Material glow, Material ember, Material smoke)
        {
            GameObject root = new GameObject("FireVfx");
            ParticleSystem glowParticles = FireFxFactory.Glow(root.transform, glow);
            ParticleSystem flames = FireFxFactory.Flames(root.transform, flame);
            ParticleSystem coreParticles = FireFxFactory.Core(root.transform, core);
            ParticleSystem embers = FireFxFactory.Embers(root.transform, ember);
            ParticleSystem smokeParticles = FireFxFactory.Smoke(root.transform, smoke);
            smokeParticles.transform.localPosition = new Vector3(0f, 0.5f, 0f);

            GameObject lightObject = new GameObject("FireLight");
            lightObject.transform.SetParent(root.transform, false);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.55f, 0.2f);
            light.range = 5f;
            light.intensity = 2.6f;
            light.shadows = LightShadows.None;
            light.enabled = false;

            FireVfx vfx = root.AddComponent<FireVfx>();
            EditorAssetUtility.SetArray(vfx, "_emitters",
                new Object[] { glowParticles, flames, coreParticles, embers, smokeParticles });
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
