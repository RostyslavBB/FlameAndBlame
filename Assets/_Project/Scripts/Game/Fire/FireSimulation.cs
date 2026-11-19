using System.Collections.Generic;
using FB.Game.Interactions;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Fire
{
    /// <summary>
    /// Central fire tick at a fixed rate (10 Hz by default) for all surfaces: burning consumes fuel and heats
    /// neighbours through the rule pipeline, cold surfaces cool down, wet surfaces dry.
    /// </summary>
    public sealed class FireSimulation : IFireStatus, ITickable
    {
        private const int MaxStepsPerFrame = 3;

        private readonly SurfaceRegistry _registry;
        private readonly ISurfaceActions _actions;
        private readonly InteractionRuleRunner _rules;
        private readonly FireConfig _config;
        private float _accumulator;

        public FireSimulation(SurfaceRegistry registry, ISurfaceActions actions, InteractionRuleRunner rules,
            FireConfig config)
        {
            _registry = registry;
            _actions = actions;
            _rules = rules;
            _config = config;
        }

        public bool HasStarted { get; private set; }
        public int BurningCount { get; private set; }

        public void Tick()
        {
            float step = 1f / _config.TickRate;
            _accumulator = Mathf.Min(_accumulator + Time.deltaTime, step * MaxStepsPerFrame);
            while (_accumulator >= step)
            {
                _accumulator -= step;
                Step(step);
            }
        }

        private void Step(float deltaTime)
        {
            IReadOnlyList<Surface> surfaces = _registry.All;
            int burning = 0;
            for (int i = 0; i < surfaces.Count; i++)
            {
                Surface surface = surfaces[i];
                if (surface.IsBurning)
                {
                    burning++;
                    Burn(surface, deltaTime);
                }
                else if (surface.Heat > 0f)
                {
                    _actions.AddHeat(surface, -_config.HeatDecayPerSecond * deltaTime);
                }

                Dry(surface, deltaTime);
            }

            BurningCount = burning;
            HasStarted |= burning > 0;
        }

        private void Burn(Surface surface, float deltaTime)
        {
            surface.Fuel -= deltaTime;
            float maxHeat = surface.Profile.IgnitionHeat * _config.MaxHeatFactor;
            if (surface.Heat < maxHeat)
            {
                _actions.AddHeat(surface, Mathf.Min(_config.BurningRegrowPerSecond * deltaTime, maxHeat - surface.Heat));
            }

            float heat = _config.HeatPerSecond * deltaTime;
            IReadOnlyList<SurfaceNeighbor> neighbors = surface.Neighbors;
            for (int n = 0; n < neighbors.Count; n++)
            {
                SurfaceNeighbor neighbor = neighbors[n];
                _rules.Run(new InteractionEvent(InteractionTrigger.NeighborHeat, neighbor.Surface, surface,
                    heat * neighbor.Falloff, null));
            }

            if (surface.Fuel <= 0f)
            {
                _actions.Ruin(surface, RuinReason.Burned, surface.Statuses.CauseOf(SurfaceStatus.Burning));
            }
        }

        private void Dry(Surface surface, float deltaTime)
        {
            if (surface.Water <= 0f)
            {
                return;
            }

            surface.Water = Mathf.Max(0f, surface.Water - _config.DryPerSecond * deltaTime);
            if (surface.Water < _config.WetThreshold)
            {
                _actions.Dry(surface);
            }
        }
    }
}
