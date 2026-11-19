using System.Collections.Generic;
using FB.Game.Level;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>Bedroom x[1.5,7] z[-7,-2.5]: bed against the south wall, wardrobe west, desk with laptop east.</summary>
    internal static class BedroomProps
    {
        public static IEnumerable<PropDefinition> Create()
        {
            yield return Static("Bed", 4f, 0f, -5.85f, 180f);
            yield return Static("NightstandLeft", 2.85f, 0f, -6.65f, 180f);
            yield return Static("NightstandRight", 5.15f, 0f, -6.65f, 180f);
            yield return Static("NightLamp", 2.85f, 0.5f, -6.65f, 180f);
            yield return Static("Wardrobe", 1.88f, 0f, -4.1f, -90f);
            yield return Static("Desk", 6.62f, 0f, -4.7f, 90f);
            yield return Static("Laptop", 6.68f, 0.76f, -4.7f, 90f);
            yield return Dynamic("DeskChair", 6.0f, 0f, -4.7f, -90f, 5f);

            yield return Static("BedroomRug", 4f, 0f, -4.3f, 0f,
                Box(0f, 0.01f, 0f, 2.4f, 0.02f, 1.5f, C.Cushion, true),
                Box(0f, 0.012f, 0f, 2.1f, 0.02f, 1.2f, C.Sofa, true));
        }
    }
}
