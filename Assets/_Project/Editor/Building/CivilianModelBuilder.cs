using FB.Game.Config;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>
    /// Original cartoon neighbour from primitives: big round head with messy hair, red hoodie, jeans, white
    /// sneakers. Limbs hang from pivots so <c>CivilianAnimator</c> can swing them. Spiral "dizzy" eyes are
    /// a generated texture on two quads.
    /// </summary>
    internal sealed class CivilianModelBuilder
    {
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        private readonly MaterialLibrary _materials;

        public CivilianModelBuilder(MaterialLibrary materials)
        {
            _materials = materials;
        }

        public Transform Body { get; private set; }
        public Transform Head { get; private set; }
        public Transform ArmLeft { get; private set; }
        public Transform ArmRight { get; private set; }
        public Transform LegLeft { get; private set; }
        public Transform LegRight { get; private set; }
        public Transform Mouth { get; private set; }
        public GameObject Eyes { get; private set; }
        public GameObject DizzyEyes { get; private set; }

        public void Build(Transform root)
        {
            Body = Pivot("Body", root, Vector3.zero);
            LegLeft = Leg("LegLeft", -0.1f);
            LegRight = Leg("LegRight", 0.1f);

            Shape(PrimitiveType.Capsule, Body, new Vector3(0f, 0.8f, 0f), new Vector3(0.46f, 0.3f, 0.32f), PaletteColor.Hoodie);
            Shape(PrimitiveType.Cube, Body, new Vector3(0f, 0.72f, 0.155f), new Vector3(0.26f, 0.12f, 0.02f), PaletteColor.Hoodie);
            Shape(PrimitiveType.Sphere, Body, new Vector3(0f, 1.0f, -0.12f), new Vector3(0.4f, 0.2f, 0.2f), PaletteColor.Hoodie);
            Shape(PrimitiveType.Cylinder, Body, new Vector3(-0.05f, 0.86f, 0.16f), new Vector3(0.02f, 0.07f, 0.02f), PaletteColor.Ceramic);
            Shape(PrimitiveType.Cylinder, Body, new Vector3(0.05f, 0.86f, 0.16f), new Vector3(0.02f, 0.07f, 0.02f), PaletteColor.Ceramic);
            ArmLeft = Arm("ArmLeft", -0.27f);
            ArmRight = Arm("ArmRight", 0.27f);
            BuildHead();
        }

        private Transform Leg(string name, float x)
        {
            Transform pivot = Pivot(name, Body, new Vector3(x, 0.58f, 0f));
            Shape(PrimitiveType.Capsule, pivot, new Vector3(0f, -0.27f, 0f), new Vector3(0.17f, 0.27f, 0.17f), PaletteColor.Jeans);
            Shape(PrimitiveType.Cube, pivot, new Vector3(0f, -0.53f, 0.05f), new Vector3(0.17f, 0.1f, 0.28f), PaletteColor.Ceramic);
            Shape(PrimitiveType.Cube, pivot, new Vector3(0f, -0.575f, 0.05f), new Vector3(0.18f, 0.03f, 0.29f), PaletteColor.Accent);
            return pivot;
        }

        private Transform Arm(string name, float x)
        {
            Transform pivot = Pivot(name, Body, new Vector3(x, 0.98f, 0f));
            Shape(PrimitiveType.Capsule, pivot, new Vector3(0f, -0.2f, 0f), new Vector3(0.13f, 0.21f, 0.13f), PaletteColor.Hoodie);
            Shape(PrimitiveType.Sphere, pivot, new Vector3(0f, -0.43f, 0f), Vector3.one * 0.12f, PaletteColor.Skin);
            return pivot;
        }

        private void BuildHead()
        {
            Head = Pivot("Head", Body, new Vector3(0f, 1.05f, 0f));
            Shape(PrimitiveType.Sphere, Head, new Vector3(0f, 0.3f, 0f), new Vector3(0.62f, 0.6f, 0.58f), PaletteColor.Skin);
            Shape(PrimitiveType.Sphere, Head, new Vector3(0f, 0.27f, 0.29f), new Vector3(0.11f, 0.1f, 0.1f), PaletteColor.Skin);
            Shape(PrimitiveType.Sphere, Head, new Vector3(-0.31f, 0.3f, 0f), new Vector3(0.08f, 0.13f, 0.08f), PaletteColor.Skin);
            Shape(PrimitiveType.Sphere, Head, new Vector3(0.31f, 0.3f, 0f), new Vector3(0.08f, 0.13f, 0.08f), PaletteColor.Skin);

            // Messy hair: a cap plus a few tufts sticking up.
            Shape(PrimitiveType.Sphere, Head, new Vector3(0f, 0.47f, -0.04f), new Vector3(0.64f, 0.34f, 0.6f), PaletteColor.Hair);
            Tuft(new Vector3(-0.1f, 0.63f, 0.08f), new Vector3(-20f, 0f, 25f));
            Tuft(new Vector3(0.08f, 0.65f, 0.02f), new Vector3(10f, 0f, -20f));
            Tuft(new Vector3(0f, 0.62f, -0.15f), new Vector3(-35f, 0f, 5f));

            Eyes = Pivot("Eyes", Head, Vector3.zero).gameObject;
            Eye(-0.12f);
            Eye(0.12f);
            DizzyEyes = Pivot("DizzyEyes", Head, Vector3.zero).gameObject;
            Material spiral = SpiralMaterial();
            Spiral(-0.12f, spiral);
            Spiral(0.12f, spiral);
            DizzyEyes.SetActive(false);

            Mouth = Pivot("Mouth", Head, new Vector3(0f, 0.15f, 0.265f));
            Shape(PrimitiveType.Sphere, Mouth, Vector3.zero, new Vector3(0.16f, 0.1f, 0.06f), PaletteColor.Black);
        }

        private void Tuft(Vector3 position, Vector3 euler)
        {
            Transform tuft = Shape(PrimitiveType.Capsule, Head, position, new Vector3(0.09f, 0.1f, 0.09f), PaletteColor.Hair);
            tuft.localRotation = Quaternion.Euler(euler);
        }

        private void Eye(float x)
        {
            Transform eyes = Eyes.transform;
            Shape(PrimitiveType.Sphere, eyes, new Vector3(x, 0.38f, 0.25f), new Vector3(0.17f, 0.2f, 0.08f), PaletteColor.Ceramic);
            Shape(PrimitiveType.Sphere, eyes, new Vector3(x * 0.9f, 0.37f, 0.29f), new Vector3(0.07f, 0.09f, 0.03f), PaletteColor.Black);
        }

        private void Spiral(float x, Material material)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(quad.GetComponent<Collider>());
            quad.name = "Spiral";
            quad.transform.SetParent(DizzyEyes.transform, false);
            quad.transform.localPosition = new Vector3(x, 0.38f, 0.285f);
            quad.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            quad.transform.localScale = Vector3.one * 0.2f;
            quad.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        private Transform Shape(PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, PaletteColor color)
        {
            GameObject shape = GameObject.CreatePrimitive(type);
            Object.DestroyImmediate(shape.GetComponent<Collider>());
            shape.name = type.ToString();
            shape.transform.SetParent(parent, false);
            shape.transform.localPosition = position;
            shape.transform.localScale = scale;
            shape.GetComponent<MeshRenderer>().sharedMaterial = _materials.Get(color);
            return shape.transform;
        }

        private static Transform Pivot(string name, Transform parent, Vector3 position)
        {
            Transform pivot = new GameObject(name).transform;
            pivot.SetParent(parent, false);
            pivot.localPosition = position;
            return pivot;
        }

        private static Material SpiralMaterial()
        {
            Texture2D texture = EditorAssetUtility.SaveOrReplace(SpiralTexture(), AssetPaths.SpiralTexture);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetPaths.SpiralMaterial);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                AssetDatabase.CreateAsset(material, AssetPaths.SpiralMaterial);
            }

            material.SetTexture(BaseMap, texture);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_AlphaClip", 1f);
            material.SetFloat("_Cutoff", 0.5f);
            material.EnableKeyword("_ALPHATEST_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            EditorUtility.SetDirty(material);
            return material;
        }

        /// <summary>White disc with a black Archimedean spiral, transparent outside.</summary>
        private static Texture2D SpiralTexture()
        {
            const int size = 128;
            const float turns = 3f;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "DizzySpiral" };
            Color32[] pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x + 0.5f) / size * 2f - 1f;
                    float v = (y + 0.5f) / size * 2f - 1f;
                    float r = Mathf.Sqrt(u * u + v * v);
                    float angle = (Mathf.Atan2(v, u) / (2f * Mathf.PI) + 1f) % 1f;
                    float band = Mathf.Repeat(r * turns - angle, 1f);
                    bool ink = band < 0.38f || r < 0.08f;
                    byte alpha = r <= 1f ? (byte)255 : (byte)0;
                    pixels[y * size + x] = ink ? new Color32(25, 20, 30, alpha) : new Color32(255, 255, 255, alpha);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }
    }
}
