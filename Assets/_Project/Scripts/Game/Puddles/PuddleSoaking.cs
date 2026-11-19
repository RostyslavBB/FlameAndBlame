using System.Collections.Generic;
using FB.Game.Blame;
using FB.Game.Interactions;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Puddles
{
    /// <summary>
    /// Objects standing in a puddle get water through the normal rule pipeline: the power strip short-circuits,
    /// the ficus drowns, furniture bottoms get wet and ignite slower. The puddle's cause is passed on.
    /// </summary>
    public sealed class PuddleSoaking : ITickable
    {
        private const float FloorContactHeight = 0.1f;
        private const float FootprintInset = 0.8f;

        private readonly IPuddles _puddles;
        private readonly SurfaceRegistry _surfaces;
        private readonly ISurfaceActions _actions;
        private readonly InteractionRuleRunner _rules;
        private readonly PuddleConfig _config;
        private float _accumulator;

        public PuddleSoaking(IPuddles puddles, SurfaceRegistry surfaces, ISurfaceActions actions,
            InteractionRuleRunner rules, PuddleConfig config)
        {
            _puddles = puddles;
            _surfaces = surfaces;
            _actions = actions;
            _rules = rules;
            _config = config;
        }

        public void Tick()
        {
            _accumulator += Time.deltaTime;
            if (_accumulator < _config.SoakInterval)
            {
                return;
            }

            float amount = _config.SoakPerSecond * _accumulator;
            _accumulator = 0f;
            IReadOnlyList<Surface> surfaces = _surfaces.All;
            for (int i = 0; i < surfaces.Count; i++)
            {
                Bounds bounds = surfaces[i].Bounds;
                if (bounds.min.y > FloorContactHeight)
                {
                    continue;
                }

                Cause cause = WetCause(bounds);
                if (cause == null)
                {
                    continue;
                }

                _actions.AddWater(surfaces[i], amount, cause);
                _rules.Run(new InteractionEvent(InteractionTrigger.Water, surfaces[i], null, amount, cause));
            }
        }

        // Center and the four (inset) corners of the footprint; furniture cells themselves are blocked,
        // so the corners catch water pooling around the base.
        private Cause WetCause(Bounds bounds)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents * FootprintInset;
            return CauseIfWet(new Vector3(c.x, 0f, c.z)) ??
                   CauseIfWet(new Vector3(c.x - e.x, 0f, c.z - e.z)) ??
                   CauseIfWet(new Vector3(c.x + e.x, 0f, c.z - e.z)) ??
                   CauseIfWet(new Vector3(c.x - e.x, 0f, c.z + e.z)) ??
                   CauseIfWet(new Vector3(c.x + e.x, 0f, c.z + e.z)) ??
                   CauseIfWet(new Vector3(c.x - bounds.extents.x - 0.1f, 0f, c.z)) ??
                   CauseIfWet(new Vector3(c.x + bounds.extents.x + 0.1f, 0f, c.z)) ??
                   CauseIfWet(new Vector3(c.x, 0f, c.z - bounds.extents.z - 0.1f)) ??
                   CauseIfWet(new Vector3(c.x, 0f, c.z + bounds.extents.z + 0.1f));
        }

        private Cause CauseIfWet(Vector3 point) => _puddles.IsWet(point) ? _puddles.CauseAt(point) : null;
    }
}
