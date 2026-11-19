using System.Collections.Generic;
using FB.Game.Level;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>
    /// Places an imported model inside a prop root (which must be at the origin, unrotated, while building):
    /// fits it uniformly into the prop's box, moves its bottom-center to the origin, recolors its materials
    /// into the palette and adds one box collider.
    /// </summary>
    internal sealed class ModelPropBuilder
    {
        private readonly MaterialLibrary _materials;

        public ModelPropBuilder(MaterialLibrary materials)
        {
            _materials = materials;
        }

        public void Build(PropDefinition prop, Transform root)
        {
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(prop.Model);
            if (source == null)
            {
                Debug.LogError($"[Build] Model '{prop.Model}' for prop '{prop.Id}' not found.");
                return;
            }

            GameObject model = Object.Instantiate(source, root, false);
            model.name = "Model";
            Bounds bounds = LocalBounds(model);
            Vector3 fit = prop.ModelFit;
            float scale = Mathf.Min(fit.x / bounds.size.x, fit.y / bounds.size.y, fit.z / bounds.size.z);
            model.transform.localScale = Vector3.one * scale;
            model.transform.localPosition = -new Vector3(bounds.center.x, bounds.min.y, bounds.center.z) * scale;

            AddCollider(model);
            Recolor(prop, model);
        }

        /// <summary>
        /// The box must be expressed in the model's own frame: many FBX roots carry a rotation (and a tiny scale),
        /// so root-space bounds used directly would put the collider beside the mesh.
        /// </summary>
        private static void AddCollider(GameObject model)
        {
            Bounds placed = LocalBounds(model);
            Transform frame = model.transform;
            Vector3 size = frame.InverseTransformVector(placed.size);
            BoxCollider collider = model.AddComponent<BoxCollider>();
            collider.center = frame.InverseTransformPoint(placed.center);
            collider.size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
        }

        private static Bounds LocalBounds(GameObject model)
        {
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            // The root is at the origin and unrotated while building, so world bounds are model-local bounds.
            return bounds;
        }

        private void Recolor(PropDefinition prop, GameObject model)
        {
            Dictionary<string, Material> map = new Dictionary<string, Material>();
            foreach (ModelMaterial entry in prop.ModelMaterials)
            {
                map[entry.Name] = _materials.Get(entry.Color);
            }

            foreach (MeshRenderer renderer in model.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && map.TryGetValue(materials[i].name, out Material palette))
                    {
                        materials[i] = palette;
                    }
                }

                renderer.sharedMaterials = materials;
            }
        }
    }
}
