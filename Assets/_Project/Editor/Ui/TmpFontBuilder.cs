using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace FB.Editor.Ui
{
    /// <summary>
    /// Imports TMP Essential Resources if needed and creates a static SDF font asset with Latin + Ukrainian
    /// characters from LiberationSans (shipped with TMP), so WebGL never needs runtime glyph generation.
    /// </summary>
    internal static class TmpFontBuilder
    {
        private const string EssentialsFont = "Assets/TextMesh Pro/Fonts/LiberationSans.ttf";
        private const string FontPath = AssetPaths.Fonts + "/FB_Font SDF.asset";

        private const string Characters =
            " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
            "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯабвгґдеєжзиіїйклмнопрстуфхцчшщьюя" +
            "ЁЪЫЭёъыэ«»—–…№’ʼ•×→←↑↓·°";

        public static TMP_FontAsset Ensure()
        {
            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
            if (existing != null)
            {
                return existing;
            }

            ImportEssentialsIfMissing();
            Font source = AssetDatabase.LoadAssetAtPath<Font>(EssentialsFont);
            if (source == null)
            {
                throw new FileNotFoundException("[Build] TMP Essentials were just imported; run Build All once more.",
                    EssentialsFont);
            }

            TMP_FontAsset font = TMP_FontAsset.CreateFontAsset(source, 48, 5, GlyphRenderMode.SDFAA, 1024, 1024,
                AtlasPopulationMode.Dynamic, false);
            font.name = "FB_Font SDF";
            if (!font.TryAddCharacters(Characters, out string missing))
            {
                Debug.LogWarning($"[Build] Font is missing glyphs: {missing}");
            }

            font.atlasPopulationMode = AtlasPopulationMode.Static;
            EditorAssetUtility.EnsureFolder(AssetPaths.Fonts);
            AssetDatabase.CreateAsset(font, FontPath);
            font.atlasTexture.name = "FB_Font Atlas";
            AssetDatabase.AddObjectToAsset(font.atlasTexture, font);
            font.material.name = "FB_Font Material";
            AssetDatabase.AddObjectToAsset(font.material, font);
            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
            return font;
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
