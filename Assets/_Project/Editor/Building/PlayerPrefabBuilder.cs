using FB.Game.Player;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>Player prefab: frictionless capsule rigidbody + camera pivot at eye height.</summary>
    internal static class PlayerPrefabBuilder
    {
        private const float Height = 1.8f;
        private const float Radius = 0.35f;
        private const float EyeHeight = 1.62f;

        public static void Build()
        {
            GameObject root = new GameObject("Player");

            Rigidbody body = root.AddComponent<Rigidbody>();
            body.mass = 70f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.constraints = RigidbodyConstraints.FreezeRotation;

            CapsuleCollider capsule = root.AddComponent<CapsuleCollider>();
            capsule.height = Height;
            capsule.radius = Radius;
            capsule.center = new Vector3(0f, Height * 0.5f, 0f);
            capsule.sharedMaterial = FrictionlessMaterial();

            Transform pivot = new GameObject("CameraPivot").transform;
            pivot.SetParent(root.transform, false);
            pivot.localPosition = new Vector3(0f, EyeHeight, 0f);

            GameObject cameraObject = new GameObject("Camera") { tag = "MainCamera" };
            cameraObject.transform.SetParent(pivot, false);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 75f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 60f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.62f, 0.82f, 0.95f);
            cameraObject.AddComponent<AudioListener>();

            PlayerBody player = root.AddComponent<PlayerBody>();
            EditorAssetUtility.SetReference(player, "_rigidbody", body);
            EditorAssetUtility.SetReference(player, "_collider", capsule);
            EditorAssetUtility.SetReference(player, "_cameraPivot", pivot);
            EditorAssetUtility.SetReference(player, "_camera", camera);

            EditorAssetUtility.EnsureFolder(AssetPaths.Prefabs);
            PrefabUtility.SaveAsPrefabAsset(root, AssetPaths.PlayerPrefab);
            Object.DestroyImmediate(root);
        }

        private static PhysicsMaterial FrictionlessMaterial()
        {
            PhysicsMaterial material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(AssetPaths.PlayerPhysicsMaterial);
            if (material == null)
            {
                material = new PhysicsMaterial("PM_Player");
                AssetDatabase.CreateAsset(material, AssetPaths.PlayerPhysicsMaterial);
            }

            material.staticFriction = 0f;
            material.dynamicFriction = 0f;
            material.bounciness = 0f;
            material.frictionCombine = PhysicsMaterialCombine.Minimum;
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
