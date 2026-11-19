using FB.Core.Input;

namespace FB.Game.Flow
{
    /// <summary>End of round: frees the cursor so the player can click the report UI.</summary>
    public sealed class ReportState : IGameState
    {
        private readonly ICursorService _cursor;

        public ReportState(ICursorService cursor)
        {
            _cursor = cursor;
        }

        public GamePhase Phase => GamePhase.Report;

        public void Enter()
        {
            _cursor.LockAllowed = false;
            _cursor.Unlock();
        }

        public void Exit()
        {
        }
    }
}
