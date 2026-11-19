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

            // Warm "evening fire" grade: golden highlights, brown-orange shadows, soft orange glow.
            Bloom bloom = Add<Bloom>(profile);
            bloom.threshold.Override(0.85f);
            bloom.intensity.Override(1.15f);
            bloom.scatter.Override(0.7f);
            bloom.tint.Override(new Color(1f, 0.78f, 0.55f));

            ColorAdjustments color = Add<ColorAdjustments>(profile);
            color.postExposure.Override(0.2f);
            color.contrast.Override(18f);
            color.saturation.Override(12f);
            color.colorFilter.Override(new Color(1f, 0.93f, 0.84f));

            WhiteBalance balance = Add<WhiteBalance>(profile);
            balance.temperature.Override(16f);
            balance.tint.Override(4f);

            ShadowsMidtonesHighlights tones = Add<ShadowsMidtonesHighlights>(profile);
            tones.shadows.Override(new Vector4(1f, 0.86f, 0.74f, -0.02f));
            tones.midtones.Override(new Vector4(1f, 0.96f, 0.9f, 0f));
            tones.highlights.Override(new Vector4(1f, 0.95f, 0.85f, 0.03f));

            Vignette vignette = Add<Vignette>(profile);
            vignette.intensity.Override(0.3f);
            vignette.smoothness.Override(0.5f);
            vignette.color.Override(new Color(0.2f, 0.07f, 0.02f));
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

        /// <summary>Electric shock pulse: chromatic aberration, lens kick and a cold overexposed flash.</summary>
        public static VolumeProfile ShockProfile()
        {
            VolumeProfile profile = Recreate(AssetPaths.ShockPostProcess);
            ChromaticAberration aberration = Add<ChromaticAberration>(profile);
            aberration.intensity.Override(1f);

            LensDistortion distortion = Add<LensDistortion>(profile);
            distortion.intensity.Override(-0.35f);
            distortion.scale.Override(1.05f);

            ColorAdjustments color = Add<ColorAdjustments>(profile);
            color.postExposure.Override(0.7f);
            color.saturation.Override(-35f);
            color.colorFilter.Override(new Color(0.65f, 0.88f, 1f));

            Vignette vignette = Add<Vignette>(profile);
            vignette.intensity.Override(0.45f);
            vignette.color.Override(new Color(0.1f, 0.35f, 0.8f));
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
