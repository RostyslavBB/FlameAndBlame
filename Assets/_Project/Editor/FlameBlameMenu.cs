using FB.Editor.Building;
using FB.Editor.Configs;
using FB.Game.Config;
using FB.Game.Hose;
using FB.Game.Installers;
using FB.Game.Player;
using FB.Game.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FB.Editor
{
    /// <summary>Tools/Flame&amp;Blame menu. Also the entry points for batch mode (-executeMethod).</summary>
    public static class FlameBlameMenu
    {
        private const string Menu = "Tools/Flame&Blame/";

        [MenuItem(Menu + "Build All", priority = 0)]
        public static void BuildAll()
        {
            LayerSetup.Ensure();
            ConfigAssets configs = ConfigAssets.Ensure();
            MaterialLibrary materials = new MaterialLibrary(configs.Palette);
            MeshLibrary meshes = new MeshLibrary();
            Material water = WaterParticleFactory.CreateMaterial(TextureLibrary.Droplet(),
                configs.Palette.Get(PaletteColor.Water));

            PlayerPrefabBuilder.Build(materials, meshes, water);
            FxPrefabBuilder.BuildAll(water);
            PowerPrefabBuilder.Build(materials, meshes);
            ProjectContextBuilder.Build();
            GameSceneBuilder.Build(materials, meshes);
            AssetDatabase.SaveAssets();
            Debug.Log("[Build] Build All finished: configs, materials, meshes, textures, prefabs, Game.unity.");
        }

        [MenuItem(Menu + "Rebuild Level", priority = 1)]
        public static void RebuildLevel()
        {
            ConfigAssets configs = ConfigAssets.Ensure();
            Scene scene = EditorSceneManager.OpenScene(AssetPaths.GameScene, OpenSceneMode.Single);
            LevelCatalog levelCatalog = CreateLevelBuilder(configs).Rebuild(scene);
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.TryGetComponent(out PlayerBody player))
                {
                    GameSceneBuilder.PlacePlayer(player.transform, configs);
                }

                if (root.name == HoseIds.Anchor)
                {
                    GameSceneBuilder.PlaceHoseAnchor(root.transform, configs);
                }

                if (root.TryGetComponent(out GameplayInstaller installer))
                {
                    EditorAssetUtility.SetReference(installer, "_level", levelCatalog);
                }
            }

            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Build] Level rebuilt from LevelLayoutConfig.");
        }

        [MenuItem(Menu + "Reset Level Layout To Defaults", priority = 2)]
        public static void ResetLevelLayout()
        {
            ConfigAssets configs = ConfigAssets.Ensure();
            LevelLayoutDefaults.Apply(configs.Layout);
            EditorUtility.SetDirty(configs.Layout);
            AssetDatabase.SaveAssets();
            Debug.Log("[Build] LevelLayoutConfig reset to defaults. Run Rebuild Level to apply.");
        }

        [MenuItem(Menu + "Apply WebGL Settings", priority = 20)]
        public static void ApplyWebGLSettings() => WebGLBuildTool.ApplySettings();

        [MenuItem(Menu + "Build WebGL", priority = 21)]
        public static void BuildWebGL() => WebGLBuildTool.Build();

        /// <summary>Batch mode: Build All. An exception makes Unity exit with code 1.</summary>
        public static void BatchBuildAll()
        {
            BuildAll();
            EditorApplication.Exit(0);
        }

        /// <summary>Batch mode: Zenject validation of Game.unity (missing bindings fail without entering Play Mode).</summary>
        public static void BatchValidateScene()
        {
            EditorSceneManager.OpenScene(AssetPaths.GameScene, OpenSceneMode.Single);
            // Throws (exit code 1) on any missing binding or resolve error.
            Zenject.Internal.ZenUnityEditorUtil.ValidateCurrentSceneSetup();
            Debug.Log("[Build] Zenject validation: OK");
            EditorApplication.Exit(0);
        }

        /// <summary>Batch mode: WebGL build, exit code 1 on failure.</summary>
        public static void BatchBuildWebGL()
        {
            EditorApplication.Exit(WebGLBuildTool.Build() ? 0 : 1);
        }

        private static LevelBuilder CreateLevelBuilder(ConfigAssets configs) =>
            new LevelBuilder(configs.Layout, new MaterialLibrary(configs.Palette), new MeshLibrary(), configs.Profiles);
    }
}
