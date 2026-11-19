using System.Collections.Generic;
using FB.Game.Level;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>Kitchen x[-6,0]. Counter line along the north wall, stove close to the living-room doorway.</summary>
    internal static class KitchenProps
    {
        public static IEnumerable<PropDefinition> Create()
        {
            yield return Static("Fridge", -5.5f, 0f, 2.05f, 0f,
                Box(0f, 0.95f, 0f, 0.8f, 1.9f, 0.75f, C.Appliance),
                Box(0f, 1.25f, -0.38f, 0.78f, 0.02f, 0.01f, C.Metal, true),
                Box(0.3f, 0.85f, -0.4f, 0.04f, 0.45f, 0.04f, C.Metal, true),
                Box(0.3f, 1.6f, -0.4f, 0.04f, 0.4f, 0.04f, C.Metal, true));

            yield return Static("CabinetLeft", -3.75f, 0f, 2.125f, 0f,
                Box(0f, 0.43f, 0f, 2.7f, 0.86f, 0.6f, C.Wood),
                Box(0f, 0.885f, -0.02f, 2.74f, 0.05f, 0.64f, C.Counter),
                Box(-0.9f, 0.7f, -0.31f, 0.22f, 0.03f, 0.03f, C.Metal, true),
                Box(0f, 0.7f, -0.31f, 0.22f, 0.03f, 0.03f, C.Metal, true),
                Box(0.9f, 0.7f, -0.31f, 0.22f, 0.03f, 0.03f, C.Metal, true));

            yield return Static("Stove", -2f, 0f, 2.125f, 0f,
                Box(0f, 0.45f, 0f, 0.8f, 0.9f, 0.6f, C.Appliance),
                Box(0f, 0.4f, -0.305f, 0.5f, 0.3f, 0.01f, C.Black, true),
                Cyl(-0.18f, 0.91f, -0.08f, 0.24f, 0.02f, 0.24f, C.Black, true),
                Cyl(0.18f, 0.91f, 0.1f, 0.2f, 0.02f, 0.2f, C.Black, true));

            yield return Static("Pan", -2.18f, 0.92f, 2.04f, 0f,
                Cyl(0f, 0.045f, 0f, 0.34f, 0.09f, 0.34f, C.Black),
                Box(0f, 0.07f, -0.32f, 0.05f, 0.03f, 0.32f, C.WoodDark));

            yield return Static("CabinetRight", -0.95f, 0f, 2.125f, 0f,
                Box(0f, 0.43f, 0f, 1.3f, 0.86f, 0.6f, C.Wood),
                Box(0f, 0.885f, -0.02f, 1.34f, 0.05f, 0.64f, C.Counter),
                Box(0.1f, 0.915f, 0f, 0.5f, 0.01f, 0.4f, C.Metal, true),
                Box(-0.35f, 0.7f, -0.31f, 0.22f, 0.03f, 0.03f, C.Metal, true));

            yield return Static("UpperCabinetLeft", -3.75f, 0f, 2.25f, 0f,
                Box(0f, 1.85f, 0f, 2.7f, 0.7f, 0.35f, C.Wood),
                Box(-0.7f, 1.6f, -0.18f, 0.03f, 0.18f, 0.03f, C.Metal, true),
                Box(0.7f, 1.6f, -0.18f, 0.03f, 0.18f, 0.03f, C.Metal, true));

            yield return Static("Hood", -2f, 0f, 2.2f, 0f,
                Box(0f, 1.75f, 0f, 0.8f, 0.2f, 0.45f, C.Metal),
                Box(0f, 2.25f, 0.05f, 0.3f, 0.8f, 0.3f, C.Metal));

            yield return Static("UpperCabinetRight", -0.95f, 0f, 2.25f, 0f,
                Box(0f, 1.85f, 0f, 1.3f, 0.7f, 0.35f, C.Wood),
                Box(0f, 1.6f, -0.18f, 0.03f, 0.18f, 0.03f, C.Metal, true));

            yield return Dynamic("KitchenTable", -3.2f, 0f, -0.6f, 0f, 15f,
                Box(0f, 0.75f, 0f, 1.4f, 0.06f, 0.9f, C.Wood),
                Box(-0.62f, 0.36f, -0.38f, 0.07f, 0.72f, 0.07f, C.WoodDark),
                Box(0.62f, 0.36f, -0.38f, 0.07f, 0.72f, 0.07f, C.WoodDark),
                Box(-0.62f, 0.36f, 0.38f, 0.07f, 0.72f, 0.07f, C.WoodDark),
                Box(0.62f, 0.36f, 0.38f, 0.07f, 0.72f, 0.07f, C.WoodDark));

            yield return Chair("ChairSouth", -3.2f, -1.45f, 180f);
            yield return Chair("ChairNorth", -3.2f, 0.25f, 0f);

            yield return Dynamic("Plate1", -3.5f, 0.79f, -0.6f, 0f, 0.3f,
                Cyl(0f, 0.015f, 0f, 0.26f, 0.03f, 0.26f, C.Ceramic));
            yield return Dynamic("Plate2", -2.85f, 0.79f, -0.4f, 0f, 0.3f,
                Cyl(0f, 0.015f, 0f, 0.26f, 0.03f, 0.26f, C.Ceramic));
            yield return Dynamic("Mug", -3f, 0.79f, -0.85f, 0f, 0.25f,
                Cyl(0f, 0.06f, 0f, 0.1f, 0.12f, 0.1f, C.Accent));

            yield return Dynamic("TrashBin", -5.6f, 0f, 1.2f, 0f, 3f,
                Cyl(0f, 0.3f, 0f, 0.35f, 0.6f, 0.35f, C.Metal));
        }

        private static PropDefinition Chair(string id, float x, float z, float yaw) =>
            Dynamic(id, x, 0f, z, yaw, 4f,
                Box(0f, 0.45f, 0f, 0.45f, 0.05f, 0.45f, C.WoodDark),
                Box(0f, 0.75f, 0.2f, 0.45f, 0.55f, 0.05f, C.WoodDark),
                Box(-0.19f, 0.21f, -0.19f, 0.05f, 0.42f, 0.05f, C.WoodDark),
                Box(0.19f, 0.21f, -0.19f, 0.05f, 0.42f, 0.05f, C.WoodDark),
                Box(-0.19f, 0.21f, 0.19f, 0.05f, 0.42f, 0.05f, C.WoodDark),
                Box(0.19f, 0.21f, 0.19f, 0.05f, 0.42f, 0.05f, C.WoodDark));
    }
}
