using System.Collections.Generic;
using FB.Game.Level;
using UnityEngine;
using C = FB.Game.Config.PaletteColor;

namespace FB.Editor.Configs
{
    /// <summary>
    /// The few props that use CC0 models (Kenney Furniture Kit) instead of procedural parts: complex silhouettes
    /// that read better on video. Materials are recolored into the project palette.
    /// </summary>
    internal static class PropModels
    {
        private const string Folder = "Assets/_Project/Models/Kenney/";

        public static IEnumerable<PropDefinition> Apply(IEnumerable<PropDefinition> props)
        {
            foreach (PropDefinition prop in props)
            {
                switch (prop.Id)
                {
                    case "Sofa":
                        prop.WithModel(Folder + "loungeSofa.fbx", new Vector3(2.2f, 1f, 0.95f),
                            new ModelMaterial("carpet", C.Sofa), new ModelMaterial("wood", C.WoodDark));
                        break;
                    case "TV":
                        prop.WithModel(Folder + "televisionModern.fbx", new Vector3(1.4f, 1f, 0.35f),
                            new ModelMaterial("metal", C.Black), new ModelMaterial("metalDark", C.Metal));
                        break;
                    case "Fridge":
                        prop.WithModel(Folder + "kitchenFridge.fbx", new Vector3(0.85f, 1.95f, 0.8f),
                            new ModelMaterial("metal", C.Appliance), new ModelMaterial("metalLight", C.Appliance),
                            new ModelMaterial("metalDark", C.Metal), new ModelMaterial("glass", C.Window));
                        break;
                }

                yield return prop;
            }
        }
    }
}
