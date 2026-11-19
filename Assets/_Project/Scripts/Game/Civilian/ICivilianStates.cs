namespace FB.Game.Civilian
{
    /// <summary>Lets a neighbour state switch the machine without knowing who owns it.</summary>
    public interface ICivilianStates
    {
        void Enter<TState>() where TState : CivilianState;
    }
}
