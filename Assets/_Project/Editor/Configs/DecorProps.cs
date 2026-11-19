using System.Collections.Generic;
using FB.Game.Level;
using UnityEngine;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>Small things that make the apartment feel lived in: lamps, pictures, coat rack, radio, plant.</summary>
    internal static class DecorProps
    {
        private const float LampY = 2.4f;

        public static IEnumerable<PropDefinition> Create()
        {
            yield return Static("CeilingLampHall", -7f, LampY, 0f, 0f);
            yield return Static("CeilingLampKitchen", -3f, LampY, 0.2f, 0f);
            yield return Static("CeilingLampLiving", 3.5f, LampY, 0f, 0f);
            yield return Static("CeilingLampBath", -4.75f, LampY, -4.25f, 0f);
            yield return Static("CeilingLampBedroom", 4.25f, LampY, -4.6f, 0f);

            yield return Dynamic("CoatRack", -7.55f, 0f, 1.95f, 0f, 6f);
            yield return Static("Radio", -4.6f, 0.91f, 2.22f, 0f);
            yield return Dynamic("PlantPot", -3.35f, 0.78f, -0.45f, 0f, 0.6f);

            yield return Picture("PictureLiving", 0.09f, 1.6f, -1.2f, -90f, C.Curtain, C.Cushion, C.Accent);
            yield return Picture("PictureBedroom", 4f, 1.75f, -6.91f, 180f, C.Plant, C.LampShade, C.Sofa);
            yield return Picture("PictureHall", -7.91f, 1.6f, -1.6f, -90f, C.Rug, C.Window, C.Cushion);
        }

        private static PropDefinition Picture(string id, float x, float y, float z, float yaw, C canvas, C a, C b) =>
            Static(id, x, y, z, yaw,
                Box(0f, 0f, 0f, 0.9f, 0.65f, 0.04f, C.WoodDark, true),
                Box(0f, 0f, -0.022f, 0.8f, 0.55f, 0.01f, canvas, true),
                Ball(-0.18f, 0.08f, -0.03f, 0.22f, 0.22f, 0.01f, a, true),
                Box(0.16f, -0.08f, -0.03f, 0.3f, 0.18f, 0.008f, b, true, new Vector3(0f, 0f, 12f)));
    }
}
