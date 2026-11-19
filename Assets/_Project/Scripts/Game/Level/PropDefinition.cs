using System;
using System.Collections.Generic;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>
    /// A prop made of parts. Convention: local +Z points to the wall behind the prop, the front faces -Z.
    /// </summary>
    [Serializable]
    public sealed class PropDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private Vector3 _position;
        [SerializeField] private float _yaw;
        [SerializeField] private bool _isDynamic;
        [SerializeField, Min(0.01f)] private float _mass = 1f;
        [SerializeField] private List<PropPart> _parts;

        public PropDefinition(string id, Vector3 position, float yaw, bool isDynamic, float mass,
            params PropPart[] parts)
        {
            _id = id;
            _position = position;
            _yaw = yaw;
            _isDynamic = isDynamic;
            _mass = mass;
            _parts = new List<PropPart>(parts);
        }

        /// <summary>Stable name, later used to attach gameplay components (flammable, electric...).</summary>
        public string Id => _id;
        public Vector3 Position => _position;
        public float Yaw => _yaw;
        public bool IsDynamic => _isDynamic;
        public float Mass => _mass;
        public IReadOnlyList<PropPart> Parts => _parts;
    }
}
