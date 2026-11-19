namespace FB.Core.StateMachine
{
    /// <summary>A state of a <see cref="StateMachine{TState}"/>.</summary>
    public interface IState
    {
        /// <summary>Called when the machine switches into this state.</summary>
        void Enter();

        /// <summary>Called when the machine leaves this state.</summary>
        void Exit();
    }
}
