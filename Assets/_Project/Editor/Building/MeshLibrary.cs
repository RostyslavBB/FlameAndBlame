using FB.Core.Meshes;
using FB.Game.Level;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>Generates the flat-shaded low-poly meshes as assets and maps <see cref="PartShape"/> to them.</summary>
    internal sealed class MeshLibrary
    {
        private readonly Mesh _cube;
        private readonly Mesh _cylinder;
        private readonly Mesh _cone;
        private readonly Mesh _sphere;

        public MeshLibrary()
        {
            _cube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            _cylinder = Save(LowPolyMeshFactory.Cylinder("LowPolyCylinder", 10), "LowPolyCylinder");
            _cone = Save(LowPolyMeshFactory.Cylinder("LowPolyCone", 10, 0.25f, 0.5f), "LowPolyCone");
            _sphere = Save(LowPolyMeshFactory.Sphere("LowPolySphere", 8, 6), "LowPolySphere");
            AssetDatabase.SaveAssets();
        }

        public Mesh Get(PartShape shape)
        {
            switch (shape)
            {
                case PartShape.Cylinder: return _cylinder;
                case PartShape.Cone: return _cone;
                case PartShape.Sphere: return _sphere;
                default: return _cube;
            }
        }

        private static Mesh Save(Mesh mesh, string name) =>
            EditorAssetUtility.SaveOrReplace(mesh, $"{AssetPaths.Meshes}/{name}.asset");
    }
}
