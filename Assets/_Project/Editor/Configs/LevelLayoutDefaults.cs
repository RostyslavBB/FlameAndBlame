using System.Collections.Generic;
using FB.Game.Config;
using FB.Game.Level;
using UnityEngine;

namespace FB.Editor.Configs
{
    /// <summary>
    /// Default apartment, top view (X right, Z up), meters:
    /// Hall x[-8,-6] | Kitchen x[-6,0] | Living room x[0,7], all rooms z[-2.5,2.5].
    /// The wide kitchen→living doorway is next to the stove so fire can travel cabinets → curtain → sofa.
    /// </summary>
    internal static class LevelLayoutDefaults
    {
        /// <summary>Bump when the defaults change: Build All regenerates layouts made from an older version.</summary>
        public const int Version = 3;

        public static void Apply(LevelLayoutConfig config)
        {
            config.EditorSetLayout(Version, new Vector3(-7.2f, 0f, -1f), 90f, HallProps.HoseAnchor,
                Rooms(), Walls(), Doorways(), Props());
        }

        private static IEnumerable<RoomDefinition> Rooms()
        {
            yield return new RoomDefinition("Hall", new Vector2(-8f, -2.5f), new Vector2(-6f, 2.5f), PaletteColor.FloorHall);
            yield return new RoomDefinition("Kitchen", new Vector2(-6f, -2.5f), new Vector2(0f, 2.5f), PaletteColor.FloorKitchen);
            yield return new RoomDefinition("Living", new Vector2(0f, -2.5f), new Vector2(7f, 2.5f), PaletteColor.FloorLiving);
        }

        private static IEnumerable<WallDefinition> Walls()
        {
            yield return new WallDefinition(new Vector2(-8f, 2.5f), new Vector2(7f, 2.5f));
            yield return new WallDefinition(new Vector2(-8f, -2.5f), new Vector2(7f, -2.5f));
            yield return new WallDefinition(new Vector2(-8f, -2.5f), new Vector2(-8f, 2.5f));
            yield return new WallDefinition(new Vector2(7f, -2.5f), new Vector2(7f, 2.5f));
            yield return new WallDefinition(new Vector2(-6f, -2.5f), new Vector2(-6f, 2.5f));
            yield return new WallDefinition(new Vector2(0f, -2.5f), new Vector2(0f, 2.5f));
        }

        private static IEnumerable<DoorwayDefinition> Doorways()
        {
            yield return new DoorwayDefinition(new Vector2(-6f, -1f), 1.2f, 2.2f);
            yield return new DoorwayDefinition(new Vector2(0f, 1.15f), 2f, 2.3f);
        }

        private static IEnumerable<PropDefinition> Props()
        {
            List<PropDefinition> props = new List<PropDefinition>();
            props.AddRange(HallProps.Create());
            props.AddRange(KitchenProps.Create());
            props.AddRange(LivingRoomProps.Create());
            return PropSurfaces.Apply(props);
        }
    }
}
