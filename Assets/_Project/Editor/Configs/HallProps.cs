using System.Collections.Generic;
using FB.Game.Level;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>Entry hall: the spot for the water tap (S2) and the breaker (S4).</summary>
    internal static class HallProps
    {
        public static IEnumerable<PropDefinition> Create()
        {
            yield return Static("EntryDoor", -7.9f, 0f, 1f, 0f,
                Box(0f, 1.05f, 0f, 0.06f, 2.1f, 1f, C.WoodDark),
                Ball(0.06f, 1f, -0.35f, 0.09f, 0.09f, 0.09f, C.Metal, true));

            yield return Static("Doormat", -7.4f, 0f, 1f, 0f,
                Box(0f, 0.01f, 0f, 0.6f, 0.02f, 1f, C.Accent, true));

            yield return Static("ShoeRack", -7.75f, 0f, -2f, 0f,
                Box(0f, 0.25f, 0f, 0.4f, 0.5f, 0.8f, C.Wood));
        }
    }
}
