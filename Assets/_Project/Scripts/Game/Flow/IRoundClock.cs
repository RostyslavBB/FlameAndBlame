namespace FB.Game.Flow
{
    /// <summary>Read-only round timer for HUD and other systems.</summary>
    public interface IRoundClock
    {
        float Duration { get; }
        float Remaining { get; }
        bool IsRunning { get; }
    }
}
