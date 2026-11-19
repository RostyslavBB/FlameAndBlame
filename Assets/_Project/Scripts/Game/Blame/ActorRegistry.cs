using System;
using UnityEngine;

namespace FB.Game.Blame
{
    public sealed class ActorRegistry : IActorRegistry
    {
        private readonly string[] _names;
        private readonly Color[] _colors;

        public ActorRegistry(ActorRegistryConfig config)
        {
            Count = Enum.GetValues(typeof(ActorId)).Length;
            _names = new string[Count];
            _colors = new Color[Count];
            for (int i = 0; i < Count; i++)
            {
                _names[i] = ((ActorId)i).ToString();
                _colors[i] = Color.gray;
            }

            foreach (ActorEntry entry in config.Actors)
            {
                _names[(int)entry.Id] = entry.Name;
                _colors[(int)entry.Id] = entry.Color;
            }
        }

        public int Count { get; }
        public string NameOf(ActorId actor) => _names[(int)actor];
        public Color ColorOf(ActorId actor) => _colors[(int)actor];
    }
}
