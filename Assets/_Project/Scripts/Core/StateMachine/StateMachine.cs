using System;
using System.Collections.Generic;

namespace FB.Core.StateMachine
{
    /// <summary>
    /// Minimal class-based state machine. States are registered once and looked up by type,
    /// so transitions read as Enter&lt;PlayingState&gt;().
    /// </summary>
    public sealed class StateMachine<TState> where TState : class, IState
    {
        private readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();

        public StateMachine(IEnumerable<TState> states)
        {
            foreach (TState state in states)
            {
                _states.Add(state.GetType(), state);
            }
        }

        public TState Current { get; private set; }

        public void Enter<T>() where T : TState
        {
            if (!_states.TryGetValue(typeof(T), out TState next))
            {
                throw new InvalidOperationException($"State {typeof(T).Name} is not registered.");
            }

            Current?.Exit();
            Current = next;
            Current.Enter();
        }

        public void Tick(float deltaTime)
        {
            if (Current is ITickableState tickable)
            {
                tickable.Tick(deltaTime);
            }
        }
    }
}
