using System.Collections.Generic;
using FB.Game.Level;
using UnityEngine;
using static FB.Editor.Configs.Part;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>Living room x[0,7]. Sofa under the curtained window, TV with the power strip on the opposite wall.</summary>
    internal static class LivingRoomProps
    {
        public static IEnumerable<PropDefinition> Create()
        {
            yield return Static("Window", 2f, 0f, 2.41f, 0f,
                Box(0f, 1.6f, 0f, 1.8f, 1.2f, 0.02f, C.Window, true),
                Box(0f, 2.23f, -0.02f, 1.9f, 0.06f, 0.05f, C.Wood, true),
                Box(0f, 0.98f, -0.05f, 1.95f, 0.05f, 0.12f, C.Wood));

            yield return Static("CurtainRod", 2f, 0f, 2.33f, 0f,
                Cyl(0f, 2.32f, 0f, 0.04f, 3.2f, 0.04f, C.Metal, true, new Vector3(0f, 0f, 90f)));

            yield return Curtain("CurtainLeft", 0.75f);
            yield return Curtain("CurtainRight", 3.25f);

            yield return Static("Sofa", 2f, 0f, 1.8f, 0f,
                Box(0f, 0.25f, 0f, 2.2f, 0.5f, 0.9f, C.Sofa),
                Box(0f, 0.72f, 0.35f, 2.2f, 0.55f, 0.25f, C.Sofa),
                Box(-1f, 0.35f, 0f, 0.25f, 0.7f, 0.9f, C.Sofa),
                Box(1f, 0.35f, 0f, 0.25f, 0.7f, 0.9f, C.Sofa));

            yield return Dynamic("Cushion1", 1.4f, 0.5f, 1.75f, 0f, 0.5f,
                Box(0f, 0.2f, 0.1f, 0.5f, 0.4f, 0.15f, C.Cushion, false, new Vector3(-15f, 0f, 0f)));
            yield return Dynamic("Cushion2", 2.6f, 0.5f, 1.75f, 0f, 0.5f,
                Box(0f, 0.2f, 0.1f, 0.5f, 0.4f, 0.15f, C.Cushion, false, new Vector3(-15f, 0f, 0f)));

            yield return Dynamic("FloorLamp", 3.95f, 0f, 2f, 0f, 5f,
                Cyl(0f, 0.03f, 0f, 0.36f, 0.06f, 0.36f, C.Black),
                Cyl(0f, 0.8f, 0f, 0.05f, 1.5f, 0.05f, C.Metal),
                Cone(0f, 1.65f, 0f, 0.55f, 0.4f, 0.55f, C.LampShade),
                Ball(0f, 1.52f, 0f, 0.15f, 0.15f, 0.15f, C.Bulb, true));

            yield return Static("Rug", 3.2f, 0f, -0.3f, 0f,
                Box(0f, 0.01f, 0f, 3.2f, 0.02f, 2.2f, C.Rug, true));

            yield return Dynamic("CoffeeTable", 3.2f, 0f, -0.3f, 0f, 8f,
                Box(0f, 0.4f, 0f, 1.1f, 0.06f, 0.6f, C.Wood),
                Box(-0.48f, 0.185f, -0.24f, 0.06f, 0.37f, 0.06f, C.WoodDark),
                Box(0.48f, 0.185f, -0.24f, 0.06f, 0.37f, 0.06f, C.WoodDark),
                Box(-0.48f, 0.185f, 0.24f, 0.06f, 0.37f, 0.06f, C.WoodDark),
                Box(0.48f, 0.185f, 0.24f, 0.06f, 0.37f, 0.06f, C.WoodDark));
            yield return Dynamic("CoffeeMug", 3.4f, 0.44f, -0.25f, 0f, 0.25f,
                Cyl(0f, 0.06f, 0f, 0.1f, 0.12f, 0.1f, C.Ceramic));

            yield return Static("TvStand", 3.2f, 0f, -2.15f, 180f,
                Box(0f, 0.25f, 0f, 1.6f, 0.5f, 0.45f, C.WoodDark),
                Box(-0.4f, 0.25f, -0.23f, 0.7f, 0.35f, 0.01f, C.Wood, true),
                Box(0.4f, 0.25f, -0.23f, 0.7f, 0.35f, 0.01f, C.Wood, true));

            yield return Static("TV", 3.2f, 0.5f, -2.2f, 180f,
                Box(0f, 0.02f, 0f, 0.4f, 0.04f, 0.25f, C.Black),
                Box(0f, 0.12f, 0.02f, 0.06f, 0.2f, 0.04f, C.Black),
                Box(0f, 0.6f, 0.02f, 1.4f, 0.8f, 0.06f, C.Black),
                Ball(0.62f, 0.24f, -0.02f, 0.03f, 0.03f, 0.03f, C.Accent, true));

            yield return Static("PowerStrip", 4.3f, 0f, -2.15f, 0f,
                Box(0f, 0.03f, 0f, 0.45f, 0.06f, 0.12f, C.Appliance),
                Box(0.17f, 0.065f, 0f, 0.06f, 0.02f, 0.06f, C.Accent, true),
                Box(-0.1f, 0.061f, 0f, 0.06f, 0.01f, 0.06f, C.Black, true),
                Box(0.03f, 0.061f, 0f, 0.06f, 0.01f, 0.06f, C.Black, true),
                Box(-0.62f, 0.01f, -0.02f, 0.8f, 0.02f, 0.02f, C.Black, true),
                Box(0.4f, 0.01f, 0.1f, 0.4f, 0.02f, 0.02f, C.Black, true, new Vector3(0f, -30f, 0f)));

            yield return Static("Ficus", 0.65f, 0f, -1.95f, 0f,
                Cone(0f, 0.25f, 0f, 0.5f, 0.5f, 0.5f, C.Pot, false, new Vector3(180f, 0f, 0f)),
                Cyl(0f, 0.75f, 0f, 0.07f, 0.6f, 0.07f, C.WoodDark),
                Ball(0f, 1.3f, 0f, 0.8f, 0.7f, 0.8f, C.Plant),
                Ball(0.2f, 1.65f, 0.1f, 0.55f, 0.5f, 0.55f, C.Plant),
                Ball(-0.2f, 1.1f, -0.15f, 0.5f, 0.45f, 0.5f, C.Plant));

            yield return Static("Bookshelf", 6.68f, 0f, 0.5f, 90f, BookshelfParts.Create(7));
        }

        private static PropDefinition Curtain(string id, float x) =>
            Static(id, x, 0f, 2.33f, 0f,
                Box(-0.2f, 1.2f, -0.03f, 0.2f, 2.2f, 0.06f, C.Curtain),
                Box(0f, 1.2f, 0f, 0.22f, 2.2f, 0.06f, C.Curtain),
                Box(0.2f, 1.2f, -0.03f, 0.2f, 2.2f, 0.06f, C.Curtain));
    }
}
