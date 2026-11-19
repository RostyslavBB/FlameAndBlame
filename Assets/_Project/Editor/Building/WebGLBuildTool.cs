using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FB.Editor.Building
{
    /// <summary>WebGL player settings (itch.io friendly) and a one-click WebGL build.</summary>
    internal static class WebGLBuildTool
    {
        public static void ApplySettings()
        {
            PlayerSettings.productName = "Flame and Blame";
            PlayerSettings.runInBackground = true;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Minimal);

            DisableScreenSpaceAmbientOcclusion();
            TunePipelineAssets();
            AssetDatabase.SaveAssets();
            Debug.Log("[Build] WebGL settings applied: Gzip + decompression fallback, Minimal stripping, no SSAO.");
        }

        public static bool Build()
        {
            ApplySettings();
            return BuildPlayer(BuildTarget.WebGL, BuildTargetGroup.WebGL, AssetPaths.WebGLBuildFolder);
        }

        /// <summary>Windows fallback build (also used for headless smoke tests of the runtime).</summary>
        public static bool BuildWindows() =>
            BuildPlayer(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, AssetPaths.WindowsBuildExe);

        private static bool BuildPlayer(BuildTarget target, BuildTargetGroup group, string location)
        {
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { AssetPaths.GameScene },
                locationPathName = location,
                target = target,
                targetGroup = group,
                options = BuildOptions.None
            };

            BuildSummary summary = BuildPipeline.BuildPlayer(options).summary;
            Debug.Log($"[Build] {target}: {summary.result}, {summary.totalSize / (1024 * 1024)} MB, " +
                      $"{summary.totalErrors} errors, {summary.totalTime.TotalSeconds:F0}s -> {location}");
            return summary.result == BuildResult.Succeeded;
        }

        private static void DisableScreenSpaceAmbientOcclusion()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:UniversalRendererData"))
            {
                ScriptableRendererData data = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(
                    AssetDatabase.GUIDToAssetPath(guid));
                foreach (ScriptableRendererFeature feature in data.rendererFeatures)
                {
                    if (feature != null && feature.GetType().Name.Contains("ScreenSpaceAmbientOcclusion"))
                    {
                        feature.SetActive(false);
                        EditorUtility.SetDirty(feature);
                    }
                }

                EditorUtility.SetDirty(data);
            }
        }

        private static void TunePipelineAssets()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
            {
                UniversalRenderPipelineAsset asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(
                    AssetDatabase.GUIDToAssetPath(guid));
                asset.shadowDistance = 25f;
                asset.shadowCascadeCount = 1;
                EditorUtility.SetDirty(asset);
            }

            DisableShadowsForWebGL();
            EnsureDepthTexture();
        }

        /// <summary>The water shader reads scene depth (thickness, foam), so every URP asset renders a depth texture.</summary>
        public static void EnsureDepthTexture()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
            {
                UniversalRenderPipelineAsset asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(
                    AssetDatabase.GUIDToAssetPath(guid));
                asset.supportsCameraDepthTexture = true;
                EditorUtility.SetDirty(asset);
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// URP 17 on WebGL2 binds a shadow texture to a comparison sampler with a mismatching format
        /// (GL_INVALID_OPERATION), which drops every Lit draw call. The WebGL quality level renders without shadows.
        /// </summary>
        private static void DisableShadowsForWebGL()
        {
            int level = WebGLQualityLevel();
            if (!(QualitySettings.GetRenderPipelineAssetAt(level) is UniversalRenderPipelineAsset asset))
            {
                Debug.LogWarning("[Build] WebGL quality level has no URP asset; shadows left unchanged.");
                return;
            }

            SerializedObject serialized = new SerializedObject(asset);
            serialized.FindProperty("m_MainLightShadowsSupported").boolValue = false;
            serialized.FindProperty("m_AdditionalLightShadowsSupported").boolValue = false;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log($"[Build] Shadows disabled in '{asset.name}' (WebGL quality level {level}).");
        }

        private static int WebGLQualityLevel()
        {
            Object qualitySettings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0];
            SerializedProperty defaults = new SerializedObject(qualitySettings).FindProperty("m_PerPlatformDefaultQuality");
            for (int i = 0; i < defaults.arraySize; i++)
            {
                SerializedProperty entry = defaults.GetArrayElementAtIndex(i);
                if (entry.FindPropertyRelative("first").stringValue == "WebGL")
                {
                    return entry.FindPropertyRelative("second").intValue;
                }
            }

            return QualitySettings.GetQualityLevel();
        }
    }
}
