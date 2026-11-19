using FB.Core.StateMachine;

namespace FB.Game.Civilian
{
    /// <summary>Base of the neighbour's AI states (Wander, Panic, Knocked).</summary>
    public abstract class CivilianState : IState, ITickableState
    {
        protected CivilianState(CivilianBody body, ICivilianStates states)
        {
            Body = body;
            States = states;
        }

        protected CivilianBody Body { get; }
        protected ICivilianStates States { get; }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public abstract void Tick(float deltaTime);
    }
}
