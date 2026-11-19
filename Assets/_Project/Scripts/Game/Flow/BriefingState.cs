using FB.Core.Input;
using FB.Core.StateMachine;
using FB.Game.Config;
using FB.Game.Player;
using Zenject;

namespace FB.Game.Flow
{
    /// <summary>Short intro card before the round. Switches to Playing after a fixed delay.</summary>
    public sealed class BriefingState : IGameState, ITickableState, IBriefingClock
    {
        private readonly RoundConfig _config;
        private readonly ICursorService _cursor;
        private readonly IPlayerControl _control;
        private readonly LazyInject<IGameStateSwitcher> _switcher;
        private float _elapsed;

        public BriefingState(RoundConfig config, ICursorService cursor, IPlayerControl control,
            LazyInject<IGameStateSwitcher> switcher)
        {
            _config = config;
            _cursor = cursor;
            _control = control;
            _switcher = switcher;
        }

        public GamePhase Phase => GamePhase.Briefing;

        public float Duration => _config.BriefingSeconds;

        public float Remaining => UnityEngine.Mathf.Max(0f, _config.BriefingSeconds - _elapsed);

        public void Enter()
        {
            _elapsed = 0f;
            _cursor.LockAllowed = true;
            _control.Enabled = false;
        }

        public void Tick(float deltaTime)
        {
            _elapsed += deltaTime;
            if (_elapsed >= _config.BriefingSeconds)
            {
                _switcher.Value.Enter<PlayingState>();
            }
        }

        public void Exit()
        {
        }
    }
}
