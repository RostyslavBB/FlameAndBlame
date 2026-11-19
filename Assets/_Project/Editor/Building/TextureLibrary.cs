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
    }
}
