using FB.Core.Input;
using FB.Game.Player;

namespace FB.Game.Flow
{
    /// <summary>End of round: frees the cursor so the player can click the report UI.</summary>
    public sealed class ReportState : IGameState
    {
        private readonly ICursorService _cursor;
        private readonly IPlayerControl _control;

        public ReportState(ICursorService cursor, IPlayerControl control)
        {
            _cursor = cursor;
            _control = control;
        }

        public GamePhase Phase => GamePhase.Report;

        public void Enter()
        {
            _control.Enabled = false;
            _cursor.LockAllowed = false;
            _cursor.Unlock();
        }

        public void Exit()
        {
        }
    }
}
