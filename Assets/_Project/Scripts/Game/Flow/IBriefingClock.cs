namespace FB.Game.Flow
{
    /// <summary>Read-only countdown of the briefing phase: the UI shows exactly the time the flow waits for.</summary>
    public interface IBriefingClock
    {
        float Duration { get; }
        float Remaining { get; }
    }
}
