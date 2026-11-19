using System;
using FB.Game.Blame;
using FB.Game.Flow;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Fire
{
    /// <summary>When the round starts, the old stove sets the pan on fire: the root cause of everything.</summary>
    public sealed class FireStarter : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly SurfaceRegistry _registry;
        private readonly ISurfaceActions _actions;
        private readonly ICauseFactory _causes;
        private readonly FireConfig _config;

        public FireStarter(SignalBus signalBus, SurfaceRegistry registry, ISurfaceActions actions,
            ICauseFactory causes, FireConfig config)
        {
            _signalBus = signalBus;
            _registry = registry;
            _actions = actions;
            _causes = causes;
            _config = config;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            if (signal.Phase != GamePhase.Playing)
            {
                return;
            }

            Surface start = _registry.FindById(_config.StartSurfaceId);
            if (start == null)
            {
                Debug.LogError($"[Fire] Start surface '{_config.StartSurfaceId}' not found in the level.");
                return;
            }

            _actions.Ignite(start, _causes.Create(ActorId.Building, CauseAction.Ignite));
        }
    }
}
