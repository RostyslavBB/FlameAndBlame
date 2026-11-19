using System.Collections.Generic;
using FB.Game.Config;
using FB.Game.Level;
using UnityEngine;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>
    /// A bookcase packed with books of random height, width and color, some leaning: a lot of cartoon
    /// "lived-in" detail for free. Seeded, so rebuilding gives the same shelf. Front faces local -Z.
    /// </summary>
    internal static class BookshelfParts
    {
        private const float Width = 1.2f;
        private const float Height = 2f;
        private const float Depth = 0.4f;
        private const int Shelves = 5;

        private static readonly C[] BookColors =
        {
            C.BookA, C.BookB, C.BookC, C.BookD, C.Accent, C.Cushion, C.Curtain, C.Ceramic, C.Plant
        };

        public static PropPart[] Create(int seed)
        {
            System.Random random = new System.Random(seed);
            List<PropPart> parts = new List<PropPart>
            {
                Box(-Width * 0.5f + 0.02f, Height * 0.5f, 0f, 0.04f, Height, Depth, C.WoodDark),
                Box(Width * 0.5f - 0.02f, Height * 0.5f, 0f, 0.04f, Height, Depth, C.WoodDark),
                Box(0f, Height * 0.5f, Depth * 0.5f - 0.02f, Width, Height, 0.04f, C.Wood)
            };

            float rowHeight = Height / Shelves;
            for (int row = 0; row <= Shelves; row++)
            {
                float y = Mathf.Min(row * rowHeight + 0.02f, Height - 0.02f);
                parts.Add(Box(0f, y, 0f, Width - 0.04f, 0.04f, Depth, C.WoodDark));
                if (row < Shelves)
                {
                    AddBooks(parts, random, y + 0.02f, rowHeight - 0.08f);
                }
            }

            return parts.ToArray();
        }

        private static void AddBooks(List<PropPart> parts, System.Random random, float floor, float maxHeight)
        {
            float x = -Width * 0.5f + 0.06f;
            float end = Width * 0.5f - 0.06f;
            while (x < end)
            {
                float width = Range(random, 0.035f, 0.075f);
                if (x + width > end)
                {
                    break;
                }

                // Now and then leave a gap, or lean the book on its neighbour.
                if (random.NextDouble() < 0.07)
                {
                    x += Range(random, 0.08f, 0.16f);
                    continue;
                }

                float height = Range(random, maxHeight * 0.6f, maxHeight * 0.95f);
                float depth = Range(random, 0.24f, 0.3f);
                float lean = random.NextDouble() < 0.1 ? Range(random, -14f, 14f) : 0f;
                C color = BookColors[random.Next(BookColors.Length)];
                parts.Add(Box(x + width * 0.5f, floor + height * 0.5f, -0.02f, width, height, depth, color, true,
                    new Vector3(0f, 0f, lean)));
                x += width + (lean != 0f ? 0.03f : 0.004f);
            }
        }

        private static float Range(System.Random random, float min, float max) =>
            min + (float)random.NextDouble() * (max - min);
    }
}
