using UnityEngine;

namespace FB.Game.Surfaces
{
    /// <summary>
    /// Passive scene marker of a gameplay object: material, price, display name and its renderers/colliders.
    /// Runtime state lives in the plain C# <see cref="Surface"/> model.
    /// </summary>
    public sealed class SurfaceEntity : MonoBehaviour
    {
        [SerializeField] private string _displayName;
        [SerializeField] private MaterialProfileConfig _profile;
        [SerializeField, Min(0)] private int _value;
        [SerializeField] private MeshRenderer[] _renderers;
        [SerializeField] private Collider[] _colliders;

        public string DisplayName => _displayName;
        public MaterialProfileConfig Profile => _profile;

        /// <summary>Price in $; falls back to the material default.</summary>
        public int Value => _value > 0 ? _value : _profile.DefaultValue;

        public MeshRenderer[] Renderers => _renderers;
        public Collider[] Colliders => _colliders;
    }
}
