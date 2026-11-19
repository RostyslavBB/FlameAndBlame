using UnityEngine;

namespace FB.Editor.Ui
{
    /// <summary>Alpha functions (x, y, size) -> 0..1 for the procedural UI sprites.</summary>
    internal static class UiSpriteShapes
    {
        public static float RoundedRect(float x, float y, int size) => RoundedBox(x, y, size, 16f, 0f);

        /// <summary>Fully rounded ends; sliced with a border of half the size it stretches into a pill.</summary>
        public static float Pill(float x, float y, int size) => RoundedBox(x, y, size, size * 0.5f - 1f, 0f);

        /// <summary>Rounded box with a wide soft falloff: a blurred drop shadow, sliced under cards.</summary>
        public static float SoftShadow(float x, float y, int size)
        {
            const float radius = 16f;
            const float blur = 22f;
            float half = size * 0.5f;
            float dx = Mathf.Max(0f, Mathf.Abs(x - half) - (half - radius - blur));
            float dy = Mathf.Max(0f, Mathf.Abs(y - half) - (half - radius - blur));
            float distance = Mathf.Sqrt(dx * dx + dy * dy) - radius;
            float t = Mathf.Clamp01(1f - distance / blur);
            return t * t * (3f - 2f * t);
        }

        public static float Disc(float x, float y, int size)
        {
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(size * 0.5f, size * 0.5f));
            return Mathf.Clamp01(size * 0.5f - 1f - distance + 0.5f);
        }

        /// <summary>Thin circle outline (crosshair).</summary>
        public static float Ring(float x, float y, int size)
        {
            float half = size * 0.5f;
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(half, half));
            return Band(distance, half - 7f, half - 1.5f);
        }

        /// <summary>Radial soft glow, brightest in the middle.</summary>
        public static float Glow(float x, float y, int size)
        {
            float half = size * 0.5f;
            float t = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), new Vector2(half, half)) / half);
            return t * t;
        }

        public static float StampRing(float x, float y, int size)
        {
            float half = size * 0.5f;
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(half, half));
            float outer = Band(distance, half - 20f, half - 4f);
            float inner = Band(distance, half - 36f, half - 30f);
            float grunge = Mathf.PerlinNoise(x * 0.11f, y * 0.11f) > 0.68f ? 0.25f : 1f;
            return Mathf.Max(outer, inner) * grunge;
        }

        private static float RoundedBox(float x, float y, int size, float radius, float inset)
        {
            float half = size * 0.5f - inset;
            float dx = Mathf.Max(0f, Mathf.Abs(x - size * 0.5f) - (half - radius));
            float dy = Mathf.Max(0f, Mathf.Abs(y - size * 0.5f) - (half - radius));
            return Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
        }

        private static float Band(float distance, float from, float to) =>
            Mathf.Clamp01(Mathf.Min(distance - from, to - distance) + 0.5f);
    }
}
