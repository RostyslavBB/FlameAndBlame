using System.Collections.Generic;
using FB.Editor.Configs;
using FB.Game.Puddles;
using FB.Game.Smoke;
using FB.Game.Environment;
using FB.Game.Level;
using FB.Game.Power;
using FB.Game.Surfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FB.Editor.Building
{
    /// <summary>
    /// Idempotent level generation: removes the previous "Level" root and rebuilds it from the layout.
    /// The root gets a LevelCatalog listing every gameplay surface and the breaker.
    /// </summary>
    internal sealed class LevelBuilder
    {
        public const string RootName = "Level";

        private readonly LevelLayoutConfig _layout;
        private readonly ShellBuilder _shell;
        private readonly PropBuilder _props;
        private readonly EnvironmentBuilder _environment;

        public LevelBuilder(LevelLayoutConfig layout, MaterialLibrary materials, MeshLibrary meshes,
            IReadOnlyDictionary<MaterialKind, MaterialProfileConfig> profiles)
        {
            _layout = layout;
            _shell = new ShellBuilder(layout, materials);
            _props = new PropBuilder(materials, meshes, profiles);
            _environment = new EnvironmentBuilder(materials, meshes);
        }

        public LevelCatalog Rebuild(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == RootName)
                {
                    Object.DestroyImmediate(root);
                }
            }

            GameObject level = new GameObject(RootName);
            SceneManager.MoveGameObjectToScene(level, scene);

            Transform shell = new GameObject("Shell").transform;
            shell.SetParent(level.transform, false);
            _shell.Build(shell);

            Transform props = new GameObject("Props").transform;
            props.SetParent(level.transform, false);
            List<SurfaceEntity> surfaces = new List<SurfaceEntity>();
            BreakerView breaker = null;
            foreach (PropDefinition prop in _layout.Props)
            {
                GameObject built = _props.Build(prop, props);
                if (built.TryGetComponent(out SurfaceEntity surface))
                {
                    surfaces.Add(surface);
                }

                if (prop.Id == HallProps.BreakerId)
                {
                    breaker = CreateBreaker(built);
                }
            }

            PuddleSurfaceView puddles = PuddleSurfaceBuilder.Build(_layout, level.transform);
            SmokeView smoke = SmokeBuilder.Build(_layout, level.transform);
            RoomLightsView lights = _environment.BuildLights(level.transform, props);
            ClockView clock = _environment.BuildClock(level.transform);
            _environment.BuildDust(level.transform, _layout.Rooms, ParticleMaterialFactory.Create(
                AssetPaths.Materials + "/M_Dust.mat", TextureLibrary.SoftCircle(), new Color(1f, 0.95f, 0.82f, 0.6f), true));
            LevelCatalog catalog = level.AddComponent<LevelCatalog>();
            EditorAssetUtility.SetArray(catalog, "_surfaces", surfaces.ToArray());
            EditorAssetUtility.SetReference(catalog, "_breaker", breaker);
            EditorAssetUtility.SetReference(catalog, "_puddles", puddles);
            EditorAssetUtility.SetReference(catalog, "_smoke", smoke);
            EditorAssetUtility.SetReference(catalog, "_lights", lights);
            EditorAssetUtility.SetReference(catalog, "_clock", clock);
            return catalog;
        }

        // The lever moves at runtime, so the breaker must not be static-batched.
        private static BreakerView CreateBreaker(GameObject root)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>())
            {
                GameObjectUtility.SetStaticEditorFlags(child.gameObject, 0);
            }

            BreakerView view = root.AddComponent<BreakerView>();
            EditorAssetUtility.SetReference(view, "_lever", root.transform.GetChild(1));
            EditorAssetUtility.SetReference(view, "_lamp", root.transform.GetChild(2).GetComponent<MeshRenderer>());
            return view;
        }
    }
}
