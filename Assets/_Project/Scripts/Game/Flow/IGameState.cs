using FB.Core.StateMachine;

namespace FB.Game.Flow
{
    /// <summary>A phase of the round (Briefing, Playing, Report).</summary>
    public interface IGameState : IState
    {
        GamePhase Phase { get; }
    }
}
