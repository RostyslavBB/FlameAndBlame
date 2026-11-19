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
        /// 2x2 flipbook of cartoon flame tongues: a noisy teardrop with a yellow-white core and orange-red rim.
        /// </summary>
        public static Texture2D FlameFlipbook()
        {
            const int tile = 64;
            Texture2D texture = NewTexture("T_FlameFlipbook", tile * 2);
            for (int frame = 0; frame < 4; frame++)
            {
                int offsetX = frame % 2 * tile;
                int offsetY = (1 - frame / 2) * tile;
                for (int y = 0; y < tile; y++)
                {
                    for (int x = 0; x < tile; x++)
                    {
                        texture.SetPixel(offsetX + x, offsetY + y,
                            FlamePixel(x / (tile - 1f), y / (tile - 1f), frame * 3.1f));
                    }
                }
            }

            texture.Apply(false, false);
            return EditorAssetUtility.SaveOrReplace(texture, AssetPaths.Textures + "/T_FlameFlipbook.asset");
        }

        private static Color FlamePixel(float u, float v, float seed)
        {
            float width = 0.42f * Mathf.Pow(1f - v, 0.7f) * Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(v / 0.2f)) + 0.01f;
            float wobble = (Mathf.PerlinNoise(v * 3f + seed, seed * 7f) - 0.5f) * 0.3f * v;
            float inside = 1f - Mathf.Abs(u - 0.5f + wobble) / width;
            float alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(inside / 0.25f));
            float core = Mathf.Clamp01(inside * 1.6f - v * 0.6f);
            Color rim = new Color(1f, 0.35f, 0.08f);
            Color heart = new Color(1f, 0.95f, 0.6f);
            Color color = Color.Lerp(rim, heart, core);
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
