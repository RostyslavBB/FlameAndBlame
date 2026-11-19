using System;
using FB.Game.Config;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>One primitive of a prop, in the prop's local space (y = 0 is the floor under the prop).</summary>
    [Serializable]
    public struct PropPart
    {
        [SerializeField] private PartShape _shape;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _scale;
        [SerializeField] private Vector3 _euler;
        [SerializeField] private PaletteColor _color;
        [SerializeField] private bool _noCollider;

        public PropPart(PartShape shape, Vector3 position, Vector3 scale, PaletteColor color,
            Vector3 euler = default, bool noCollider = false)
        {
            _shape = shape;
            _position = position;
            _scale = scale;
            _euler = euler;
            _color = color;
            _noCollider = noCollider;
        }

        public PartShape Shape => _shape;
        public Vector3 Position => _position;
        public Vector3 Scale => _scale;
        public Vector3 Euler => _euler;
        public PaletteColor Color => _color;
        public bool NoCollider => _noCollider;
    }
}
