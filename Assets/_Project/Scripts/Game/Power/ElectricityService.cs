using System.Collections.Generic;
using FB.Game.Blame;
using FB.Game.Puddles;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Power
{
    /// <summary>
    /// While power is on, puddles touching a live object (the power strip, or electronics killed by a short
    /// circuit) become electrified, and so does every puddle connected to them (BFS over overlapping discs).
    /// </summary>
    public sealed class ElectricityService : IInitializable, ITickable
    {
        private readonly IPuddleField _puddles;
        private readonly IPowerGrid _grid;
        private readonly SurfaceRegistry _surfaces;
        private readonly PowerConfig _config;
        private readonly Queue<Puddle> _frontier = new Queue<Puddle>(32);
        private Surface _powerSource;
        private float _accumulator;

        public ElectricityService(IPuddleField puddles, IPowerGrid grid, SurfaceRegistry surfaces, PowerConfig config)
        {
            _puddles = puddles;
            _grid = grid;
            _surfaces = surfaces;
            _config = config;
        }

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
            Recalculate();
        }

        private void Recalculate()
        {
            IReadOnlyList<Puddle> puddles = _puddles.Active;
            for (int i = 0; i < puddles.Count; i++)
            {
                puddles[i].IsElectrified = false;
            }

            if (!_grid.IsOn)
            {
                return;
            }

            if (_powerSource != null)
            {
                Seed(_powerSource.Bounds, _grid.WiringCause, puddles);
            }

            IReadOnlyList<Surface> all = _surfaces.All;
            for (int i = 0; i < all.Count; i++)
            {
                Cause ruin = all[i].Statuses.CauseOf(SurfaceStatus.Ruined);
                if (all[i].IsRuined && ruin != null && ruin.Action == CauseAction.ShortCircuit)
                {
                    Seed(all[i].Bounds, ruin, puddles);
                }
            }

            Spread(puddles);
        }

        private void Seed(Bounds live, Cause cause, IReadOnlyList<Puddle> puddles)
        {
            for (int i = 0; i < puddles.Count; i++)
            {
                Puddle puddle = puddles[i];
                if (!puddle.IsElectrified && puddle.Touches(live, _config.ContactMargin))
                {
                    Electrify(puddle, cause);
                }
            }
        }

        private void Spread(IReadOnlyList<Puddle> puddles)
        {
            while (_frontier.Count > 0)
            {
                Puddle current = _frontier.Dequeue();
                for (int i = 0; i < puddles.Count; i++)
                {
                    Puddle next = puddles[i];
                    if (!next.IsElectrified && current.Overlaps(next))
                    {
                        Electrify(next, current.PowerCause);
                    }
                }
            }
        }

        private void Electrify(Puddle puddle, Cause cause)
        {
            puddle.IsElectrified = true;
            puddle.PowerCause = cause;
            _frontier.Enqueue(puddle);
        }
    }
}
