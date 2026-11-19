using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FB.Editor.Building
{
    /// <summary>
    /// Volume profiles generated in code: the main cinematic grade (ACES, bloom for fire and water glints,
    /// warm balance, vignette) and the "smoke" grade blended in by SmokeService as the apartment fills with smoke.
    /// Profiles are rebuilt on every Build All, so the values here are the source of truth.
    /// </summary>
    internal static class PostProcessBuilder
    {
        public static VolumeProfile MainProfile()
        {
            VolumeProfile profile = Recreate(AssetPaths.MainPostProcess);
            Tonemapping tonemapping = Add<Tonemapping>(profile);
            tonemapping.mode.Override(TonemappingMode.ACES);

            Bloom bloom = Add<Bloom>(profile);
            bloom.threshold.Override(0.95f);
            bloom.intensity.Override(0.9f);
            bloom.scatter.Override(0.65f);
            bloom.tint.Override(new Color(1f, 0.9f, 0.8f));

            ColorAdjustments color = Add<ColorAdjustments>(profile);
            color.postExposure.Override(0.25f);
            color.contrast.Override(14f);
            color.saturation.Override(10f);

            WhiteBalance balance = Add<WhiteBalance>(profile);
            balance.temperature.Override(3f);

            Vignette vignette = Add<Vignette>(profile);
            vignette.intensity.Override(0.24f);
            vignette.smoothness.Override(0.45f);
            return Save(profile);
        }

        public static VolumeProfile SmokeProfile()
        {
            VolumeProfile profile = Recreate(AssetPaths.SmokePostProcess);
            ColorAdjustments color = Add<ColorAdjustments>(profile);
            color.postExposure.Override(-0.35f);
            color.contrast.Override(-12f);
            color.saturation.Override(-40f);
            color.colorFilter.Override(new Color(1f, 0.88f, 0.78f));

            Vignette vignette = Add<Vignette>(profile);
            vignette.intensity.Override(0.5f);
            vignette.smoothness.Override(0.6f);
            vignette.color.Override(new Color(0.12f, 0.06f, 0.03f));

            Bloom bloom = Add<Bloom>(profile);
            bloom.intensity.Override(1.6f);
            bloom.scatter.Override(0.8f);
            return Save(profile);
        }

        public static Volume CreateVolume(string name, VolumeProfile profile, float priority, float weight)
        {
            GameObject go = new GameObject(name);
            Volume volume = go.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = priority;
            volume.weight = weight;
            volume.sharedProfile = profile;
            return volume;
        }

        private static VolumeProfile Recreate(string path)
        {
            AssetDatabase.DeleteAsset(path);
            VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);
            return profile;
        }

        private static T Add<T>(VolumeProfile profile) where T : VolumeComponent
        {
            T component = profile.Add<T>(true);
            component.name = typeof(T).Name;
            AssetDatabase.AddObjectToAsset(component, profile);
            return component;
        }

        private static VolumeProfile Save(VolumeProfile profile)
        {
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<VolumeProfile>(AssetDatabase.GetAssetPath(profile));
        }
    }
}
