using TMPro;
using UnityEngine;

namespace FB.Editor.Ui
{
    /// <summary>The two UI typefaces and their "readable over the game" material presets.</summary>
    internal sealed class UiFonts
    {
        public UiFonts(TMP_FontAsset display, TMP_FontAsset body, Material displayOutlined, Material bodyShadow)
        {
            Display = display;
            Body = body;
            DisplayOutlined = displayOutlined;
            BodyShadow = bodyShadow;
        }

        /// <summary>Russo One: titles, numbers, buttons.</summary>
        public TMP_FontAsset Display { get; }

        /// <summary>Rubik: body copy, feed lines.</summary>
        public TMP_FontAsset Body { get; }

        public Material DisplayOutlined { get; }
        public Material BodyShadow { get; }
    }
}
