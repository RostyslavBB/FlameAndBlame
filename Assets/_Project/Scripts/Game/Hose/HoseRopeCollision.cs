using FB.Game.Common;
using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>
    /// Pushes rope segments out of level geometry. Each segment is tested as a capsule (not just its end points),
    /// so the hose cannot cut through wall corners and door jambs. Uses Physics.ComputePenetration with a
    /// probe capsule, which also handles segments that already ended up inside a collider. No allocations.
    /// </summary>
    public sealed class HoseRopeCollision
    {
        private const int MaxOverlaps = 8;
        private const float Margin = 0.01f;

        private readonly CapsuleCollider _probe;
        private readonly Collider[] _overlaps = new Collider[MaxOverlaps];
        private readonly float _radius;

        public HoseRopeCollision(CapsuleCollider probe, float ropeWidth, float segmentLength)
        {
            _probe = probe;
            _radius = ropeWidth * 0.5f + Margin;
            _probe.direction = 2;
            _probe.radius = _radius;
            _probe.height = segmentLength + _radius * 2f;
        }

        /// <summary>Resolves one segment; pinned ends are not moved.</summary>
        public void ResolveSegment(ref Vector3 a, ref Vector3 b, bool pinnedA, bool pinnedB)
        {
            if (pinnedA && pinnedB)
            {
                return;
            }

            Vector3 axis = b - a;
            if (axis.sqrMagnitude < 1e-8f)
            {
                return;
            }

            int count = Physics.OverlapCapsuleNonAlloc(a, b, _radius, _overlaps, GameLayers.EnvironmentMask,
                QueryTriggerInteraction.Ignore);
            Quaternion rotation = Quaternion.LookRotation(axis);
            for (int i = 0; i < count; i++)
            {
                Collider other = _overlaps[i];
                Transform otherTransform = other.transform;
                if (!Physics.ComputePenetration(_probe, (a + b) * 0.5f, rotation, other, otherTransform.position,
                        otherTransform.rotation, out Vector3 direction, out float distance))
                {
                    continue;
                }

                Vector3 push = direction * distance;
                if (!pinnedA)
                {
                    a += push;
                }

                if (!pinnedB)
                {
                    b += push;
                }
            }
        }
    }
}
