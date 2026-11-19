using FB.Game.Config;
using UnityEngine;
using Zenject;

namespace FB.Game.Hose
{
    /// <summary>
    /// Visual-only verlet rope from the tap to the back of the nozzle: gravity, distance constraints,
    /// floor clamp and capsule push-out of every segment from level geometry every few iterations.
    /// </summary>
    public sealed class HoseRope : IInitializable, ILateTickable
    {
        private const float FloorY = 0f;
        private const float FloorFriction = 0.6f;
        private const int CollideEveryIterations = 2;

        private readonly HoseView _view;
        private readonly HoseConfig _config;
        private readonly Transform _anchor;
        private HoseRopeCollision _collision;
        private Vector3[] _points;
        private Vector3[] _previous;
        private float _segmentLength;

        public HoseRope(HoseView view, HoseConfig config, [Inject(Id = HoseIds.Anchor)] Transform anchor)
        {
            _view = view;
            _config = config;
            _anchor = anchor;
        }

        // Falls back to the tip on prefabs generated before the attach point existed.
        private Transform HoseEnd => _view.HoseAttach != null ? _view.HoseAttach : _view.NozzleTip;

        public void Initialize()
        {
            int count = _config.RopePoints;
            _points = new Vector3[count];
            _previous = new Vector3[count];
            _segmentLength = _config.MaxLength * _config.RopeSlack / (count - 1);
            if (_view.RopeProbe != null)
            {
                _collision = new HoseRopeCollision(_view.RopeProbe, _config.RopeWidth, _segmentLength);
            }
            else
            {
                Debug.LogError("[Hose] Rope probe is missing on the player prefab: run Tools/Flame&Blame/Build All. " +
                               "The hose works without collisions until then.");
            }

            HoseRopeLayout.Coil(_points, _anchor.position, HoseEnd.position, _segmentLength,
                FloorY + _config.RopeWidth * 0.5f);
            System.Array.Copy(_points, _previous, count);

            LineRenderer line = _view.Line;
            line.useWorldSpace = true;
            line.positionCount = count;
            line.widthMultiplier = _config.RopeWidth;
        }

        public void LateTick()
        {
            float deltaTime = Mathf.Min(Time.deltaTime, 1f / 30f);
            Integrate(deltaTime);
            int iterations = _config.RopeIterations;
            for (int i = 0; i < iterations; i++)
            {
                SolveConstraints();
                if (i % CollideEveryIterations == CollideEveryIterations - 1 || i == iterations - 1)
                {
                    Collide();
                }
            }

            _view.Line.SetPositions(_points);
        }

        private void Integrate(float deltaTime)
        {
            float floor = FloorY + _config.RopeWidth * 0.5f;
            Vector3 gravity = Physics.gravity * (deltaTime * deltaTime);
            for (int i = 1; i < _points.Length - 1; i++)
            {
                Vector3 velocity = (_points[i] - _previous[i]) * _config.RopeDamping;
                if (_points[i].y <= floor + 0.001f)
                {
                    velocity.x *= FloorFriction;
                    velocity.z *= FloorFriction;
                }

                _previous[i] = _points[i];
                _points[i] += velocity + gravity;
            }
        }

        private void Collide()
        {
            if (_collision == null)
            {
                return;
            }

            int last = _points.Length - 1;
            for (int i = 0; i < last; i++)
            {
                _collision.ResolveSegment(ref _points[i], ref _points[i + 1], i == 0, i + 1 == last);
            }
        }

        private void SolveConstraints()
        {
            int last = _points.Length - 1;
            _points[0] = _anchor.position;
            _points[last] = HoseEnd.position;

            for (int i = 0; i < last; i++)
            {
                Vector3 delta = _points[i + 1] - _points[i];
                float distance = delta.magnitude;
                if (distance < 1e-5f)
                {
                    continue;
                }

                Vector3 correction = delta * ((distance - _segmentLength) / distance);
                bool pinnedA = i == 0;
                bool pinnedB = i + 1 == last;
                if (!pinnedA && !pinnedB)
                {
                    _points[i] += correction * 0.5f;
                    _points[i + 1] -= correction * 0.5f;
                }
                else if (!pinnedA)
                {
                    _points[i] += correction;
                }
                else if (!pinnedB)
                {
                    _points[i + 1] -= correction;
                }
            }

            float floor = FloorY + _config.RopeWidth * 0.5f;
            for (int i = 1; i < last; i++)
            {
                if (_points[i].y < floor)
                {
                    _points[i].y = floor;
                }
            }
        }
    }
}
