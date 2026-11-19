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
