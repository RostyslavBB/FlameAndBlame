using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>Procedural textures saved as assets (no image files in the repo).</summary>
    internal static class TextureLibrary
    {
        private const string DropletPath = AssetPaths.Textures + "/T_Droplet.asset";

        /// <summary>White radial gradient with soft alpha falloff, tinted by the particle color.</summary>
        public static Texture2D Droplet()
        {
            const int size = 32;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "T_Droplet",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                    float alpha = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(distance));
                    float highlight = Mathf.Lerp(1f, 0.75f, distance);
                    texture.SetPixel(x, y, new Color(highlight, highlight, 1f, alpha));
                }
            }

            texture.Apply(false, false);
            return EditorAssetUtility.SaveOrReplace(texture, DropletPath);
        }

        /// <summary>Neutral white soft disc for smoke and steam.</summary>
        public static Texture2D SoftCircle()
        {
            const int size = 32;
            Texture2D texture = NewTexture("T_SoftCircle", size);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(distance))));
                }
            }

            texture.Apply(false, false);
            return EditorAssetUtility.SaveOrReplace(texture, AssetPaths.Textures + "/T_SoftCircle.asset");
        }

        /// <summary>
        /// 4x4 animated flipbook of flame tongues (16 consecutive frames of a flickering, wobbling teardrop) with a
        /// yellow-white core and an orange-red rim.
        /// </summary>
        public static Texture2D FlameFlipbook()
        {
            const int tile = 128;
            Texture2D texture = NewTexture("T_FlameFlipbook", tile * 4);
            for (int frame = 0; frame < 16; frame++)
            {
                int offsetX = frame % 4 * tile;
                int offsetY = (3 - frame / 4) * tile;
                for (int y = 0; y < tile; y++)
                {
                    for (int x = 0; x < tile; x++)
                    {
                        texture.SetPixel(offsetX + x, offsetY + y,
                            FlamePixel(x / (tile - 1f), y / (tile - 1f), frame * 0.45f));
                    }
                }
            }

            texture.Apply(false, false);
            return EditorAssetUtility.SaveOrReplace(texture, AssetPaths.Textures + "/T_FlameFlipbook.asset");
        }

        /// <summary>
        /// Toon flame: a teardrop with wavy licking edges, painted in three hard color bands (red rim, orange,
        /// yellow heart) like a cartoon, instead of a soft gradient.
        /// </summary>
        private static Color FlamePixel(float u, float v, float seed)
        {
            float top = 0.86f + 0.14f * Mathf.Sin(seed * 2.3f);
            v = Mathf.Clamp01(v / top);
            float body = Mathf.Pow(1f - v, 0.85f) * Mathf.Sqrt(Mathf.Clamp01(v / 0.18f));
            float licks = 1f + 0.22f * Mathf.Sin(v * 14f - seed * 3.1f) * v;
            float width = 0.46f * body * licks + 0.004f;
            float sway = (Mathf.PerlinNoise(v * 2.2f + seed, seed * 5f) - 0.5f) * 0.35f * v * v;
            float inside = 1f - Mathf.Abs(u - 0.5f + sway) / width;
            float alpha = Mathf.Clamp01(inside * 40f);

            float heat = inside - v * 0.55f;
            Color color = heat > 0.42f ? new Color(1f, 0.93f, 0.42f)
                : heat > 0.12f ? new Color(1f, 0.6f, 0.1f)
                : new Color(0.95f, 0.28f, 0.06f);
            color.a = alpha;
            return color;
        }

        private static Texture2D NewTexture(string name, int size) =>
            new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = name,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
    }
}
