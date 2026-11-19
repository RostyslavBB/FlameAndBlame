using System.Collections.Generic;
using FB.Game.Blame;
using FB.Game.Surfaces;
using UnityEngine;
using UnityEngine.AI;

namespace FB.Game.Civilian
{
    /// <summary>
    /// Shared context of the neighbour's states: navigation helpers, fire sensing and the flags the
    /// animator reads. Holds no decisions; the states decide.
    /// </summary>
    public sealed class CivilianBody
    {
        private const int FleeSamples = 4;

        private readonly CivilianView _view;
        private readonly SurfaceRegistry _registry;
        private readonly List<Vector3> _waypoints = new List<Vector3>();
        private readonly CivilianFallSpace _fallSpace = new CivilianFallSpace();
        private Vector3 _shove;

        public CivilianBody(CivilianView view, CivilianConfig config, SurfaceRegistry registry)
        {
            _view = view;
            Config = config;
            _registry = registry;
        }

        public CivilianConfig Config { get; }
        public CivilianView View => _view;
        public bool IsPanicking { get; set; }
        public bool IsKnocked { get; set; }
        public bool IsShocked { get; set; }
        public bool IsBurning => BurnCause != null;

        /// <summary>Why he is on fire (null when not burning); things he sets alight inherit it.</summary>
        public Cause BurnCause { get; set; }
        public Vector3 Position => _view.transform.position;
        public float Speed => Agent.enabled ? Agent.velocity.magnitude : 0f;

        public bool HasArrived =>
            !Agent.isOnNavMesh || (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.1f);

        private NavMeshAgent Agent => _view.Agent;

        public void Activate()
        {
            _waypoints.AddRange(_view.Waypoints);
            Agent.enabled = true;
            Agent.Warp(Position);
        }

        public void MoveTo(Vector3 point, float speed)
        {
            if (!Agent.isOnNavMesh)
            {
                return;
            }

            Agent.isStopped = false;
            Agent.speed = speed;
            Agent.SetDestination(point);
        }

        public void Stop()
        {
            if (!Agent.isOnNavMesh)
            {
                return;
            }

            Agent.isStopped = true;
            Agent.ResetPath();
            Agent.velocity = Vector3.zero;
        }

        /// <summary>Turns him so he falls backwards into open space (call when he is knocked down).</summary>
        public void PrepareToFall() => _fallSpace.PrepareToFall(_view.transform, Agent);

        /// <summary>Adds a horizontal shove (hose water); it slides him along the nav mesh and dies out.</summary>
        public void Shove(Vector3 velocity)
        {
            velocity.y = 0f;
            _shove = Vector3.ClampMagnitude(_shove + velocity, Config.MaxShoveSpeed);
        }

        public void TickShove(float deltaTime)
        {
            if (_shove.sqrMagnitude < 0.0025f || !Agent.isOnNavMesh)
            {
                _shove = Vector3.zero;
                return;
            }

            Agent.Move(_shove * deltaTime);
            if (IsKnocked)
            {
                // Lying down and washed along the floor: never let the body slide into a wall.
                _fallSpace.KeepClear(_view.transform, Agent);
            }

            _shove = Vector3.MoveTowards(_shove, Vector3.zero, Config.ShoveDamping * deltaTime);
        }

        public Vector3 RandomWaypoint() =>
            _waypoints.Count == 0 ? Position : _waypoints[Random.Range(0, _waypoints.Count)];

        /// <summary>The farthest of a few random waypoints: panicked, not smart.</summary>
        public Vector3 WaypointAwayFrom(Vector3 danger)
        {
            Vector3 best = RandomWaypoint();
            for (int i = 1; i < FleeSamples; i++)
            {
                Vector3 candidate = RandomWaypoint();
                if ((candidate - danger).sqrMagnitude > (best - danger).sqrMagnitude)
                {
                    best = candidate;
                }
            }

            return best;
        }

        public bool TryFindFire(float radius, out Vector3 fire)
        {
            fire = default;
            float bestSqr = radius * radius;
            bool found = false;
            IReadOnlyList<Surface> all = _registry.All;
            for (int i = 0; i < all.Count; i++)
            {
                if (!all[i].IsBurning)
                {
                    continue;
                }

                Vector3 center = all[i].Root.position;
                float sqr = (center - Position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    fire = center;
                    found = true;
                }
            }

            return found;
        }
    }
}
