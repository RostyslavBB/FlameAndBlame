using System;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>A straight wall along its center line, XZ coordinates. Doorways on the line are cut out.</summary>
    [Serializable]
    public struct WallDefinition
    {
        [SerializeField] private Vector2 _from;
        [SerializeField] private Vector2 _to;

        public WallDefinition(Vector2 from, Vector2 to)
        {
            _from = from;
            _to = to;
        }

        public Vector2 From => _from;
        public Vector2 To => _to;
    }
}
