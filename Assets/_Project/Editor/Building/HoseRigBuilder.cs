using FB.Game.Config;
using FB.Game.Hose;
using FB.Game.Level;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>Adds the first-person nozzle, the water jet and the hose line to the player prefab root.</summary>
    internal static class HoseRigBuilder
    {
        private static readonly Vector3 NozzleOffset = new Vector3(0.24f, -0.26f, 0.45f);
        private static readonly Vector3 AlongForward = new Vector3(90f, 0f, 0f);

        public static void Attach(GameObject player, Transform camera, MaterialLibrary materials, MeshLibrary meshes,
            Material waterMaterial)
        {
            Transform nozzle = new GameObject("Nozzle").transform;
            nozzle.SetParent(camera, false);
            nozzle.localPosition = NozzleOffset;

            AddPart(nozzle, meshes.Get(PartShape.Cylinder), materials.Get(PaletteColor.Hose),
                new Vector3(0f, 0f, -0.12f), AlongForward, new Vector3(0.08f, 0.18f, 0.08f));
            AddPart(nozzle, meshes.Get(PartShape.Cylinder), materials.Get(PaletteColor.Metal),
                new Vector3(0f, 0f, 0.05f), AlongForward, new Vector3(0.06f, 0.2f, 0.06f));
            AddPart(nozzle, meshes.Get(PartShape.Cone), materials.Get(PaletteColor.Metal),
                new Vector3(0f, 0f, 0.18f), AlongForward, new Vector3(0.075f, 0.08f, 0.075f));
            AddPart(nozzle, meshes.Get(PartShape.Box), materials.Get(PaletteColor.Accent),
                new Vector3(0f, 0.05f, -0.02f), Vector3.zero, new Vector3(0.02f, 0.05f, 0.1f));

            Transform tip = new GameObject("NozzleTip").transform;
            tip.SetParent(nozzle, false);
            tip.localPosition = new Vector3(0f, 0f, 0.23f);

            Transform attach = new GameObject("HoseAttach").transform;
            attach.SetParent(nozzle, false);
            attach.localPosition = new Vector3(0f, -0.02f, -0.22f);

            ParticleSystem jetParticles = WaterParticleFactory.CreateJet(tip, waterMaterial);
            WaterJet jet = jetParticles.gameObject.AddComponent<WaterJet>();
            EditorAssetUtility.SetReference(jet, "_particles", jetParticles);

            LineRenderer line = CreateLine(player.transform, materials.Get(PaletteColor.Hose));
            CapsuleCollider probe = line.gameObject.AddComponent<CapsuleCollider>();
            probe.isTrigger = true;
            probe.direction = 2;
            probe.radius = 0.06f;
            probe.height = 0.4f;

            HoseView view = player.AddComponent<HoseView>();
            EditorAssetUtility.SetReference(view, "_nozzleTip", tip);
            EditorAssetUtility.SetReference(view, "_hoseAttach", attach);
            EditorAssetUtility.SetReference(view, "_ropeProbe", probe);
            EditorAssetUtility.SetReference(view, "_jet", jet);
            EditorAssetUtility.SetReference(view, "_line", line);
        }

        private static LineRenderer CreateLine(Transform parent, Material material)
        {
            GameObject go = new GameObject("HoseLine");
            go.transform.SetParent(parent, false);
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.sharedMaterial = material;
            line.widthMultiplier = 0.1f;
            line.numCornerVertices = 2;
            line.numCapVertices = 2;
            line.generateLightingData = true;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.positionCount = 0;
            return line;
        }

        private static void AddPart(Transform parent, Mesh mesh, Material material, Vector3 position, Vector3 euler,
            Vector3 scale)
        {
            GameObject go = new GameObject(mesh.name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localRotation = Quaternion.Euler(euler);
            go.transform.localScale = scale;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
        }
    }
}
