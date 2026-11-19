using System.Collections.Generic;
using UnityEngine;

namespace FB.Game.Config
{
    /// <summary>Single source of colors for the whole game (materials, UI, actor badges).</summary>
    [CreateAssetMenu(menuName = "FB/Palette Config", fileName = "PaletteConfig")]
    public sealed class PaletteConfig : ScriptableObject
    {
        [SerializeField] private List<PaletteEntry> _entries = new List<PaletteEntry>();

        public IReadOnlyList<PaletteEntry> Entries => _entries;

        public Color Get(PaletteColor id)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Id == id)
                {
                    return _entries[i].Color;
                }
            }

            return Color.magenta;
        }

#if UNITY_EDITOR
        public void EditorSetEntries(IEnumerable<PaletteEntry> entries)
        {
            _entries = new List<PaletteEntry>(entries);
        }
#endif
    }
}
