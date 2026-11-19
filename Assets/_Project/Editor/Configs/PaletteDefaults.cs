using System.Collections.Generic;
using FB.Game.Config;
using UnityEngine;

namespace FB.Editor.Configs
{
    /// <summary>Bright cartoon palette. Missing slots are appended on every Build All, edited colors are kept.</summary>
    internal static class PaletteDefaults
    {
        public static void Merge(PaletteConfig palette)
        {
            List<PaletteEntry> entries = new List<PaletteEntry>(palette.Entries);
            foreach (PaletteEntry entry in Defaults())
            {
                if (!entries.Exists(existing => existing.Id == entry.Id))
                {
                    entries.Add(entry);
                }
            }

            palette.EditorSetEntries(entries);
        }

        private static IEnumerable<PaletteEntry> Defaults()
        {
            yield return Entry(PaletteColor.Wall, "#F4E9D4");
            yield return Entry(PaletteColor.Ceiling, "#FFF9F0");
            yield return Entry(PaletteColor.FloorKitchen, "#8FD3C7");
            yield return Entry(PaletteColor.FloorLiving, "#C98B5A");
            yield return Entry(PaletteColor.FloorHall, "#B9A99A");
            yield return Entry(PaletteColor.Wood, "#E0A867");
            yield return Entry(PaletteColor.WoodDark, "#8A5A3C");
            yield return Entry(PaletteColor.Counter, "#F7F2E8");
            yield return Entry(PaletteColor.Appliance, "#F2F4F7");
            yield return Entry(PaletteColor.Metal, "#9EA7B3");
            yield return Entry(PaletteColor.Black, "#2A2A33");
            yield return Entry(PaletteColor.Sofa, "#E8576F");
            yield return Entry(PaletteColor.Cushion, "#FFC83D");
            yield return Entry(PaletteColor.Curtain, "#5B84F0");
            yield return Entry(PaletteColor.LampShade, "#FFD166");
            yield return Entry(PaletteColor.Plant, "#47B04B");
            yield return Entry(PaletteColor.Pot, "#E2734F");
            yield return Entry(PaletteColor.Rug, "#7A5CB0");
            yield return Entry(PaletteColor.Ceramic, "#FFFFFF");
            yield return Entry(PaletteColor.Accent, "#E63946");
            yield return Entry(PaletteColor.Window, "#BDE6FF", unlit: true);
            yield return Entry(PaletteColor.Bulb, "#FFF3B8", unlit: true);
            yield return Entry(PaletteColor.Player, "#FF8C1A");
            yield return Entry(PaletteColor.Hose, "#D7263D");
            yield return Entry(PaletteColor.Water, "#7FD4FF", unlit: true);
        }

        private static PaletteEntry Entry(PaletteColor id, string hex, bool unlit = false)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return new PaletteEntry(id, color, unlit);
        }
    }
}
