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

        public static UiSpriteLibrary Ensure()
        {
            EditorAssetUtility.EnsureFolder(AssetPaths.Textures);
            return new UiSpriteLibrary
            {
                Rounded = Save("UI_Rounded", 64, RoundedRect, 20),
                Circle = Save("UI_Circle", 64, Disc, 0),
                Stamp = Save("UI_Stamp", 256, StampRing, 0)
            };
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
