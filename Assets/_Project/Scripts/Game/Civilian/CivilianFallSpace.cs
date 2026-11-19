using FB.Game.Common;
using UnityEngine;
using UnityEngine.AI;

namespace FB.Game.Civilian
{
    /// <summary>
    /// Makes sure the neighbour has room to lie down: he falls backwards and his body reaches ~1.6 m behind his
    /// heels, so before a fall he turns his back to the most open direction, and if even that is too short he
    /// steps forward along the nav mesh. Static geometry only (loose props get knocked aside anyway). No allocations.
    /// </summary>
    public sealed class CivilianFallSpace
    {
        private const int Directions = 12;
        private const float BodyLength = 1.75f;
        private const float ProbeRadius = 0.22f;
        private const float ProbeHeight = 0.35f;

        private readonly RaycastHit[] _hits = new RaycastHit[8];

        /// <summary>Turns the root so his back faces the most open direction, then makes room behind it.</summary>
        public void PrepareToFall(Transform root, NavMeshAgent agent)
        {
            Vector3 best = -root.forward;
            float bestSpace = SpaceTowards(root.position, best);
            for (int i = 0; i < Directions && bestSpace < BodyLength; i++)
            {
                float angle = i * (360f / Directions);
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                float space = SpaceTowards(root.position, direction);
                if (space > bestSpace)
                {
                    best = direction;
                    bestSpace = space;
                }
            }

            root.rotation = Quaternion.LookRotation(-best, Vector3.up);
            KeepClear(root, agent);
        }

        /// <summary>If something stands closer than his body length behind him, slides him forward by the deficit.</summary>
        public void KeepClear(Transform root, NavMeshAgent agent)
        {
            float deficit = BodyLength - SpaceTowards(root.position, -root.forward);
            if (deficit > 0.01f && agent.isOnNavMesh)
            {
                agent.Move(root.forward * deficit);
            }
        }

        private float SpaceTowards(Vector3 feet, Vector3 direction)
        {
            Vector3 origin = feet + Vector3.up * ProbeHeight;
            int count = Physics.SphereCastNonAlloc(origin, ProbeRadius, direction, _hits, BodyLength,
                GameLayers.EnvironmentMask, QueryTriggerInteraction.Ignore);
            float space = BodyLength;
            for (int i = 0; i < count; i++)
            {
                Rigidbody body = _hits[i].rigidbody;
                bool loose = body != null && !body.isKinematic;
                // distance 0 means the probe started inside a collider: treat it as no room at all.
                if (!loose && _hits[i].distance < space)
                {
                    space = _hits[i].distance;
                }
            }

            return space;
        }
    }
}
