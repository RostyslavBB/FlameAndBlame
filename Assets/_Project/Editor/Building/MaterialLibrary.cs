using System.Collections.Generic;
using FB.Game.Config;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>One shared, GPU-instanced material per palette slot (M_{Slot}.mat), updated in place.</summary>
    internal sealed class MaterialLibrary
    {
        private const string LitShader = "Universal Render Pipeline/Lit";
        private const string UnlitShader = "Universal Render Pipeline/Unlit";
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        private readonly Dictionary<PaletteColor, Material> _materials = new Dictionary<PaletteColor, Material>();
        private readonly HashSet<PaletteColor> _unlit = new HashSet<PaletteColor>();

        public MaterialLibrary(PaletteConfig palette)
        {
            EditorAssetUtility.EnsureFolder(AssetPaths.Materials);
            foreach (PaletteEntry entry in palette.Entries)
            {
                _materials[entry.Id] = CreateOrUpdate(entry);
                if (entry.Unlit)
                {
                    _unlit.Add(entry.Id);
                }
            }

            AssetDatabase.SaveAssets();
        }

        public Material Get(PaletteColor color) => _materials[color];

        public bool IsUnlit(PaletteColor color) => _unlit.Contains(color);

        /// <summary>Gives a palette slot a tiled texture (UVs in meters) instead of a flat color.</summary>
        public void SetTexture(PaletteColor color, Texture2D texture, Color tint, float smoothness)
        {
            Material material = _materials[color];
            material.SetTexture(BaseMap, texture);
            material.SetColor(BaseColor, tint);
            material.SetFloat(Smoothness, smoothness);
            EditorUtility.SetDirty(material);
        }

        /// <summary>Floors and walls of the apartment get procedural parquet, tiles and wallpaper.</summary>
        public void ApplyShellTextures()
        {
            Texture2D parquet = SurfaceTextures.Parquet();
            SetTexture(PaletteColor.FloorLiving, parquet, Color.white, 0.38f);
            SetTexture(PaletteColor.FloorBedroom, parquet, new Color(1f, 0.86f, 0.78f), 0.32f);
            SetTexture(PaletteColor.FloorKitchen, SurfaceTextures.Tiles("T_TilesKitchen", 4, new Color(0.96f, 0.96f, 0.93f),
                new Color(0.55f, 0.82f, 0.76f), new Color(0.62f, 0.64f, 0.62f), 0.06f), Color.white, 0.55f);
            SetTexture(PaletteColor.FloorHall, SurfaceTextures.Tiles("T_TilesHall", 2, new Color(0.8f, 0.73f, 0.64f),
                new Color(0.74f, 0.67f, 0.58f), new Color(0.45f, 0.4f, 0.36f), 0.12f), Color.white, 0.3f);
            SetTexture(PaletteColor.FloorBath, SurfaceTextures.Tiles("T_TilesBath", 4, new Color(0.94f, 0.96f, 0.98f),
                new Color(0.86f, 0.92f, 0.97f), new Color(0.55f, 0.66f, 0.74f), 0.04f), Color.white, 0.6f);
            SetTexture(PaletteColor.Wall, SurfaceTextures.Wallpaper(), Color.white, 0.05f);
            AssetDatabase.SaveAssets();
        }

        private static Material CreateOrUpdate(PaletteEntry entry)
        {
            string path = $"{AssetPaths.Materials}/M_{entry.Id}.mat";
            Shader shader = Shader.Find(entry.Unlit ? UnlitShader : LitShader);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            material.shader = shader;
            material.SetColor(BaseColor, entry.Color);
            if (!entry.Unlit)
            {
                material.SetFloat(Smoothness, 0.12f);
            }

            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
