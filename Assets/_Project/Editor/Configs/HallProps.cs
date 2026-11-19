using System.Collections.Generic;
using FB.Game.Level;
using UnityEngine;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>Entry hall: the spot for the water tap (S2) and the breaker (S4).</summary>
    internal static class HallProps
    {
        private const float TapX = -6.6f;
        private const float TapZ = 2.4f;

        /// <summary>Spout of the tap, where the hose starts.</summary>
        public static readonly Vector3 HoseAnchor = new Vector3(TapX, 0.85f, TapZ - 0.2f);

        /// <summary>Prop id of the breaker; its parts 1 (lever) and 2 (lamp) are animated by BreakerView.</summary>
        public const string BreakerId = "Breaker";

        public static IEnumerable<PropDefinition> Create()
        {
            yield return Static(BreakerId, -6.6f, 0f, -2.4f, 180f,
                Box(0f, 1.45f, 0f, 0.36f, 0.46f, 0.06f, C.Metal),
                Box(0f, 1.45f, -0.07f, 0.06f, 0.2f, 0.06f, C.Black),
                Ball(0.12f, 1.63f, -0.04f, 0.06f, 0.06f, 0.06f, C.Bulb, true),
                Box(0f, 1.2f, -0.035f, 0.3f, 0.04f, 0.01f, C.Accent, true));

            yield return Static("Tap", TapX, 0f, TapZ, 0f,
                Box(0f, 0.9f, 0f, 0.28f, 0.34f, 0.04f, C.Metal),
                Cyl(0f, 0.85f, -0.1f, 0.07f, 0.2f, 0.07f, C.Metal, true, new Vector3(90f, 0f, 0f)),
                Cyl(0f, 1.0f, -0.08f, 0.2f, 0.03f, 0.2f, C.Accent, true, new Vector3(90f, 0f, 0f)),
                Cyl(0f, 0.85f, -0.2f, 0.11f, 0.06f, 0.11f, C.Hose, true, new Vector3(90f, 0f, 0f)));

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
