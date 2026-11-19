using System.Collections.Generic;
using FB.Game.Config;
using FB.Game.Environment;
using FB.Game.Level;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>
    /// Life in the apartment: warm ceiling lights (fed by the breaker), glowing shades and screens,
    /// a ticking wall clock and dust drifting in the air.
    /// </summary>
    internal sealed class EnvironmentBuilder
    {
        private static readonly string[] LitLamps =
            { "CeilingLampHall", "CeilingLampKitchen", "CeilingLampLiving", "CeilingLampBath", "CeilingLampBedroom" };

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private readonly MaterialLibrary _materials;
        private readonly MeshLibrary _meshes;

        public EnvironmentBuilder(MaterialLibrary materials, MeshLibrary meshes)
        {
            _materials = materials;
            _meshes = meshes;
        }

        public RoomLightsView BuildLights(Transform level, Transform props)
        {
            Transform root = new GameObject("RoomLights").transform;
            root.SetParent(level, false);
            List<Light> lights = new List<Light>();
            foreach (string id in LitLamps)
            {
                Transform lamp = props.Find(id);
                if (lamp == null)
                {
                    continue;
                }

                Light light = new GameObject(id + "_Light").AddComponent<Light>();
                light.transform.SetParent(root, false);
                light.transform.position = lamp.position + Vector3.down * 0.1f;
                light.type = LightType.Point;
                light.color = new Color(1f, 0.92f, 0.8f);
                light.range = 5.5f;
                light.intensity = 1.2f;
                light.shadows = LightShadows.None;
                lights.Add(light);
            }

            List<MeshRenderer> renderers = new List<MeshRenderer>();
            List<int> slots = new List<int>();
            List<Color> colors = new List<Color>();
            Material glow = _materials.Get(PaletteColor.LampGlow);
            Material screen = _materials.Get(PaletteColor.TvScreen);
            foreach (MeshRenderer renderer in props.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] shared = renderer.sharedMaterials;
                for (int i = 0; i < shared.Length; i++)
                {
                    if (shared[i] == glow || shared[i] == screen)
                    {
                        renderers.Add(renderer);
                        slots.Add(i);
                        colors.Add(shared[i].GetColor(BaseColorId));
                    }
                }
            }

            RoomLightsView view = root.gameObject.AddComponent<RoomLightsView>();
            EditorAssetUtility.SetArray(view, "_lights", lights.ToArray());
            EditorAssetUtility.SetArray(view, "_glowRenderers", renderers.ToArray());
            UnityEditor.SerializedObject serialized = new UnityEditor.SerializedObject(view);
            UnityEditor.SerializedProperty slotProperty = serialized.FindProperty("_glowSlots");
            UnityEditor.SerializedProperty colorProperty = serialized.FindProperty("_glowColors");
            slotProperty.arraySize = slots.Count;
            colorProperty.arraySize = colors.Count;
            for (int i = 0; i < slots.Count; i++)
            {
                slotProperty.GetArrayElementAtIndex(i).intValue = slots[i];
                colorProperty.GetArrayElementAtIndex(i).colorValue = colors[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        /// <summary>Kitchen wall clock facing +X; hand pivots sit at the center so they rotate correctly.</summary>
        public ClockView BuildClock(Transform level)
        {
            Transform clock = new GameObject("WallClock").transform;
            clock.SetParent(level, false);
            clock.SetPositionAndRotation(new Vector3(-5.9f, 2.05f, 1f), Quaternion.LookRotation(Vector3.right));

            Part(clock, PartShape.Cylinder, new Vector3(0f, 0f, 0.005f), new Vector3(90f, 0f, 0f),
                new Vector3(0.44f, 0.03f, 0.44f), PaletteColor.Black);
            Part(clock, PartShape.Cylinder, new Vector3(0f, 0f, 0.015f), new Vector3(90f, 0f, 0f),
                new Vector3(0.38f, 0.02f, 0.38f), PaletteColor.Ceramic);
            Transform hour = Hand(clock, "HourHand", 0.1f, 0.018f, 0.03f);
            Transform minute = Hand(clock, "MinuteHand", 0.16f, 0.012f, 0.036f);
            Part(clock, PartShape.Sphere, new Vector3(0f, 0f, 0.04f), Vector3.zero, new Vector3(0.025f, 0.025f, 0.02f),
                PaletteColor.Accent);

            ClockView view = clock.gameObject.AddComponent<ClockView>();
            EditorAssetUtility.SetReference(view, "_hourHand", hour);
            EditorAssetUtility.SetReference(view, "_minuteHand", minute);
            return view;
        }

        public void BuildDust(Transform level, IReadOnlyList<RoomDefinition> rooms, Material material)
        {
            foreach (RoomDefinition room in rooms)
            {
                Vector2 size = room.Max - room.Min;
                Vector2 center = (room.Min + room.Max) * 0.5f;
                ParticleSystem dust = WaterParticleFactory.CreateSystem("Dust_" + room.Name, level, material);
                dust.transform.position = new Vector3(center.x, 1.3f, center.y);
                ParticleSystem.MainModule main = dust.main;
                main.loop = true;
                main.prewarm = true;
                main.startLifetime = new ParticleSystem.MinMaxCurve(8f, 12f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.01f, 0.05f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.008f, 0.02f);
                main.startColor = new Color(1f, 0.92f, 0.75f, 0.7f);
                main.maxParticles = 40;

                ParticleSystem.EmissionModule emission = dust.emission;
                emission.rateOverTime = size.x * size.y * 0.25f;

                ParticleSystem.ShapeModule shape = dust.shape;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(size.x * 0.85f, 2.2f, size.y * 0.85f);

                ParticleSystem.NoiseModule noise = dust.noise;
                noise.enabled = true;
                noise.strength = 0.05f;
                noise.frequency = 0.3f;
                noise.quality = ParticleSystemNoiseQuality.Low;
                dust.GetComponent<ParticleSystemRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            }
        }

        private Transform Hand(Transform clock, string name, float length, float width, float z)
        {
            Transform pivot = new GameObject(name).transform;
            pivot.SetParent(clock, false);
            pivot.localPosition = new Vector3(0f, 0f, z);
            Part(pivot, PartShape.Box, new Vector3(0f, length * 0.5f, 0f), Vector3.zero, new Vector3(width, length, 0.006f),
                PaletteColor.Black);
            return pivot;
        }

        private void Part(Transform parent, PartShape shape, Vector3 position, Vector3 euler, Vector3 scale,
            PaletteColor color)
        {
            GameObject go = new GameObject(shape.ToString());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localRotation = Quaternion.Euler(euler);
            go.transform.localScale = scale;
            go.AddComponent<MeshFilter>().sharedMesh = _meshes.Get(shape);
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = _materials.Get(color);
            renderer.shadowCastingMode = ShadowCastingMode.Off;
        }
    }
}
