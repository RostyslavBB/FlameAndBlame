using FB.Game.Flow;

namespace FB.Game.Signals
{
    /// <summary>Fired after the round state machine entered a new phase.</summary>
    public readonly struct GamePhaseChangedSignal
    {
        public GamePhaseChangedSignal(GamePhase phase)
        {
            Phase = phase;
        }

        public GamePhase Phase { get; }
    }
}
