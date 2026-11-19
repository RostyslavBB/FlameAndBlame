using FB.Editor.Configs;
using FB.Game.Installers;
using FB.Game.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Zenject;

namespace FB.Editor.Building
{
    /// <summary>Creates Game.unity from scratch: light, ambient, level, player, SceneContext.</summary>
    internal static class GameSceneBuilder
    {
        public static void Build(LevelBuilder level)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            // NewScene(Single) unloads unreferenced assets: reload configs so references are not stale.
            ConfigAssets configs = ConfigAssets.Ensure();
            SetupLighting();
            level.Rebuild(scene);

            // Load from disk: references returned by SaveAsPrefabAsset may be invalidated by asset reimport.
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetPaths.PlayerPrefab);
            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, scene);
            PlacePlayer(player.transform, configs);

            GameObject contextObject = new GameObject("SceneContext");
            GameplayInstaller installer = contextObject.AddComponent<GameplayInstaller>();
            EditorAssetUtility.SetReference(installer, "_palette", configs.Palette);
            EditorAssetUtility.SetReference(installer, "_playerConfig", configs.Player);
            EditorAssetUtility.SetReference(installer, "_roundConfig", configs.Round);
            EditorAssetUtility.SetReference(installer, "_player", player.GetComponent<PlayerBody>());
            SceneContext context = contextObject.AddComponent<SceneContext>();
            context.Installers = new MonoInstaller[] { installer };

            EditorAssetUtility.EnsureFolder(AssetPaths.Scenes);
            EditorSceneManager.SaveScene(scene, AssetPaths.GameScene);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(AssetPaths.GameScene, true) };
        }

        public static void PlacePlayer(Transform player, ConfigAssets configs)
        {
            player.SetPositionAndRotation(configs.Layout.SpawnPosition, Quaternion.Euler(0f, configs.Layout.SpawnYaw, 0f));
        }

        private static void SetupLighting()
        {
            LightingSettings settings = AssetDatabase.LoadAssetAtPath<LightingSettings>(AssetPaths.LightingSettings);
            if (settings == null)
            {
                settings = new LightingSettings { name = "GameLighting" };
                AssetDatabase.CreateAsset(settings, AssetPaths.LightingSettings);
            }

            settings.bakedGI = false;
            settings.realtimeGI = false;
            Lightmapping.lightingSettings = settings;

            RenderSettings.skybox = null;
            RenderSettings.fog = false;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.78f, 0.74f, 0.70f);
            RenderSettings.ambientEquatorColor = new Color(0.62f, 0.55f, 0.50f);
            RenderSettings.ambientGroundColor = new Color(0.40f, 0.34f, 0.30f);

            GameObject sun = new GameObject("Sun");
            sun.transform.rotation = Quaternion.Euler(58f, -35f, 0f);
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.92f, 0.80f);
            light.intensity = 1.25f;
            light.shadows = LightShadows.Hard;
            light.shadowStrength = 0.55f;
        }
    }
}
