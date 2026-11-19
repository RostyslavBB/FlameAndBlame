using System.Collections.Generic;
using FB.Core.StateMachine;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Flow
{
    /// <summary>Round flow: Briefing → Playing → Report. Announces every phase change through the SignalBus.</summary>
    public sealed class GameStateMachine : IGameStateSwitcher, IGameFlow, IInitializable, ITickable
    {
        private readonly StateMachine<IGameState> _machine;
        private readonly SignalBus _signalBus;

        public GameStateMachine(List<IGameState> states, SignalBus signalBus)
        {
            _machine = new StateMachine<IGameState>(states);
            _signalBus = signalBus;
        }

        public GamePhase Phase => _machine.Current?.Phase ?? GamePhase.Briefing;

        public void Initialize()
        {
            Enter<BriefingState>();
        }

        public void Tick()
        {
            _machine.Tick(Time.deltaTime);
        }

        public void Enter<TState>() where TState : IGameState
        {
            _machine.Enter<TState>();
            Debug.Log($"[Flow] Phase: {_machine.Current.Phase}");
            _signalBus.Fire(new GamePhaseChangedSignal(_machine.Current.Phase));
        }
    }
}
