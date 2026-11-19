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
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { AssetPaths.GameScene },
                locationPathName = AssetPaths.WebGLBuildFolder,
                target = BuildTarget.WebGL,
                targetGroup = BuildTargetGroup.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            Debug.Log($"[Build] WebGL: {summary.result}, {summary.totalSize / (1024 * 1024)} MB, " +
                      $"{summary.totalErrors} errors, {summary.totalTime.TotalSeconds:F0}s → {AssetPaths.WebGLBuildFolder}");
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
        }
    }
}
