using FB.Game.Level;
using FB.Game.Smoke;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>Ceiling smoke: thin stacked planes over the rooms (FB/SmokeLayer), soft drifting puffs under the ceiling
    /// (they make it read as a volume from any angle) and the smoke post-processing volume.</summary>
    internal static class SmokeBuilder
    {
        private const int Layers = 6;
        private const float LayerSpacing = 0.1f;
        // Six thin slices, each fainter than the old three: same total density, smoother look.
        private const float SliceOpacity = 0.55f;
        private const string PuffMaterial = AssetPaths.Materials + "/M_Smoke.mat";

        public static SmokeView Build(LevelLayoutConfig layout, Transform parent)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            foreach (RoomDefinition room in layout.Rooms)
            {
                min = Vector2.Min(min, room.Min);
                max = Vector2.Max(max, room.Max);
            }

            GameObject root = new GameObject("Smoke");
            root.transform.SetParent(parent, false);

            GameObject layer = new GameObject("SmokeLayer");
            layer.transform.SetParent(root.transform, false);
            Vector2 center = (min + max) * 0.5f;
            layer.transform.position = new Vector3(center.x, 2.5f, center.y);
            layer.AddComponent<MeshFilter>().sharedMesh = EditorAssetUtility.SaveOrReplace(
                StackedPlanes(max - min), AssetPaths.SmokeMesh);
            MeshRenderer renderer = layer.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = PuddleSurfaceBuilder.ShaderMaterial(AssetPaths.SmokeMaterial, "FB/SmokeLayer");
            renderer.sharedMaterial.SetFloat("_MaxAlpha", SliceOpacity);
            EditorUtility.SetDirty(renderer.sharedMaterial);
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.enabled = false;

            Volume volume = PostProcessBuilder.CreateVolume("SmokeVolume", PostProcessBuilder.SmokeProfile(), 10f, 0f);
            volume.transform.SetParent(root.transform, false);

            SmokeView view = root.AddComponent<SmokeView>();
            EditorAssetUtility.SetReference(view, "_layer", renderer);
            EditorAssetUtility.SetReference(view, "_volume", volume);
            EditorAssetUtility.SetReference(view, "_puffs", CeilingPuffs.Create(layer.transform, max - min,
                AssetDatabase.LoadAssetAtPath<Material>(PuffMaterial)));
            return view;
        }

        private static Mesh StackedPlanes(Vector2 size)
        {
            Vector3[] vertices = new Vector3[Layers * 4];
            int[] triangles = new int[Layers * 6];
            float hx = size.x * 0.5f;
            float hz = size.y * 0.5f;
            for (int l = 0; l < Layers; l++)
            {
                float y = l * LayerSpacing;
                int v = l * 4;
                vertices[v] = new Vector3(-hx, y, -hz);
                vertices[v + 1] = new Vector3(-hx, y, hz);
                vertices[v + 2] = new Vector3(hx, y, hz);
                vertices[v + 3] = new Vector3(hx, y, -hz);
                int t = l * 6;
                triangles[t] = v;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + 2;
                triangles[t + 3] = v;
                triangles[t + 4] = v + 2;
                triangles[t + 5] = v + 3;
            }

            Mesh mesh = new Mesh { name = "SmokeLayers", vertices = vertices, triangles = triangles };
            mesh.RecalculateNormals();
            mesh.bounds = new Bounds(new Vector3(0f, LayerSpacing, 0f), new Vector3(size.x, 2f, size.y));
            return mesh;
        }
    }
}
