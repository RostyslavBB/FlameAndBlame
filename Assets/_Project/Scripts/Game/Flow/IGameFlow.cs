namespace FB.Game.Flow
{
    /// <summary>Current round phase, for presenters that start after the first phase change.</summary>
    public interface IGameFlow
    {
        GamePhase Phase { get; }
    }
}
