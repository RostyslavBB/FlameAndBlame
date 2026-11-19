using FB.Game.Config;
using FB.Game.Level;
using UnityEngine;

namespace FB.Editor.Configs
{
    /// <summary>Short factory for prop parts so layout tables stay readable: position, size, color.</summary>
    internal static class Part
    {
        public static PropPart Box(float x, float y, float z, float sx, float sy, float sz, PaletteColor color,
            bool noCollider = false, Vector3 euler = default) =>
            new PropPart(PartShape.Box, new Vector3(x, y, z), new Vector3(sx, sy, sz), color, euler, noCollider);

        public static PropPart Cyl(float x, float y, float z, float sx, float sy, float sz, PaletteColor color,
            bool noCollider = false, Vector3 euler = default) =>
            new PropPart(PartShape.Cylinder, new Vector3(x, y, z), new Vector3(sx, sy, sz), color, euler, noCollider);

        public static PropPart Cone(float x, float y, float z, float sx, float sy, float sz, PaletteColor color,
            bool noCollider = false, Vector3 euler = default) =>
            new PropPart(PartShape.Cone, new Vector3(x, y, z), new Vector3(sx, sy, sz), color, euler, noCollider);

        public static PropPart Ball(float x, float y, float z, float sx, float sy, float sz, PaletteColor color,
            bool noCollider = false) =>
            new PropPart(PartShape.Sphere, new Vector3(x, y, z), new Vector3(sx, sy, sz), color, default, noCollider);

        public static PropDefinition Static(string id, float x, float y, float z, float yaw, params PropPart[] parts) =>
            new PropDefinition(id, new Vector3(x, y, z), yaw, false, 1f, parts);

        public static PropDefinition Dynamic(string id, float x, float y, float z, float yaw, float mass,
            params PropPart[] parts) =>
            new PropDefinition(id, new Vector3(x, y, z), yaw, true, mass, parts);
    }
}
