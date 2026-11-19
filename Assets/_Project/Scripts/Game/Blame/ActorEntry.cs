using System;
using UnityEngine;

namespace FB.Game.Blame
{
    [Serializable]
    public struct ActorEntry
    {
        [SerializeField] private ActorId _id;
        [SerializeField] private string _name;
        [SerializeField] private Color _color;

        public ActorEntry(ActorId id, string name, Color color)
        {
            _id = id;
            _name = name;
            _color = color;
        }

        public ActorId Id => _id;
        public string Name => _name;
        public Color Color => _color;
    }
}
