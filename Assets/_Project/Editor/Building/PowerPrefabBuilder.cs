using FB.Game.Config;
using FB.Game.Level;
using FB.Game.Power;
using FB.Game.Puddles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>Pooled prefabs of the electricity module: puddle disc and lightning arc.</summary>
    internal static class PowerPrefabBuilder
    {
        private const int ArcPoints = 10;

        public static void Build(MaterialLibrary materials, MeshLibrary meshes)
        {
            BuildPuddle(materials.Get(PaletteColor.Puddle), meshes.Get(PartShape.Cylinder));
            BuildArc(materials.Get(PaletteColor.Electric));
        }

        private static void BuildPuddle(Material material, Mesh disc)
        {
            GameObject root = new GameObject("Puddle");
            root.AddComponent<MeshFilter>().sharedMesh = disc;
            MeshRenderer renderer = root.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;

            PuddleView view = root.AddComponent<PuddleView>();
            EditorAssetUtility.SetReference(view, "_renderer", renderer);
            Save(root, AssetPaths.PuddlePrefab);
        }

        private static void BuildArc(Material material)
        {
            GameObject root = new GameObject("ElectricArc");
            LineRenderer line = root.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = ArcPoints;
            line.widthMultiplier = 0.045f;
            line.sharedMaterial = material;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.enabled = false;

            ElectricArc arc = root.AddComponent<ElectricArc>();
            EditorAssetUtility.SetReference(arc, "_line", line);
            Save(root, AssetPaths.ArcPrefab);
        }

        private static void Save(GameObject root, string path)
        {
            EditorAssetUtility.EnsureFolder(AssetPaths.Prefabs);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
