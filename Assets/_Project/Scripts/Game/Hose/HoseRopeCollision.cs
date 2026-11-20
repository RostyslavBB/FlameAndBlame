using FB.Game.Common;
using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>
    /// Pushes rope segments out of level geometry. Each segment is tested as a capsule (not just its end points),
    /// so the hose cannot cut through wall corners and door jambs. Uses Physics.ComputePenetration with a
    /// probe capsule, which also handles segments that already ended up inside a collider.
    /// Broad phase once per frame: one box query around the whole rope caches the nearby colliders and their
    /// bounds; segments then only test colliders whose bounds they touch (instead of one physics query per
    /// segment per pass). Colliders lying flat under the rope (floors) are skipped: the rope has its own floor.
    /// No allocations.
    /// </summary>
    public sealed class HoseRopeCollision
    {
        private const int MaxCandidates = 256;
        private const float Margin = 0.01f;
        private const float FrameMargin = 0.35f;
        private const float FloorTop = 0.02f;

        private readonly CapsuleCollider _probe;
        private readonly Collider[] _candidates = new Collider[MaxCandidates];
        private readonly Bounds[] _candidateBounds = new Bounds[MaxCandidates];
        private readonly float _radius;
        private int _candidateCount;

        public HoseRopeCollision(CapsuleCollider probe, float ropeWidth, float segmentLength)
        {
            _probe = probe;
            _radius = ropeWidth * 0.5f + Margin;
            _probe.direction = 2;
            _probe.radius = _radius;
            _probe.height = segmentLength + _radius * 2f;
        }

        /// <summary>Collects the colliders near the rope once per frame (points move only a little per frame).</summary>
        public void BeginFrame(Vector3[] points)
        {
            Bounds area = new Bounds(points[0], Vector3.zero);
            for (int i = 1; i < points.Length; i++)
            {
                area.Encapsulate(points[i]);
            }

            area.Expand(FrameMargin * 2f + _radius * 2f);
            int found = Physics.OverlapBoxNonAlloc(area.center, area.extents, _candidates, Quaternion.identity,
                GameLayers.EnvironmentMask, QueryTriggerInteraction.Ignore);
            _candidateCount = 0;
            for (int i = 0; i < found; i++)
            {
                Bounds bounds = _candidates[i].bounds;
                if (bounds.max.y <= FloorTop)
                {
                    continue;
                }

                _candidates[_candidateCount] = _candidates[i];
                _candidateBounds[_candidateCount] = bounds;
                _candidateCount++;
            }
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

            Vector3 min = Vector3.Min(a, b) - Vector3.one * _radius;
            Vector3 max = Vector3.Max(a, b) + Vector3.one * _radius;
            Quaternion rotation = Quaternion.LookRotation(axis);
            for (int i = 0; i < _candidateCount; i++)
            {
                Bounds bounds = _candidateBounds[i];
                if (max.x < bounds.min.x || min.x > bounds.max.x || max.y < bounds.min.y || min.y > bounds.max.y ||
                    max.z < bounds.min.z || min.z > bounds.max.z)
                {
                    continue;
                }

                Collider other = _candidates[i];
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
