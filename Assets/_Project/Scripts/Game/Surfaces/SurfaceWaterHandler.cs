using System;
using System.Collections.Generic;
using FB.Game.Fire;
using FB.Game.Interactions;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Surfaces
{
    /// <summary>
    /// Water hits become surface state: the hit surface gets wet and runs the water rules; burning surfaces
    /// near the impact are cooled too, so small targets like the pan can be put out without pixel aiming.
    /// </summary>
    public sealed class SurfaceWaterHandler : IInitializable, IDisposable
    {
        private const float SplashShare = 0.6f;

        private readonly SignalBus _signalBus;
        private readonly SurfaceRegistry _registry;
        private readonly ISurfaceActions _actions;
        private readonly InteractionRuleRunner _rules;
        private readonly FireConfig _config;

        public SurfaceWaterHandler(SignalBus signalBus, SurfaceRegistry registry, ISurfaceActions actions,
            InteractionRuleRunner rules, FireConfig config)
        {
            _signalBus = signalBus;
            _registry = registry;
            _actions = actions;
            _rules = rules;
            _config = config;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<WaterHitSignal>(OnWaterHit);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<WaterHitSignal>(OnWaterHit);
        }

        private void OnWaterHit(WaterHitSignal hit)
        {
            Surface direct = null;
            if (hit.Target != null && _registry.TryGet(hit.Target, out direct))
            {
                Soak(direct, hit.Amount, hit);
            }

            float radiusSqr = _config.WaterSplashRadius * _config.WaterSplashRadius;
            IReadOnlyList<Surface> surfaces = _registry.All;
            for (int i = 0; i < surfaces.Count; i++)
            {
                Surface surface = surfaces[i];
                if (surface != direct && surface.IsBurning && surface.Bounds.SqrDistance(hit.Point) <= radiusSqr)
                {
                    Soak(surface, hit.Amount * SplashShare, hit);
                }
            }
        }

        private void Soak(Surface surface, float amount, in WaterHitSignal hit)
        {
            _actions.AddWater(surface, amount, hit.Cause);
            _rules.Run(new InteractionEvent(InteractionTrigger.Water, surface, null, amount, hit.Cause));
        }
    }
}
