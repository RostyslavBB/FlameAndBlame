using System.Collections.Generic;
using FB.Game.Level;
using UnityEngine;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>Bathroom x[-6,-3.5] z[-6,-2.5]; models from the Kenney kit are attached in PropModels.</summary>
    internal static class BathroomProps
    {
        public static IEnumerable<PropDefinition> Create()
        {
            yield return Static("Bathtub", -5.05f, 0f, -5.52f, 180f);
            yield return Static("Toilet", -3.92f, 0f, -4.6f, 90f);
            yield return Static("BathSink", -3.8f, 0f, -3.35f, 90f);
            yield return Static("BathMirror", -3.6f, 1.1f, -3.35f, 90f);

            yield return Static("WashingMachine", -5.6f, 0f, -3.45f, -90f,
                Box(0f, 0.42f, 0f, 0.6f, 0.84f, 0.58f, C.Appliance),
                Cyl(0f, 0.45f, -0.3f, 0.36f, 0.03f, 0.36f, C.Metal, true, new Vector3(90f, 0f, 0f)),
                Cyl(0f, 0.45f, -0.31f, 0.28f, 0.02f, 0.28f, C.Window, true, new Vector3(90f, 0f, 0f)),
                Box(0.18f, 0.76f, -0.295f, 0.14f, 0.05f, 0.01f, C.Black, true),
                Ball(-0.2f, 0.76f, -0.295f, 0.05f, 0.05f, 0.02f, C.Accent, true));

            yield return Static("BathMat", -5.0f, 0f, -4.6f, 0f,
                Box(0f, 0.008f, 0f, 1f, 0.016f, 0.55f, C.Curtain, true));
        }
    }
}
