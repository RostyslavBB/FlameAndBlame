using FB.Game.Level;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>Turns a <see cref="PropDefinition"/> into a GameObject hierarchy with colliders and (optionally) a Rigidbody.</summary>
    internal sealed class PropBuilder
    {
        private readonly MaterialLibrary _materials;
        private readonly MeshLibrary _meshes;

        public PropBuilder(MaterialLibrary materials, MeshLibrary meshes)
        {
            _materials = materials;
            _meshes = meshes;
        }

        public GameObject Build(PropDefinition prop, Transform parent)
        {
            GameObject root = new GameObject(prop.Id);
            root.transform.SetParent(parent, false);
            root.transform.SetPositionAndRotation(prop.Position, Quaternion.Euler(0f, prop.Yaw, 0f));

            for (int i = 0; i < prop.Parts.Count; i++)
            {
                BuildPart(prop, prop.Parts[i], i, root.transform);
            }

            if (prop.IsDynamic)
            {
                Rigidbody body = root.AddComponent<Rigidbody>();
                body.mass = prop.Mass;
                body.angularDamping = 0.5f;
            }
            else
            {
                foreach (Transform child in root.GetComponentsInChildren<Transform>())
                {
                    GameObjectUtility.SetStaticEditorFlags(child.gameObject, StaticEditorFlags.BatchingStatic);
                }
            }

            return root;
        }

        private void BuildPart(PropDefinition prop, PropPart part, int index, Transform parent)
        {
            GameObject go = new GameObject($"{prop.Id}_{index}");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = part.Position;
            go.transform.localRotation = Quaternion.Euler(part.Euler);
            go.transform.localScale = part.Scale;

            Mesh mesh = _meshes.Get(part.Shape);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = _materials.Get(part.Color);
            if (_materials.IsUnlit(part.Color))
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }

            if (part.NoCollider)
            {
                return;
            }

            if (part.Shape == PartShape.Box)
            {
                go.AddComponent<BoxCollider>();
                return;
            }

            MeshCollider meshCollider = go.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = prop.IsDynamic;
        }
    }
}
