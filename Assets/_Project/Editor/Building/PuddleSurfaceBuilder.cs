using FB.Core.Meshes;
using FB.Game.Level;
using FB.Game.Puddles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>
    /// Grid surface over the floor of all rooms, drawn with FB/Puddle; the vertex shader lifts it by the simulated
    /// water depth, so the level visibly rises. Its XZ rectangle defines the water grid.
    /// </summary>
    internal static class PuddleSurfaceBuilder
    {
        private const float HeightAboveFloor = 0.002f;
        private const float VertexSpacing = 0.1f;

        public static PuddleSurfaceView Build(LevelLayoutConfig layout, Transform parent)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            foreach (RoomDefinition room in layout.Rooms)
            {
                min = Vector2.Min(min, room.Min);
                max = Vector2.Max(max, room.Max);
            }

            Vector2 size = max - min;
            Vector2 center = (min + max) * 0.5f;
            GameObject surface = new GameObject("PuddleSurface");
            surface.transform.SetParent(parent, false);
            surface.transform.position = new Vector3(center.x, HeightAboveFloor, center.y);
            Mesh mesh = EditorAssetUtility.SaveOrReplace(GridMeshFactory.Plane("PuddleGrid", size.x, size.y,
                Mathf.CeilToInt(size.x / VertexSpacing), Mathf.CeilToInt(size.y / VertexSpacing)), AssetPaths.PuddleMesh);
            surface.AddComponent<MeshFilter>().sharedMesh = mesh;
            MeshRenderer renderer = surface.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = ShaderMaterial(AssetPaths.PuddleMaterial, "FB/Puddle");
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            PuddleSurfaceView view = surface.AddComponent<PuddleSurfaceView>();
            SerializedObject serialized = new SerializedObject(view);
            serialized.FindProperty("_renderer").objectReferenceValue = renderer;
            serialized.FindProperty("_origin").vector2Value = min;
            serialized.FindProperty("_size").vector2Value = size;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        /// <summary>Material for one of the project's hand-written shaders (created once, tuned in the Inspector).</summary>
        public static Material ShaderMaterial(string path, string shaderName)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find(shaderName));
                AssetDatabase.CreateAsset(material, path);
            }

            return material;
        }
    }
}
