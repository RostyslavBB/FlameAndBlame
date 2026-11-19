using System;
using FB.Game.Config;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>A floor area. Min/Max are XZ coordinates in meters.</summary>
    [Serializable]
    public struct RoomDefinition
    {
        [SerializeField] private string _name;
        [SerializeField] private Vector2 _min;
        [SerializeField] private Vector2 _max;
        [SerializeField] private PaletteColor _floor;

        public RoomDefinition(string name, Vector2 min, Vector2 max, PaletteColor floor)
        {
            _name = name;
            _min = min;
            _max = max;
            _floor = floor;
        }

        public string Name => _name;
        public Vector2 Min => _min;
        public Vector2 Max => _max;
        public PaletteColor Floor => _floor;
    }
}
