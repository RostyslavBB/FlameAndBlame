using System.Collections.Generic;
using FB.Game.Fire;
using FB.Game.Level;
using UnityEngine;
using Zenject;

namespace FB.Game.Surfaces
{
    /// <summary>
    /// Builds <see cref="Surface"/> models from the catalog, maps colliders to surfaces (for water hits)
    /// and caches fire neighbours once at start (OverlapBox NonAlloc).
    /// </summary>
    public sealed class SurfaceRegistry : IInitializable
    {
        private const int OverlapBufferSize = 32;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private readonly LevelCatalog _catalog;
        private readonly FireConfig _config;
        private readonly List<Surface> _surfaces = new List<Surface>();
        private readonly Dictionary<Collider, Surface> _byCollider = new Dictionary<Collider, Surface>();
        private readonly Collider[] _overlaps = new Collider[OverlapBufferSize];

        public SurfaceRegistry(LevelCatalog catalog, FireConfig config)
        {
            _catalog = catalog;
            _config = config;
        }

        public IReadOnlyList<Surface> All => _surfaces;

        public void Initialize()
        {
            SurfaceEntity[] entities = _catalog.Surfaces;
            for (int i = 0; i < entities.Length; i++)
            {
                Surface surface = new Surface(i, entities[i]);
                _surfaces.Add(surface);
                foreach (Collider collider in entities[i].Colliders)
                {
                    _byCollider[collider] = surface;
                }

                foreach (MeshRenderer renderer in entities[i].Renderers)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int m = 0; m < materials.Length; m++)
                    {
                        if (materials[m] != null && materials[m].HasProperty(BaseColorId))
                        {
                            surface.AddTintSlot(new TintSlot(renderer, m, materials[m].GetColor(BaseColorId)));
                        }
                    }
                }
            }

            Physics.SyncTransforms();
            foreach (Surface surface in _surfaces)
            {
                CacheNeighbors(surface);
            }
        }

        public bool TryGet(Collider collider, out Surface surface) => _byCollider.TryGetValue(collider, out surface);

        public Surface FindById(string id)
        {
            foreach (Surface surface in _surfaces)
            {
                if (surface.Root.name == id)
                {
                    return surface;
                }
            }

            return null;
        }

        private void CacheNeighbors(Surface surface)
        {
            Bounds bounds = surface.Bounds;
            float range = _config.SpreadRange;
            int count = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents + Vector3.one * range, _overlaps,
                Quaternion.identity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            HashSet<Surface> added = new HashSet<Surface>();
            for (int i = 0; i < count; i++)
            {
                if (!_byCollider.TryGetValue(_overlaps[i], out Surface other) || other == surface || !added.Add(other))
                {
                    continue;
                }

                float falloff = 1f - Gap(bounds, other.Bounds) / range;
                if (falloff > 0f)
                {
                    surface.AddNeighbor(new SurfaceNeighbor(other, falloff));
                }
            }
        }

        private static float Gap(Bounds a, Bounds b)
        {
            Vector3 delta = a.center - b.center;
            Vector3 extents = a.extents + b.extents;
            Vector3 gap = new Vector3(
                Mathf.Max(0f, Mathf.Abs(delta.x) - extents.x),
                Mathf.Max(0f, Mathf.Abs(delta.y) - extents.y),
                Mathf.Max(0f, Mathf.Abs(delta.z) - extents.z));
            return gap.magnitude;
        }
    }
}
