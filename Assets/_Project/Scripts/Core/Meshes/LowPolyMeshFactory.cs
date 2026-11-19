using UnityEngine;

namespace FB.Core.Meshes
{
    /// <summary>
    /// Procedural flat-shaded meshes in a unit box (-0.5..0.5 on every axis), so they scale like Unity's cube.
    /// </summary>
    public static class LowPolyMeshFactory
    {
        public static Mesh Cylinder(string name, int segments, float topRadius = 0.5f, float bottomRadius = 0.5f)
        {
            FlatMeshWriter writer = new FlatMeshWriter(Vector3.zero);
            Vector3 top = new Vector3(0f, 0.5f, 0f);
            Vector3 bottom = new Vector3(0f, -0.5f, 0f);

            for (int i = 0; i < segments; i++)
            {
                Vector3 dirA = Ring(i, segments);
                Vector3 dirB = Ring(i + 1, segments);
                Vector3 topA = top + dirA * topRadius;
                Vector3 topB = top + dirB * topRadius;
                Vector3 bottomA = bottom + dirA * bottomRadius;
                Vector3 bottomB = bottom + dirB * bottomRadius;

                writer.Quad(bottomA, bottomB, topB, topA);
                writer.Triangle(top, topA, topB);
                writer.Triangle(bottom, bottomA, bottomB);
            }

            return writer.Build(name);
        }

        public static Mesh Sphere(string name, int longitude, int latitude)
        {
            FlatMeshWriter writer = new FlatMeshWriter(Vector3.zero);
            for (int lat = 0; lat < latitude; lat++)
            {
                for (int lon = 0; lon < longitude; lon++)
                {
                    Vector3 a = SpherePoint(lon, lat, longitude, latitude);
                    Vector3 b = SpherePoint(lon + 1, lat, longitude, latitude);
                    Vector3 c = SpherePoint(lon + 1, lat + 1, longitude, latitude);
                    Vector3 d = SpherePoint(lon, lat + 1, longitude, latitude);
                    writer.Quad(a, b, c, d);
                }
            }

            return writer.Build(name);
        }

        private static Vector3 Ring(int index, int segments)
        {
            float angle = index * Mathf.PI * 2f / segments;
            return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        }

        private static Vector3 SpherePoint(int lon, int lat, int longitude, int latitude)
        {
            float theta = lat * Mathf.PI / latitude;
            float phi = lon * Mathf.PI * 2f / longitude;
            float ring = Mathf.Sin(theta) * 0.5f;
            return new Vector3(Mathf.Cos(phi) * ring, Mathf.Cos(theta) * 0.5f, Mathf.Sin(phi) * ring);
        }
    }
}
