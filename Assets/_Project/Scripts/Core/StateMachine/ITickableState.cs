namespace FB.Core.StateMachine
{
    /// <summary>A state that wants a per-frame update while it is active.</summary>
    public interface ITickableState
    {
        /// <summary>Per-frame update with scaled delta time.</summary>
        void Tick(float deltaTime);
    }
}
