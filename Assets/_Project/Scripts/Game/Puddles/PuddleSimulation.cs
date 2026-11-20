using System;
using Unity.Collections;
using FB.Game.Blame;
using FB.Game.Common;
using FB.Game.Level;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Puddles
{
    /// <summary>
    /// Runs the floor water grid: hose water lands where it hits (or drips below), flows, merges into puddles,
    /// stops at walls and furniture, evaporates. Uploads depth/electricity to a texture and feeds impact ripples.
    /// </summary>
    public sealed class PuddleSimulation : IPuddles, IInitializable, IDisposable, ITickable
    {
        private const int MaxStepsPerFrame = 3;
        private const float FloorNormalY = 0.7f;
        private const float FloorHitHeight = 0.2f;

        private readonly SignalBus _signalBus;
        private readonly ICauseFactory _causes;
        private readonly PuddleConfig _config;
        private readonly PuddleSurfaceView _view;
        private readonly Collider[] _overlaps = new Collider[8];
        private readonly Vector4[] _ripples;
        private WaterGrid _grid;
        private Texture2D _texture;
        private Cause _lastWater;
        private Cause _puddleCause;
        private float _accumulator;
        private float _nextRippleTime;
        private int _nextRipple;

        public PuddleSimulation(SignalBus signalBus, ICauseFactory causes, PuddleConfig config, LevelCatalog level)
        {
            _signalBus = signalBus;
            _causes = causes;
            _config = config;
            _view = level.Puddles;
            _ripples = new Vector4[8];
        }

        public WaterGrid Grid => _grid;

        public void Initialize()
        {
            int width = Mathf.CeilToInt(_view.Size.x / _config.CellSize);
            int height = Mathf.CeilToInt(_view.Size.y / _config.CellSize);
            _grid = new WaterGrid(_view.Origin, width, height, _config.CellSize);
            MarkObstacles();

            // Half-float: R = water depth in meters (the surface is displaced by it), G = electrified.
            _texture = new Texture2D(width, height, TextureFormat.RGHalf, false, true)
            {
                name = "PuddleWater",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            Upload(new RectInt(0, 0, width, height));
            _view.Bind(_texture);
            _signalBus.Subscribe<WaterHitSignal>(OnWaterHit);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<WaterHitSignal>(OnWaterHit);
            UnityEngine.Object.Destroy(_texture);
        }

        public float DepthAt(Vector3 world) => _grid.DepthAt(world);

        public bool IsWet(Vector3 world) => _grid.DepthAt(world) > _config.FilmThickness * 0.5f;

        public Cause CauseAt(Vector3 world) => _grid.TryGetCell(world, out int index) ? _grid.Causes[index] : null;

        public void Tick()
        {
            float step = 1f / _config.SimulationRate;
            _accumulator = Mathf.Min(_accumulator + Time.deltaTime, step * MaxStepsPerFrame);
            bool stepped = false;
            RectInt changed = default;
            while (_accumulator >= step)
            {
                _accumulator -= step;
                if (_grid.Step(_config.FlowRate, _config.FilmThickness, _config.EvaporationPerSecond * step, out RectInt touched))
                {
                    changed = stepped ? Union(changed, touched) : touched;
                    stepped = true;
                }
            }

            if (stepped)
            {
                Upload(changed);
            }

            _view.SetRipples(_ripples);
        }

        private void OnWaterHit(WaterHitSignal hit)
        {
            if (hit.Cause == null)
            {
                return;
            }

            bool floorHit = hit.Normal.y >= FloorNormalY && hit.Point.y <= FloorHitHeight;
            float volume = hit.Amount * _config.VolumePerWater * (floorHit ? 1f : _config.NonFloorShare);
            _grid.AddVolume(hit.Point, volume, PuddleCause(hit.Cause), _config.MaxDepth);

            if (floorHit && Time.time >= _nextRippleTime)
            {
                _nextRippleTime = Time.time + _config.RippleInterval;
                _ripples[_nextRipple] = new Vector4(hit.Point.x, hit.Point.z, Time.time, Mathf.Clamp01(hit.Amount));
                _nextRipple = (_nextRipple + 1) % _config.MaxRipples;
            }
        }

        private Cause PuddleCause(Cause water)
        {
            if (_lastWater != water)
            {
                _lastWater = water;
                _puddleCause = _causes.Create(water.Instigator, CauseAction.Puddle, water);
            }

            return _puddleCause;
        }

        // Cells covered by static colliders a few centimeters above the floor (walls, furniture bases) hold no water.
        private void MarkObstacles()
        {
            Vector3 halfExtents = new Vector3(_grid.CellSize * 0.45f, 0.03f, _grid.CellSize * 0.45f);
            for (int i = 0; i < _grid.Count; i++)
            {
                Vector3 center = _grid.CellCenter(i) + Vector3.up * 0.04f;
                int count = Physics.OverlapBoxNonAlloc(center, halfExtents, _overlaps, Quaternion.identity,
                    GameLayers.EnvironmentMask, QueryTriggerInteraction.Ignore);
                for (int c = 0; c < count; c++)
                {
                    if (_overlaps[c].attachedRigidbody == null)
                    {
                        _grid.Blocked[i] = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Writes half-floats (R = depth, G = electrified) straight into the texture memory, only for the cells that
        /// may have changed: no Color[] conversion of the whole grid every step.
        /// </summary>
        private void Upload(RectInt area)
        {
            NativeArray<ushort> data = _texture.GetPixelData<ushort>(0);
            float[] depth = _grid.Depth;
            bool[] electrified = _grid.Electrified;
            ushort one = Mathf.FloatToHalf(1f);
            int width = _grid.Width;
            for (int z = area.yMin; z < area.yMax; z++)
            {
                for (int x = area.xMin; x < area.xMax; x++)
                {
                    int index = z * width + x;
                    data[index * 2] = Mathf.FloatToHalf(depth[index]);
                    data[index * 2 + 1] = electrified[index] ? one : (ushort)0;
                }
            }

            _texture.Apply(false, false);
        }

        private static RectInt Union(RectInt a, RectInt b)
        {
            int xMin = Mathf.Min(a.xMin, b.xMin);
            int yMin = Mathf.Min(a.yMin, b.yMin);
            return new RectInt(xMin, yMin, Mathf.Max(a.xMax, b.xMax) - xMin, Mathf.Max(a.yMax, b.yMax) - yMin);
        }
    }
}
