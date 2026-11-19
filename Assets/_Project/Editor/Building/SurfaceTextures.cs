using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>
    /// Procedural, seamless 512 px = 1 m textures for the apartment shell: parquet, ceramic tiles, stone tiles and
    /// striped wallpaper. Saved as PNG and imported with mipmaps and repeat wrapping.
    /// </summary>
    internal static class SurfaceTextures
    {
        private const int Size = 512;

        public static Texture2D Parquet() => Save("T_Parquet", (x, y) =>
        {
            const int plankWidth = 64;
            const int plankLength = 256;
            int column = x / plankWidth;
            int shifted = (y + Hash(column, 7) % plankLength + Size) % Size;
            int row = shifted / plankLength;
            float tone = 0.88f + Rand(column, row) * 0.24f;
            float grain = Mathf.PerlinNoise(x * 0.35f + column * 13f, shifted * 0.02f) * 0.18f +
                          Mathf.Sin((x % plankWidth) * 0.45f + Mathf.PerlinNoise(column, shifted * 0.01f) * 6f) * 0.04f;
            Color wood = new Color(0.72f, 0.47f, 0.28f) * (tone - grain);
            bool gap = x % plankWidth < 2 || shifted % plankLength < 2;
            return gap ? wood * 0.55f : wood;
        });

        public static Texture2D Tiles(string name, int tilesPerMeter, Color a, Color b, Color grout, float variation) =>
            Save(name, (x, y) =>
            {
                int tile = Size / tilesPerMeter;
                int tx = x / tile;
                int ty = y / tile;
                bool line = x % tile < 3 || y % tile < 3;
                if (line)
                {
                    return grout;
                }

                Color baseColor = (tx + ty) % 2 == 0 ? a : b;
                float speckle = Mathf.PerlinNoise(x * 0.08f + tx * 31f, y * 0.08f + ty * 17f) - 0.5f;
                float edge = Mathf.Min(Mathf.Min(x % tile, tile - x % tile), Mathf.Min(y % tile, tile - y % tile));
                float bevel = Mathf.Clamp01(edge / 6f) * 0.08f + 0.92f;
                return baseColor * ((1f + (Rand(tx, ty) - 0.5f) * variation + speckle * variation) * bevel);
            });

        public static Texture2D Wallpaper() => Save("T_Wallpaper", (x, y) =>
        {
            Color baseColor = new Color(0.96f, 0.91f, 0.81f);
            Color stripe = new Color(0.92f, 0.85f, 0.73f);
            Color line = new Color(0.82f, 0.68f, 0.45f);
            int band = x % 64;
            Color color = band < 32 ? baseColor : stripe;
            if (band == 31 || band == 32)
            {
                color = line;
            }

            int cx = x % 32 - 16;
            int cy = y % 32 - 16;
            if (band < 32 && cx * cx + cy * cy < 5)
            {
                color = Color.Lerp(color, line, 0.6f);
            }

            float paper = Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * 0.04f;
            return color * (0.98f + paper);
        });

        private static Texture2D Save(string name, Func<int, int, Color> pixel)
        {
            string path = $"{AssetPaths.Textures}/{name}.png";
            Texture2D texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[Size * Size];
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    Color c = pixel(x, y);
                    c.a = 1f;
                    pixels[y * Size + x] = c;
                }
            }

            texture.SetPixels(pixels);
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Default;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.filterMode = FilterMode.Trilinear;
            importer.anisoLevel = 4;
            importer.maxTextureSize = Size;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static int Hash(int a, int b) => Mathf.Abs((a * 73856093) ^ (b * 19349663));

        private static float Rand(int a, int b) => Hash(a, b) % 1000 / 1000f;
    }
}
