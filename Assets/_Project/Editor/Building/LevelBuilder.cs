using System.Collections.Generic;
using FB.Game.Level;
using FB.Game.Surfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FB.Editor.Building
{
    /// <summary>
    /// Idempotent level generation: removes the previous "Level" root and rebuilds it from the layout.
    /// The root gets a SurfaceCatalog listing every gameplay surface.
    /// </summary>
    internal sealed class LevelBuilder
    {
        public const string RootName = "Level";

        private readonly LevelLayoutConfig _layout;
        private readonly ShellBuilder _shell;
        private readonly PropBuilder _props;

        public LevelBuilder(LevelLayoutConfig layout, MaterialLibrary materials, MeshLibrary meshes,
            IReadOnlyDictionary<MaterialKind, MaterialProfileConfig> profiles)
        {
            _layout = layout;
            _shell = new ShellBuilder(layout, materials, meshes);
            _props = new PropBuilder(materials, meshes, profiles);
        }

        public SurfaceCatalog Rebuild(Scene scene)
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
            foreach (PropDefinition prop in _layout.Props)
            {
                if (_props.Build(prop, props).TryGetComponent(out SurfaceEntity surface))
                {
                    surfaces.Add(surface);
                }
            }

            SurfaceCatalog catalog = level.AddComponent<SurfaceCatalog>();
            EditorAssetUtility.SetArray(catalog, "_entities", surfaces.ToArray());
            return catalog;
        }
    }
}
