using System;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>An opening in whichever wall passes through <see cref="Center"/> (XZ).</summary>
    [Serializable]
    public struct DoorwayDefinition
    {
        [SerializeField] private Vector2 _center;
        [SerializeField, Min(0.1f)] private float _width;
        [SerializeField, Min(0.1f)] private float _height;

        public DoorwayDefinition(Vector2 center, float width, float height)
        {
            _center = center;
            _width = width;
            _height = height;
        }

        public Vector2 Center => _center;
        public float Width => _width;
        public float Height => _height;
    }
}
