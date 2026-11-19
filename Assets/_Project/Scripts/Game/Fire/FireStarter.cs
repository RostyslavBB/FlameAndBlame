using System;
using System.Collections.Generic;
using FB.Game.Blame;
using FB.Game.Flow;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Fire
{
    /// <summary>
    /// The old wiring starts fires all over the apartment: one outbreak per room, staggered in time, each in a
    /// randomly chosen spot, so every round plays differently. Every outbreak is its own root cause.
    /// </summary>
    public sealed class FireStarter : IFireOutbreaks, IInitializable, IDisposable, ITickable
    {
        private readonly SignalBus _signalBus;
        private readonly SurfaceRegistry _registry;
        private readonly ISurfaceActions _actions;
        private readonly ICauseFactory _causes;
        private readonly FireConfig _config;
        private readonly List<(Surface Target, float Time)> _pending = new List<(Surface, float)>();
        private float _elapsed;
        private bool _running;

        public FireStarter(SignalBus signalBus, SurfaceRegistry registry, ISurfaceActions actions,
            ICauseFactory causes, FireConfig config)
        {
            _signalBus = signalBus;
            _registry = registry;
            _actions = actions;
            _causes = causes;
            _config = config;
        }

        public bool HasPending => _pending.Count > 0;

        public void Initialize()
        {
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Tick()
        {
            if (!_running)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                if (_elapsed < _pending[i].Time)
                {
                    continue;
                }

                _actions.Ignite(_pending[i].Target, _causes.Create(ActorId.Building, CauseAction.Ignite));
                _pending.RemoveAt(i);
            }

            _running = _pending.Count > 0;
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            if (signal.Phase != GamePhase.Playing)
            {
                _running = false;
                return;
            }

            _pending.Clear();
            _elapsed = 0f;
            foreach (FireStartGroup group in _config.Outbreaks)
            {
                Surface target = Pick(group);
                if (target != null)
                {
                    _pending.Add((target, group.Delay));
                }
            }

            _running = true;
            Tick();
        }

        private Surface Pick(FireStartGroup group)
        {
            string[] candidates = group.Candidates;
            int offset = UnityEngine.Random.Range(0, candidates.Length);
            for (int i = 0; i < candidates.Length; i++)
            {
                Surface surface = _registry.FindById(candidates[(offset + i) % candidates.Length]);
                if (surface != null && !surface.IsRuined)
                {
                    return surface;
                }
            }

            Debug.LogWarning($"[Fire] No outbreak candidate found for '{group.Room}'.");
            return null;
        }
    }
}
