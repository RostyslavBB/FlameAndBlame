using System.Collections.Generic;
using FB.Game.Level;
using FB.Game.Surfaces;

namespace FB.Editor.Configs
{
    /// <summary>Gameplay data per prop id: material, price in $, name shown in the blame feed.</summary>
    internal static class PropSurfaces
    {
        private static readonly Dictionary<string, (MaterialKind Kind, int Value, string Name)> Table =
            new Dictionary<string, (MaterialKind, int, string)>
            {
                ["EntryDoor"] = (MaterialKind.Wood, 300, "Вхідні двері"),
                ["ShoeRack"] = (MaterialKind.Wood, 60, "Взуттєва полиця"),

                ["Fridge"] = (MaterialKind.Metal, 900, "Холодильник"),
                ["CabinetLeft"] = (MaterialKind.Wood, 350, "Кухонна шафка"),
                ["Stove"] = (MaterialKind.Metal, 500, "Плита"),
                ["Pan"] = (MaterialKind.Grease, 25, "Сковорідка"),
                ["CabinetRight"] = (MaterialKind.Wood, 250, "Шафка біля плити"),
                ["UpperCabinetLeft"] = (MaterialKind.Wood, 300, "Навісна шафка"),
                ["Hood"] = (MaterialKind.Metal, 150, "Витяжка"),
                ["UpperCabinetRight"] = (MaterialKind.Wood, 200, "Навісна шафка над плитою"),
                ["KitchenTable"] = (MaterialKind.Wood, 180, "Кухонний стіл"),
                ["ChairSouth"] = (MaterialKind.Wood, 60, "Стілець"),
                ["ChairNorth"] = (MaterialKind.Wood, 60, "Стілець"),

                ["CurtainLeft"] = (MaterialKind.Fabric, 120, "Штора"),
                ["CurtainRight"] = (MaterialKind.Fabric, 120, "Штора"),
                ["Sofa"] = (MaterialKind.Fabric, 1200, "Диван"),
                ["Cushion1"] = (MaterialKind.Fabric, 40, "Подушка"),
                ["Cushion2"] = (MaterialKind.Fabric, 40, "Подушка"),
                ["FloorLamp"] = (MaterialKind.Fabric, 150, "Торшер"),
                ["CoffeeTable"] = (MaterialKind.Wood, 220, "Журнальний столик"),
                ["TvStand"] = (MaterialKind.Wood, 200, "Тумба під ТВ"),
                ["TV"] = (MaterialKind.Electronics, 800, "Телевізор"),
                ["PowerStrip"] = (MaterialKind.Electronics, 25, "Подовжувач"),
                ["Ficus"] = (MaterialKind.Plant, 90, "Фікус"),
                ["Bookshelf"] = (MaterialKind.Wood, 400, "Книжкова шафа"),

                ["Bed"] = (MaterialKind.Fabric, 900, "Ліжко"),
                ["NightstandLeft"] = (MaterialKind.Wood, 80, "Тумбочка"),
                ["NightstandRight"] = (MaterialKind.Wood, 80, "Тумбочка"),
                ["NightLamp"] = (MaterialKind.Fabric, 50, "Нічник"),
                ["Wardrobe"] = (MaterialKind.Wood, 600, "Шафа"),
                ["Desk"] = (MaterialKind.Wood, 250, "Письмовий стіл"),
                ["Laptop"] = (MaterialKind.Electronics, 1100, "Ноутбук"),
                ["DeskChair"] = (MaterialKind.Fabric, 120, "Крісло"),

                ["Bathtub"] = (MaterialKind.Metal, 700, "Ванна"),
                ["Toilet"] = (MaterialKind.Metal, 350, "Унітаз"),
                ["BathSink"] = (MaterialKind.Metal, 200, "Раковина"),
                ["WashingMachine"] = (MaterialKind.Electronics, 650, "Пральна машина"),

                ["CoatRack"] = (MaterialKind.Wood, 90, "Вішак"),
                ["Radio"] = (MaterialKind.Electronics, 90, "Радіо"),
                ["PlantPot"] = (MaterialKind.Plant, 25, "Вазон")
            };

        public static IEnumerable<PropDefinition> Apply(IEnumerable<PropDefinition> props)
        {
            foreach (PropDefinition prop in props)
            {
                if (Table.TryGetValue(prop.Id, out (MaterialKind Kind, int Value, string Name) data))
                {
                    prop.WithSurface(data.Kind, data.Value, data.Name);
                }

                yield return prop;
            }
        }
    }
}
