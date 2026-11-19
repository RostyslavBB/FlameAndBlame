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
            yield return Entry(PaletteColor.Puddle, "#5CB8F2");
            yield return Entry(PaletteColor.Electric, "#9FE8FF", unlit: true);
            yield return Entry(PaletteColor.FloorBath, "#E8F1F5");
            yield return Entry(PaletteColor.FloorBedroom, "#D7A77C");
            yield return Entry(PaletteColor.Trim, "#FAF6EC");
            yield return Entry(PaletteColor.TvScreen, "#5C8DE0", unlit: true);
            yield return Entry(PaletteColor.LampGlow, "#FFF1C9", unlit: true);
            yield return Entry(PaletteColor.Skin, "#F5C6A0");
            yield return Entry(PaletteColor.Hoodie, "#E2403F");
            yield return Entry(PaletteColor.Jeans, "#3E64B5");
            yield return Entry(PaletteColor.Hair, "#4B2E22");
            yield return Entry(PaletteColor.BookA, "#C8473A");
            yield return Entry(PaletteColor.BookB, "#2F7F7A");
            yield return Entry(PaletteColor.BookC, "#E8B54A");
            yield return Entry(PaletteColor.BookD, "#4A5FA8");
        }

        private static PaletteEntry Entry(PaletteColor id, string hex, bool unlit = false)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return new PaletteEntry(id, color, unlit);
        }
    }
}
