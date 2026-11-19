using System.Collections.Generic;
using FB.Game.Level;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>Builds floors, walls (with doorway cut-outs and lintels) and the ceiling from the layout.</summary>
    internal sealed class ShellBuilder
    {
        private readonly LevelLayoutConfig _layout;
        private readonly MaterialLibrary _materials;
        private readonly Mesh _cube;

        public ShellBuilder(LevelLayoutConfig layout, MaterialLibrary materials, MeshLibrary meshes)
        {
            _layout = layout;
            _materials = materials;
            _cube = meshes.Get(PartShape.Box);
        }

        public void Build(Transform parent)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            foreach (RoomDefinition room in _layout.Rooms)
            {
                Vector2 size = room.Max - room.Min;
                Vector2 center = (room.Min + room.Max) * 0.5f;
                CreateBox("Floor_" + room.Name, parent, new Vector3(center.x, -0.05f, center.y),
                    Quaternion.identity, new Vector3(size.x, 0.1f, size.y), _materials.Get(room.Floor), true);
                min = Vector2.Min(min, room.Min);
                max = Vector2.Max(max, room.Max);
            }

            for (int i = 0; i < _layout.Walls.Count; i++)
            {
                BuildWall(_layout.Walls[i], i, parent);
            }

            Vector2 bounds = max - min;
            Vector2 middle = (min + max) * 0.5f;
            GameObject ceiling = CreateBox("Ceiling", parent, new Vector3(middle.x, _layout.WallHeight + 0.05f, middle.y),
                Quaternion.identity, new Vector3(bounds.x + 1f, 0.1f, bounds.y + 1f),
                _materials.Get(_layout.CeilingColor), true);
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
                WallPiece(wall, direction, cursor, opening.x, 0f, height, $"Wall_{index}_{piece++}", parent);
                WallPiece(wall, direction, opening.x, opening.y, opening.z, height, $"Lintel_{index}_{piece++}", parent);
                cursor = opening.y;
            }

            WallPiece(wall, direction, cursor, length + thickness * 0.5f, 0f, height, $"Wall_{index}_{piece}", parent);
        }

        private void WallPiece(WallDefinition wall, Vector2 direction, float from, float to, float bottom, float top,
            string name, Transform parent)
        {
            if (to - from < 0.01f || top - bottom < 0.01f)
            {
                return;
            }

            Vector2 center = wall.From + direction * ((from + to) * 0.5f);
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.y));
            CreateBox(name, parent, new Vector3(center.x, (bottom + top) * 0.5f, center.y), rotation,
                new Vector3(_layout.WallThickness, top - bottom, to - from), _materials.Get(_layout.WallColor), true);
        }

        private GameObject CreateBox(string name, Transform parent, Vector3 position, Quaternion rotation,
            Vector3 scale, Material material, bool isStatic)
        {
            GameObject box = new GameObject(name);
            box.transform.SetParent(parent, false);
            box.transform.SetPositionAndRotation(position, rotation);
            box.transform.localScale = scale;
            box.AddComponent<MeshFilter>().sharedMesh = _cube;
            box.AddComponent<MeshRenderer>().sharedMaterial = material;
            box.AddComponent<BoxCollider>();
            if (isStatic)
            {
                GameObjectUtility.SetStaticEditorFlags(box, StaticEditorFlags.BatchingStatic);
            }

            return box;
        }
    }
}
