using UnityEngine;

namespace FB.Core.Meshes
{
    /// <summary>
    /// Box mesh with UVs in meters (1 UV unit = 1 m), so tiled textures (parquet, tiles, wallpaper) keep their
    /// real size on boxes of any proportions. <paramref name="uvOffset"/> keeps patterns continuous across pieces.
    /// </summary>
    public static class BoxMeshFactory
    {
        public static Mesh Create(string name, Vector3 size, Vector3 uvOffset)
        {
            Vector3 h = size * 0.5f;
            Vector3[] vertices = new Vector3[24];
            Vector3[] normals = new Vector3[24];
            Vector2[] uvs = new Vector2[24];
            int[] triangles = new int[36];
            int v = 0;
            int t = 0;

            Face(Vector3.right, Vector3.forward, Vector3.up, h, uvOffset, vertices, normals, uvs, triangles, ref v, ref t);
            Face(Vector3.left, Vector3.back, Vector3.up, h, uvOffset, vertices, normals, uvs, triangles, ref v, ref t);
            Face(Vector3.up, Vector3.right, Vector3.forward, h, uvOffset, vertices, normals, uvs, triangles, ref v, ref t);
            Face(Vector3.down, Vector3.right, Vector3.back, h, uvOffset, vertices, normals, uvs, triangles, ref v, ref t);
            Face(Vector3.forward, Vector3.left, Vector3.up, h, uvOffset, vertices, normals, uvs, triangles, ref v, ref t);
            Face(Vector3.back, Vector3.right, Vector3.up, h, uvOffset, vertices, normals, uvs, triangles, ref v, ref t);

            Mesh mesh = new Mesh { name = name, vertices = vertices, normals = normals, uv = uvs, triangles = triangles };
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            return mesh;
        }

        private static void Face(Vector3 normal, Vector3 uAxis, Vector3 vAxis, Vector3 half, Vector3 uvOffset,
            Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles, ref int v, ref int t)
        {
            Vector3 center = Vector3.Scale(normal, half);
            Vector3 u = Vector3.Scale(uAxis, half);
            Vector3 w = Vector3.Scale(vAxis, half);
            Vector3[] corners = { center - u - w, center - u + w, center + u + w, center + u - w };
            for (int i = 0; i < 4; i++)
            {
                Vector3 p = corners[i];
                vertices[v + i] = p;
                normals[v + i] = normal;
                Vector3 world = p + uvOffset;
                uvs[v + i] = new Vector2(Vector3.Dot(world, uAxis), Vector3.Dot(world, vAxis));
            }

            // Clockwise when looking at the face from outside (Unity front faces).
            triangles[t++] = v;
            triangles[t++] = v + 1;
            triangles[t++] = v + 2;
            triangles[t++] = v;
            triangles[t++] = v + 2;
            triangles[t++] = v + 3;
            v += 4;
        }
    }
}
