using System;
using UnityEngine;

namespace FB.Game.Config
{
    [Serializable]
    public struct PaletteEntry
    {
        [SerializeField] private PaletteColor _id;
        [SerializeField] private Color _color;
        [SerializeField] private bool _unlit;

        public PaletteEntry(PaletteColor id, Color color, bool unlit = false)
        {
            _id = id;
            _color = color;
            _unlit = unlit;
        }

        public PaletteColor Id => _id;
        public Color Color => _color;

        /// <summary>Unlit slots glow on their own (windows, bulbs).</summary>
        public bool Unlit => _unlit;
    }
}
