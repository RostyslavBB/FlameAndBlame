using UnityEngine;

namespace FB.Core.Meshes
{
    /// <summary>Flat XZ grid with shared vertices, centered on the origin (for vertex-displaced surfaces).</summary>
    public static class GridMeshFactory
    {
        public static Mesh Plane(string name, float width, float depth, int cellsX, int cellsZ)
        {
            int columns = cellsX + 1;
            int rows = cellsZ + 1;
            Vector3[] vertices = new Vector3[columns * rows];
            for (int z = 0; z < rows; z++)
            {
                for (int x = 0; x < columns; x++)
                {
                    vertices[z * columns + x] = new Vector3((x / (float)cellsX - 0.5f) * width, 0f,
                        (z / (float)cellsZ - 0.5f) * depth);
                }
            }

            int[] triangles = new int[cellsX * cellsZ * 6];
            int t = 0;
            for (int z = 0; z < cellsZ; z++)
            {
                for (int x = 0; x < cellsX; x++)
                {
                    int i = z * columns + x;
                    triangles[t++] = i;
                    triangles[t++] = i + columns;
                    triangles[t++] = i + 1;
                    triangles[t++] = i + 1;
                    triangles[t++] = i + columns;
                    triangles[t++] = i + columns + 1;
                }
            }

            Mesh mesh = new Mesh { name = name };
            if (vertices.Length > 65535)
            {
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            // The vertex shader lifts the surface by the water depth, so allow some height in the bounds.
            mesh.bounds = new Bounds(Vector3.zero, new Vector3(width, 0.5f, depth));
            return mesh;
        }
    }
}
