namespace FB.Game.Power
{
    /// <summary>Breaker state needed by the HUD prompt.</summary>
    public interface IBreakerInteraction
    {
        /// <summary>True when the player is close enough to press E.</summary>
        bool InReach { get; }
    }
}
