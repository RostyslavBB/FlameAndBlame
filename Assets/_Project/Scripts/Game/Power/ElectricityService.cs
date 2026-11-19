using System.Collections.Generic;
using FB.Game.Blame;
using FB.Game.Puddles;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Power
{
    /// <summary>
    /// With the breaker on, every wet cell connected to the power strip (BFS over the water grid) is live:
    /// only the puddle that actually touches the strip, and everything it has merged with, carries current.
    /// A short-circuited appliance replaces the wiring as the blamed power source.
    /// </summary>
    public sealed class ElectricityService : IElectricity, IInitializable, ITickable
    {
        private const float ContactMargin = 0.12f;

        private readonly IPuddles _puddles;
        private readonly IPowerGrid _grid;
        private readonly SurfaceRegistry _surfaces;
        private readonly PowerConfig _config;
        private readonly Queue<int> _frontier = new Queue<int>(1024);
        private Surface _powerSource;
        private float _accumulator;

        public ElectricityService(IPuddles puddles, IPowerGrid grid, SurfaceRegistry surfaces, PowerConfig config)
        {
            _puddles = puddles;
            _grid = grid;
            _surfaces = surfaces;
            _config = config;
        }

        public bool IsLive { get; private set; }
        public Cause PowerCause { get; private set; }

        public bool IsElectrifiedAt(Vector3 world) =>
            IsLive && _puddles.Grid.TryGetCell(world, out int index) && _puddles.Grid.Electrified[index];

        public void Initialize()
        {
            _powerSource = _surfaces.FindById(_config.PowerSourceId);
            if (_powerSource == null)
            {
                Debug.LogError($"[Power] Power source '{_config.PowerSourceId}' not found in the level.");
            }
        }

        public void Tick()
        {
            _accumulator += Time.deltaTime;
            if (_accumulator < 1f / _config.TickRate)
            {
                return;
            }

            _accumulator = 0f;
            WaterGrid water = _puddles.Grid;
            System.Array.Clear(water.Electrified, 0, water.Electrified.Length);
            IsLive = false;
            PowerCause = null;
            if (!_grid.IsOn || _powerSource == null)
            {
                return;
            }

            SeedAround(water, _powerSource.Bounds);
            IsLive = Spread(water);
            PowerCause = IsLive ? ShortCircuitCause() ?? _grid.WiringCause : null;
        }

        private void SeedAround(WaterGrid water, Bounds source)
        {
            float wet = _config.WetDepth;
            for (float x = source.min.x - ContactMargin; x <= source.max.x + ContactMargin; x += water.CellSize)
            {
                for (float z = source.min.z - ContactMargin; z <= source.max.z + ContactMargin; z += water.CellSize)
                {
                    if (water.TryGetCell(new Vector3(x, 0f, z), out int index) && !water.Electrified[index] &&
                        water.Depth[index] > wet)
                    {
                        water.Electrified[index] = true;
                        _frontier.Enqueue(index);
                    }
                }
            }
        }

        private bool Spread(WaterGrid water)
        {
            bool any = _frontier.Count > 0;
            float wet = _config.WetDepth;
            while (_frontier.Count > 0)
            {
                int index = _frontier.Dequeue();
                int x = index % water.Width;
                Visit(water, x > 0 ? index - 1 : -1, wet);
                Visit(water, x < water.Width - 1 ? index + 1 : -1, wet);
                Visit(water, index - water.Width, wet);
                Visit(water, index + water.Width, wet);
            }

            return any;
        }

        private void Visit(WaterGrid water, int index, float wet)
        {
            if (index < 0 || index >= water.Count || water.Electrified[index] || water.Depth[index] <= wet)
            {
                return;
            }

            water.Electrified[index] = true;
            _frontier.Enqueue(index);
        }

        private Cause ShortCircuitCause()
        {
            IReadOnlyList<Surface> all = _surfaces.All;
            for (int i = 0; i < all.Count; i++)
            {
                Cause ruin = all[i].Statuses.CauseOf(SurfaceStatus.Ruined);
                if (ruin != null && ruin.Action == CauseAction.ShortCircuit)
                {
                    return ruin;
                }
            }

            return null;
        }
    }
}
