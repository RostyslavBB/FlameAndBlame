using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace FB.Editor.Ui
{
    /// <summary>
    /// Creates static SDF font assets with Latin + Ukrainian glyphs (so WebGL never generates glyphs at runtime):
    /// Russo One for headings and numbers, Rubik for body text (both SIL OFL, licenses next to the TTFs), plus
    /// material presets with an outline and a drop shadow for text drawn over the 3D view.
    /// </summary>
    internal static class TmpFontBuilder
    {
        private const string DisplaySource = AssetPaths.Fonts + "/RussoOne-Regular.ttf";
        private const string BodySource = AssetPaths.Fonts + "/Rubik-Variable.ttf";
        private const string LegacyFont = AssetPaths.Fonts + "/FB_Font SDF.asset";
        private const int Padding = 9;

        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int UnderlayColor = Shader.PropertyToID("_UnderlayColor");
        private static readonly int UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
        private static readonly int UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
        private static readonly int UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
        private static readonly int FaceDilate = Shader.PropertyToID("_FaceDilate");

        private const string Characters =
            " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
            "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯабвгґдеєжзиіїйклмнопрстуфхцчшщьюя" +
            "ЁЪЫЭёъыэ«»—–…№’ʼ•×→←↑↓·°";

        public static UiFonts Ensure()
        {
            ImportEssentialsIfMissing();
            TMP_FontAsset display = EnsureFont(DisplaySource, "FB_Display SDF");
            TMP_FontAsset body = EnsureFont(BodySource, "FB_Body SDF");
            AssetDatabase.DeleteAsset(LegacyFont);
            // Russo One lacks a few symbols (arrows, rare punctuation): Rubik fills them in.
            display.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset> { body };
            EditorUtility.SetDirty(display);

            Material displayOutlined = Preset(display, "FB_Display Outlined", 0.22f, 0.9f);
            Material bodyShadow = Preset(body, "FB_Body Shadow", 0f, 0.6f);
            AssetDatabase.SaveAssets();
            return new UiFonts(display, body, displayOutlined, bodyShadow);
        }

        private static TMP_FontAsset EnsureFont(string sourcePath, string name)
        {
            string path = $"{AssetPaths.Fonts}/{name}.asset";
            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (existing != null)
            {
                return existing;
            }

            Font source = AssetDatabase.LoadAssetAtPath<Font>(sourcePath);
            if (source == null)
            {
                throw new FileNotFoundException("[Build] UI font is missing.", sourcePath);
            }

            TMP_FontAsset font = TMP_FontAsset.CreateFontAsset(source, 64, Padding, GlyphRenderMode.SDFAA, 1024, 2048,
                AtlasPopulationMode.Dynamic, false);
            font.name = name;
            if (!font.TryAddCharacters(Characters, out string missing))
            {
                Debug.LogWarning($"[Build] {name} is missing glyphs: {missing}");
            }

            font.atlasPopulationMode = AtlasPopulationMode.Static;
            EditorAssetUtility.EnsureFolder(AssetPaths.Fonts);
            AssetDatabase.CreateAsset(font, path);
            font.atlasTexture.name = name + " Atlas";
            AssetDatabase.AddObjectToAsset(font.atlasTexture, font);
            font.material.name = name + " Material";
            AssetDatabase.AddObjectToAsset(font.material, font);
            EditorUtility.SetDirty(font);
            return font;
        }

        /// <summary>A material preset (same atlas) with a dark outline and/or a soft drop shadow.</summary>
        private static Material Preset(TMP_FontAsset font, string name, float outline, float shadow)
        {
            string path = $"{AssetPaths.Fonts}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(font.material);
                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.CopyPropertiesFromMaterial(font.material);
            }

            material.shader = font.material.shader;
            material.mainTexture = font.atlasTexture;
            Color ink = new Color(0.1f, 0.06f, 0.06f, 1f);
            if (outline > 0f)
            {
                material.EnableKeyword("OUTLINE_ON");
                material.SetColor(OutlineColor, ink);
                material.SetFloat(OutlineWidth, outline);
                material.SetFloat(FaceDilate, outline * 0.6f);
            }

            material.EnableKeyword("UNDERLAY_ON");
            material.SetColor(UnderlayColor, new Color(0f, 0f, 0f, shadow));
            material.SetFloat(UnderlayOffsetX, 0.6f);
            material.SetFloat(UnderlayOffsetY, -0.8f);
            material.SetFloat(UnderlaySoftness, 0.35f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ImportEssentialsIfMissing()
        {
            if (AssetDatabase.IsValidFolder("Assets/TextMesh Pro"))
            {
                return;
            }

            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMP_Text).Assembly);
            string unityPackage = Path.Combine(package.resolvedPath, "Package Resources",
                "TMP Essential Resources.unitypackage");
            AssetDatabase.ImportPackage(unityPackage, false);
            AssetDatabase.Refresh();
        }
    }
}
