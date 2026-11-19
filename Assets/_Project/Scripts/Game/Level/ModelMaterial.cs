using System;
using FB.Game.Config;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>Replaces a material of an imported model (by name) with a palette color.</summary>
    [Serializable]
    public struct ModelMaterial
    {
        [SerializeField] private string _name;
        [SerializeField] private PaletteColor _color;

        public ModelMaterial(string name, PaletteColor color)
        {
            _name = name;
            _color = color;
        }

        public string Name => _name;
        public PaletteColor Color => _color;
    }
}
