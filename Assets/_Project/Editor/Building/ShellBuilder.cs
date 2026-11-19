using System.Collections.Generic;
using FB.Core.Meshes;
using FB.Game.Config;
using FB.Game.Level;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>
    /// Builds floors, walls (doorway cut-outs, lintels, door casings, skirting boards) and the ceiling.
    /// Every piece gets its own box mesh with UVs in meters, so parquet, tiles and wallpaper keep their real scale
    /// and continue across neighbouring pieces. The meshes are stored inside the scene.
    /// </summary>
    internal sealed class ShellBuilder
    {
        private const float SkirtingHeight = 0.09f;
        private const float SkirtingDepth = 0.015f;
        private const float CasingWidth = 0.08f;
        private const float CasingDepth = 0.025f;

        private readonly LevelLayoutConfig _layout;
        private readonly MaterialLibrary _materials;

        public ShellBuilder(LevelLayoutConfig layout, MaterialLibrary materials)
        {
            _layout = layout;
            _materials = materials;
        }

        public void Build(Transform parent)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            foreach (RoomDefinition room in _layout.Rooms)
            {
                Vector2 size = room.Max - room.Min;
                Vector2 center = (room.Min + room.Max) * 0.5f;
                Vector3 position = new Vector3(center.x, -0.05f, center.y);
                CreateBox("Floor_" + room.Name, parent, position, Quaternion.identity,
                    new Vector3(size.x, 0.1f, size.y), position, room.Floor);
                min = Vector2.Min(min, room.Min);
                max = Vector2.Max(max, room.Max);
            }

            for (int i = 0; i < _layout.Walls.Count; i++)
            {
                BuildWall(_layout.Walls[i], i, parent);
            }

            Vector2 bounds = max - min;
            Vector2 middle = (min + max) * 0.5f;
            Vector3 ceilingPosition = new Vector3(middle.x, _layout.WallHeight + 0.05f, middle.y);
            GameObject ceiling = CreateBox("Ceiling", parent, ceilingPosition, Quaternion.identity,
                new Vector3(bounds.x + 1f, 0.1f, bounds.y + 1f), ceilingPosition, _layout.CeilingColor);
            ceiling.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        }

        private void BuildWall(WallDefinition wall, int index, Transform parent)
        {
            float thickness = _layout.WallThickness;
            float height = _layout.WallHeight;
            Vector2 delta = wall.To - wall.From;
            float length = delta.magnitude;
            Vector2 direction = delta / length;

            List<Vector3> openings = new List<Vector3>();
            foreach (DoorwayDefinition door in _layout.Doorways)
            {
                Vector2 local = door.Center - wall.From;
                float along = Vector2.Dot(local, direction);
                float across = Mathf.Abs(direction.x * local.y - direction.y * local.x);
                if (across < thickness && along > 0f && along < length)
                {
                    openings.Add(new Vector3(along - door.Width * 0.5f, along + door.Width * 0.5f, door.Height));
                }
            }

            openings.Sort((a, b) => a.x.CompareTo(b.x));
            float cursor = -thickness * 0.5f;
            int piece = 0;
            foreach (Vector3 opening in openings)
            {
                Segment(wall, direction, cursor, opening.x, 0f, height, thickness, PaletteColor.Wall,
                    $"Wall_{index}_{piece++}", parent);
                Skirting(wall, direction, cursor, opening.x, $"Skirting_{index}_{piece}", parent);
                Segment(wall, direction, opening.x, opening.y, opening.z, height, thickness, PaletteColor.Wall,
                    $"Lintel_{index}_{piece++}", parent);
                Casing(wall, direction, opening, $"Casing_{index}_{piece}", parent);
                cursor = opening.y;
            }

            Segment(wall, direction, cursor, length + thickness * 0.5f, 0f, height, thickness, PaletteColor.Wall,
                $"Wall_{index}_{piece}", parent);
            Skirting(wall, direction, cursor, length + thickness * 0.5f, $"Skirting_{index}_end", parent);
        }

        private void Skirting(WallDefinition wall, Vector2 direction, float from, float to, string name, Transform parent)
        {
            Segment(wall, direction, from, to, 0f, SkirtingHeight, _layout.WallThickness + SkirtingDepth * 2f,
                PaletteColor.Trim, name, parent);
        }

        private void Casing(WallDefinition wall, Vector2 direction, Vector3 opening, string name, Transform parent)
        {
            float depth = _layout.WallThickness + CasingDepth * 2f;
            Segment(wall, direction, opening.x - CasingWidth, opening.x, 0f, opening.z, depth, PaletteColor.Trim,
                name + "_A", parent);
            Segment(wall, direction, opening.y, opening.y + CasingWidth, 0f, opening.z, depth, PaletteColor.Trim,
                name + "_B", parent);
            Segment(wall, direction, opening.x - CasingWidth, opening.y + CasingWidth, opening.z,
                opening.z + CasingWidth, depth, PaletteColor.Trim, name + "_Top", parent);
        }

        private void Segment(WallDefinition wall, Vector2 direction, float from, float to, float bottom, float top,
            float thickness, PaletteColor color, string name, Transform parent)
        {
            if (to - from < 0.01f || top - bottom < 0.01f)
            {
                return;
            }

            float along = (from + to) * 0.5f;
            Vector2 center = wall.From + direction * along;
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.y));
            float y = (bottom + top) * 0.5f;
            // Local UV offset: height for v, distance along the wall for u, so wallpaper continues across pieces.
            CreateBox(name, parent, new Vector3(center.x, y, center.y), rotation, new Vector3(thickness, top - bottom, to - from),
                new Vector3(0f, y, along), color);
        }

        private GameObject CreateBox(string name, Transform parent, Vector3 position, Quaternion rotation,
            Vector3 size, Vector3 uvOffset, PaletteColor color)
        {
            GameObject box = new GameObject(name);
            box.transform.SetParent(parent, false);
            box.transform.SetPositionAndRotation(position, rotation);
            box.AddComponent<MeshFilter>().sharedMesh = BoxMeshFactory.Create(name, size, uvOffset);
            box.AddComponent<MeshRenderer>().sharedMaterial = _materials.Get(color);
            box.AddComponent<BoxCollider>().size = size;
            GameObjectUtility.SetStaticEditorFlags(box, StaticEditorFlags.BatchingStatic);
            return box;
        }
    }
}
