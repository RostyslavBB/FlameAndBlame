using UnityEngine;

namespace FB.Game.Surfaces
{
    /// <summary>List of all surfaces in the level, filled by the level builder (no runtime scene searches).</summary>
    public sealed class SurfaceCatalog : MonoBehaviour
    {
        [SerializeField] private SurfaceEntity[] _entities;

        public SurfaceEntity[] Entities => _entities;
    }
}
