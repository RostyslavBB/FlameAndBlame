using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Ui
{
    /// <summary>Procedural UI sprites (PNG + sprite import): rounded panel (9-sliced), circle, rubber stamp.</summary>
    internal sealed class UiSpriteLibrary
    {
        private UiSpriteLibrary()
        {
        }

        public Sprite Rounded { get; private set; }
        public Sprite Circle { get; private set; }
        public Sprite Stamp { get; private set; }

        public Texture2D Lightning { get; private set; }

        public static UiSpriteLibrary Ensure()
        {
            EditorAssetUtility.EnsureFolder(AssetPaths.Textures);
            return new UiSpriteLibrary
            {
                Rounded = Save("UI_Rounded", 64, RoundedRect, 20),
                Circle = Save("UI_Circle", 64, Disc, 0),
                Stamp = Save("UI_Stamp", 256, StampRing, 0),
                Lightning = LightningTexture()
            };
        }

        /// <summary>Jagged branching bolts crawling in from the screen edges: white core, cyan glow, transparent.</summary>
        private static Texture2D LightningTexture()
        {
            const int width = 512;
            const int height = 288;
            float[] core = new float[width * height];
            float[] glow = new float[width * height];
            System.Random random = new System.Random(42);
            for (int bolt = 0; bolt < 7; bolt++)
            {
                Vector2 start = EdgePoint(random, width, height);
                Vector2 target = new Vector2(width * (0.3f + (float)random.NextDouble() * 0.4f),
                    height * (0.3f + (float)random.NextDouble() * 0.4f));
                Bolt(start, target, 26, 3.2f, core, glow, width, height, random, true);
            }

            Color32[] pixels = new Color32[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                float c = Mathf.Clamp01(core[i]);
                float g = Mathf.Clamp01(glow[i]);
                Color color = Color.Lerp(new Color(0.45f, 0.85f, 1f), Color.white, c);
                color.a = Mathf.Clamp01(c + g * 0.7f);
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels32(pixels);
            string path = AssetPaths.Textures + "/UI_Lightning.png";
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static Vector2 EdgePoint(System.Random random, int width, int height)
        {
            float t = (float)random.NextDouble();
            switch (random.Next(4))
            {
                case 0: return new Vector2(t * width, 0f);
                case 1: return new Vector2(t * width, height - 1f);
                case 2: return new Vector2(0f, t * height);
                default: return new Vector2(width - 1f, t * height);
            }
        }

        private static void Bolt(Vector2 from, Vector2 to, int steps, float jitter, float[] core, float[] glow,
            int width, int height, System.Random random, bool branches)
        {
            Vector2 previous = from;
            Vector2 direction = (to - from) / steps;
            Vector2 normal = new Vector2(-direction.y, direction.x).normalized;
            for (int s = 1; s <= steps; s++)
            {
                Vector2 next = from + direction * s + normal * (float)(random.NextDouble() - 0.5) * jitter * 8f;
                for (float d = 0f; d <= 1f; d += 0.5f / Mathf.Max(1f, Vector2.Distance(previous, next)))
                {
                    PaintDot(Vector2.Lerp(previous, next, d), core, glow, width, height);
                }

                if (branches && random.NextDouble() < 0.18)
                {
                    Vector2 branchEnd = next + (direction * 6f + normal * (float)(random.NextDouble() - 0.5) * 120f);
                    Bolt(next, branchEnd, 8, jitter * 0.7f, core, glow, width, height, random, false);
                }

                previous = next;
            }
        }

        private static void PaintDot(Vector2 center, float[] core, float[] glow, int width, int height)
        {
            const int radius = 9;
            int cx = Mathf.RoundToInt(center.x);
            int cy = Mathf.RoundToInt(center.y);
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                for (int x = cx - radius; x <= cx + radius; x++)
                {
                    if (x < 0 || y < 0 || x >= width || y >= height)
                    {
                        continue;
                    }

                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    int index = y * width + x;
                    core[index] = Mathf.Max(core[index], Mathf.Clamp01(1.6f - distance));
                    glow[index] = Mathf.Max(glow[index], Mathf.Exp(-distance / 3.5f));
                }
            }
        }

        private static float RoundedRect(float x, float y, int size)
        {
            const float radius = 16f;
            float dx = Mathf.Max(0f, Mathf.Abs(x - size * 0.5f) - (size * 0.5f - radius));
            float dy = Mathf.Max(0f, Mathf.Abs(y - size * 0.5f) - (size * 0.5f - radius));
            return Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
        }

        private static float Disc(float x, float y, int size)
        {
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(size * 0.5f, size * 0.5f));
            return Mathf.Clamp01(size * 0.5f - 1f - distance + 0.5f);
        }

        private static float StampRing(float x, float y, int size)
        {
            float half = size * 0.5f;
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(half, half));
            float outer = Band(distance, half - 20f, half - 4f);
            float inner = Band(distance, half - 36f, half - 30f);
            float grunge = Mathf.PerlinNoise(x * 0.11f, y * 0.11f) > 0.68f ? 0.25f : 1f;
            return Mathf.Max(outer, inner) * grunge;
        }

        private static float Band(float distance, float from, float to) =>
            Mathf.Clamp01(Mathf.Min(distance - from, to - distance) + 0.5f);

        private static Sprite Save(string name, int size, Func<float, float, int, float> alpha, int border)
        {
            string path = $"{AssetPaths.Textures}/{name}.png";
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha(x + 0.5f, y + 0.5f, size)));
                }
            }

            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spriteBorder = new Vector4(border, border, border, border);
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
