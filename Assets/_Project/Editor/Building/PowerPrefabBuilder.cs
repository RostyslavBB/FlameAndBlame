using FB.Game.Config;
using FB.Game.Power;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>Pooled prefab of the electricity module: the lightning arc.</summary>
    internal static class PowerPrefabBuilder
    {
        private const int ArcPoints = 10;

        public static void Build(MaterialLibrary materials)
        {
            GameObject root = new GameObject("ElectricArc");
            LineRenderer line = root.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = ArcPoints;
            line.widthMultiplier = 0.045f;
            line.sharedMaterial = materials.Get(PaletteColor.Electric);
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.enabled = false;

            ElectricArc arc = root.AddComponent<ElectricArc>();
            EditorAssetUtility.SetReference(arc, "_line", line);

            EditorAssetUtility.EnsureFolder(AssetPaths.Prefabs);
            PrefabUtility.SaveAsPrefabAsset(root, AssetPaths.ArcPrefab);
            Object.DestroyImmediate(root);
        }
    }
}
