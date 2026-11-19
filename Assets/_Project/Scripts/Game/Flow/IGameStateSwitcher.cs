namespace FB.Game.Flow
{
    /// <summary>Lets states request transitions without depending on the concrete state machine.</summary>
    public interface IGameStateSwitcher
    {
        void Enter<TState>() where TState : IGameState;
    }
}
