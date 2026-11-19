namespace FB.Game.Fire
{
    /// <summary>Read-only fire state for the round flow and HUD.</summary>
    public interface IFireStatus
    {
        /// <summary>True once the first fire was lit this round.</summary>
        bool HasStarted { get; }

        int BurningCount { get; }
    }
}
