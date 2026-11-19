using System.Collections.Generic;
using FB.Game.Civilian;
using FB.Game.Common;
using FB.Game.Level;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace FB.Editor.Building
{
    /// <summary>
    /// Bakes the nav mesh of the level (loose dynamic props are ignored, so the neighbour runs through them
    /// and knocks them over), picks floor waypoints in every room and places the neighbour.
    /// </summary>
    internal static class CivilianBuilder
    {
        private const float AgentRadius = 0.25f;
        private const float AgentHeight = 1.5f;
        private const float MaxFloorHeight = 0.2f;

        public static CivilianView Build(Scene scene, MaterialLibrary materials, LevelLayoutConfig layout,
            GameObject level)
        {
            NavMeshData navMesh = BakeNavMesh(level);
            Vector3[] waypoints = PickWaypoints(navMesh, layout);

            GameObject root = new GameObject("Neighbour");
            SceneManager.MoveGameObjectToScene(root, scene);
            root.transform.SetPositionAndRotation(SpawnPoint(layout, waypoints), Quaternion.Euler(0f, 180f, 0f));
            CivilianModelBuilder model = new CivilianModelBuilder(materials);
            model.Build(root.transform);

            NavMeshAgent agent = root.AddComponent<NavMeshAgent>();
            agent.radius = AgentRadius;
            agent.height = AgentHeight;
            agent.angularSpeed = 720f;
            agent.acceleration = 14f;
            agent.stoppingDistance = 0.3f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            // Enabled by CivilianBrain after the nav mesh data is added, avoiding "not on NavMesh" warnings.
            agent.enabled = false;

            CapsuleCollider capsule = root.AddComponent<CapsuleCollider>();
            capsule.center = new Vector3(0f, 0.8f, 0f);
            capsule.height = 1.6f;
            capsule.radius = 0.3f;
            Rigidbody body = root.AddComponent<Rigidbody>();
            body.isKinematic = true;
            // Own layer: the hose rope, puddles and his own chaos query ignore him; water still hits him.
            LayerSetup.SetLayerRecursively(root, GameLayers.Civilian);

            CivilianView view = root.AddComponent<CivilianView>();
            EditorAssetUtility.SetReference(view, "_agent", agent);
            EditorAssetUtility.SetReference(view, "_collider", capsule);
            EditorAssetUtility.SetReference(view, "_body", model.Body);
            EditorAssetUtility.SetReference(view, "_head", model.Head);
            EditorAssetUtility.SetReference(view, "_armLeft", model.ArmLeft);
            EditorAssetUtility.SetReference(view, "_armRight", model.ArmRight);
            EditorAssetUtility.SetReference(view, "_legLeft", model.LegLeft);
            EditorAssetUtility.SetReference(view, "_legRight", model.LegRight);
            EditorAssetUtility.SetReference(view, "_mouth", model.Mouth);
            EditorAssetUtility.SetReference(view, "_eyes", model.Eyes);
            EditorAssetUtility.SetReference(view, "_dizzyEyes", model.DizzyEyes);
            EditorAssetUtility.SetReference(view, "_navMesh", navMesh);
            SetWaypoints(view, waypoints);
            return view;
        }

        private static NavMeshData BakeNavMesh(GameObject level)
        {
            List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
            foreach (Rigidbody body in level.GetComponentsInChildren<Rigidbody>())
            {
                if (!body.isKinematic)
                {
                    markups.Add(new NavMeshBuildMarkup { root = body.transform, ignoreFromBuild = true });
                }
            }

            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            NavMeshBuilder.CollectSources(level.transform, ~0, NavMeshCollectGeometry.PhysicsColliders, 0, markups,
                sources);

            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
            settings.agentRadius = AgentRadius;
            settings.agentHeight = AgentHeight;
            settings.agentClimb = 0.15f;
            settings.agentSlope = 30f;
            Bounds bounds = new Bounds(Vector3.zero, new Vector3(60f, 10f, 60f));
            NavMeshData data = NavMeshBuilder.BuildNavMeshData(settings, sources, bounds, Vector3.zero,
                Quaternion.identity);
            data.name = "GameNavMesh";

            AssetDatabase.DeleteAsset(AssetPaths.CivilianNavMesh);
            AssetDatabase.CreateAsset(data, AssetPaths.CivilianNavMesh);
            Debug.Log($"[Build] Nav mesh baked from {sources.Count} sources ({markups.Count} loose props ignored).");
            return data;
        }

        /// <summary>Room center plus four points toward the corners, kept only where the floor is walkable.</summary>
        private static Vector3[] PickWaypoints(NavMeshData navMesh, LevelLayoutConfig layout)
        {
            NavMeshDataInstance instance = NavMesh.AddNavMeshData(navMesh);
            List<Vector3> points = new List<Vector3>();
            foreach (RoomDefinition room in layout.Rooms)
            {
                Vector2 center = (room.Min + room.Max) * 0.5f;
                Vector2 half = (room.Max - room.Min) * 0.3f;
                TryAdd(points, center);
                TryAdd(points, center + new Vector2(half.x, half.y));
                TryAdd(points, center + new Vector2(-half.x, half.y));
                TryAdd(points, center + new Vector2(half.x, -half.y));
                TryAdd(points, center + new Vector2(-half.x, -half.y));
            }

            instance.Remove();
            Debug.Log($"[Build] Neighbour waypoints: {points.Count}.");
            return points.ToArray();
        }

        private static void TryAdd(List<Vector3> points, Vector2 xz)
        {
            Vector3 probe = new Vector3(xz.x, 0f, xz.y);
            if (NavMesh.SamplePosition(probe, out NavMeshHit hit, 1.2f, NavMesh.AllAreas) &&
                hit.position.y < MaxFloorHeight)
            {
                points.Add(hit.position);
            }
        }

        private static Vector3 SpawnPoint(LevelLayoutConfig layout, Vector3[] waypoints)
        {
            Vector3 best = waypoints.Length > 0 ? waypoints[0] : layout.SpawnPosition;
            float bestDistance = float.MaxValue;
            foreach (Vector3 point in waypoints)
            {
                // Start a few meters from the player, so he is on screen right away.
                float distance = Vector3.Distance(point, layout.SpawnPosition);
                if (distance > 2.5f && distance < bestDistance)
                {
                    best = point;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private static void SetWaypoints(CivilianView view, Vector3[] waypoints)
        {
            SerializedObject serialized = new SerializedObject(view);
            SerializedProperty property = serialized.FindProperty("_waypoints");
            property.arraySize = waypoints.Length;
            for (int i = 0; i < waypoints.Length; i++)
            {
                property.GetArrayElementAtIndex(i).vector3Value = waypoints[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
