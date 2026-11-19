using System.Collections.Generic;
using UnityEngine;

namespace FB.Core.Meshes
{
    /// <summary>
    /// Collects triangles with unshared vertices so normals come out flat (low-poly look).
    /// Triangles are auto-oriented to face away from the mesh center, which is valid for convex shapes.
    /// </summary>
    internal sealed class FlatMeshWriter
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly Vector3 _center;

        public FlatMeshWriter(Vector3 center)
        {
            _center = center;
        }

        public void Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 normal = Vector3.Cross(b - a, c - a);
            if (normal.sqrMagnitude < 1e-12f)
            {
                return;
            }

            Vector3 outward = (a + b + c) / 3f - _center;
            if (Vector3.Dot(normal, outward) < 0f)
            {
                (b, c) = (c, b);
            }

            _vertices.Add(a);
            _vertices.Add(b);
            _vertices.Add(c);
        }

        public void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Triangle(a, b, c);
            Triangle(a, c, d);
        }

        public Mesh Build(string name)
        {
            int[] indices = new int[_vertices.Count];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            Mesh mesh = new Mesh { name = name };
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(indices, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
