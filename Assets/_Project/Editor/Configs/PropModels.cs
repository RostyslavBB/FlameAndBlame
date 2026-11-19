using System.Collections.Generic;
using FB.Game.Level;
using UnityEngine;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>
    /// Props that use CC0 models (Kenney Furniture Kit) instead of procedural parts: complex silhouettes that read
    /// better on video. Each entry: model, the box it is fitted into (m), and its materials recolored to the palette.
    /// </summary>
    internal static class PropModels
    {
        private const string Folder = "Assets/_Project/Models/Kenney/";

        private static readonly Dictionary<string, (string Model, Vector3 Fit, ModelMaterial[] Materials)> Table =
            new Dictionary<string, (string, Vector3, ModelMaterial[])>
            {
                ["Sofa"] = ("loungeSofa", new Vector3(2.2f, 1f, 0.95f), M(("carpet", C.Sofa), ("wood", C.WoodDark))),
                ["TV"] = ("televisionModern", new Vector3(1.4f, 1f, 0.35f),
                    M(("metal", C.TvScreen), ("metalDark", C.Black))),
                ["Fridge"] = ("kitchenFridge", new Vector3(0.85f, 1.95f, 0.8f),
                    M(("metal", C.Appliance), ("metalLight", C.Appliance), ("metalDark", C.Metal), ("glass", C.Window))),

                ["Bed"] = ("bedDouble", new Vector3(1.6f, 0.75f, 2.1f),
                    M(("carpet", C.Curtain), ("carpetWhite", C.Ceramic), ("wood", C.Wood), ("metal", C.Metal))),
                ["NightstandLeft"] = ("cabinetBedDrawerTable", new Vector3(0.45f, 0.5f, 0.4f), Nightstand()),
                ["NightstandRight"] = ("cabinetBedDrawerTable", new Vector3(0.45f, 0.5f, 0.4f), Nightstand()),
                ["NightLamp"] = ("lampSquareTable", new Vector3(0.22f, 0.45f, 0.22f), Lamp()),
                ["Wardrobe"] = ("bookcaseClosedDoors", new Vector3(1f, 2.1f, 0.6f), M(("wood", C.Wood), ("metal", C.Metal))),
                ["Desk"] = ("desk", new Vector3(1.2f, 0.76f, 0.6f), M(("wood", C.Wood), ("metal", C.Metal))),
                ["Laptop"] = ("laptop", new Vector3(0.36f, 0.25f, 0.3f),
                    M(("metal", C.TvScreen), ("metalDark", C.Black), ("metalMedium", C.Metal))),
                ["DeskChair"] = ("chairDesk", new Vector3(0.55f, 0.95f, 0.55f),
                    M(("metalMedium", C.Metal), ("carpet", C.Accent))),

                ["Bathtub"] = ("bathtub", new Vector3(1.7f, 0.6f, 0.8f),
                    M(("carpetWhite", C.Ceramic), ("metalLight", C.Ceramic), ("metalDark", C.Metal))),
                ["Toilet"] = ("toilet", new Vector3(0.4f, 0.75f, 0.65f),
                    M(("carpetWhite", C.Ceramic), ("metalLight", C.Ceramic), ("metalDark", C.Metal), ("_defaultMat", C.Ceramic))),
                ["BathSink"] = ("bathroomSink", new Vector3(0.5f, 0.9f, 0.45f),
                    M(("_defaultMat", C.Ceramic), ("metalLight", C.Metal), ("carpetWhite", C.Ceramic))),
                ["BathMirror"] = ("bathroomMirror", new Vector3(0.55f, 0.8f, 0.25f),
                    M(("wood", C.WoodDark), ("metal", C.Metal), ("glass", C.Window))),

                ["CeilingLampHall"] = ("lampSquareCeiling", new Vector3(0.32f, 0.4f, 0.32f), Lamp()),
                ["CeilingLampKitchen"] = ("lampSquareCeiling", new Vector3(0.32f, 0.4f, 0.32f), Lamp()),
                ["CeilingLampLiving"] = ("lampSquareCeiling", new Vector3(0.32f, 0.4f, 0.32f), Lamp()),
                ["CeilingLampBath"] = ("lampSquareCeiling", new Vector3(0.32f, 0.4f, 0.32f), Lamp()),
                ["CeilingLampBedroom"] = ("lampSquareCeiling", new Vector3(0.32f, 0.4f, 0.32f), Lamp()),
                ["CoatRack"] = ("coatRackStanding", new Vector3(0.5f, 1.8f, 0.5f), M(("wood", C.WoodDark))),
                ["Radio"] = ("radio", new Vector3(0.36f, 0.26f, 0.12f),
                    M(("metalMedium", C.Accent), ("wood", C.Wood), ("metal", C.Metal))),
                ["PlantPot"] = ("plantSmall1", new Vector3(0.18f, 0.28f, 0.18f), M(("wood", C.Pot), ("plant", C.Plant)))
            };

        public static IEnumerable<PropDefinition> Apply(IEnumerable<PropDefinition> props)
        {
            foreach (PropDefinition prop in props)
            {
                if (Table.TryGetValue(prop.Id, out (string Model, Vector3 Fit, ModelMaterial[] Materials) entry))
                {
                    prop.WithModel(Folder + entry.Model + ".fbx", entry.Fit, entry.Materials);
                }

                yield return prop;
            }
        }

        private static ModelMaterial[] Nightstand() =>
            M(("wood", C.Wood), ("_defaultMat", C.WoodDark), ("metal", C.Metal));

        private static ModelMaterial[] Lamp() => M(("lamp", C.LampGlow), ("metal", C.Metal));

        private static ModelMaterial[] M(params (string Name, C Color)[] materials)
        {
            ModelMaterial[] result = new ModelMaterial[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                result[i] = new ModelMaterial(materials[i].Name, materials[i].Color);
            }

            return result;
        }
    }
}
