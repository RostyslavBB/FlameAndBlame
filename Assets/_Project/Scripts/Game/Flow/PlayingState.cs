using FB.Core.StateMachine;
using FB.Game.Config;
using FB.Game.Fire;
using FB.Game.Player;
using Zenject;

namespace FB.Game.Flow
{
    /// <summary>The round itself. Ends when the clock runs out or every fire is out (after a short pause).</summary>
    public sealed class PlayingState : IGameState, ITickableState
    {
        private readonly RoundConfig _config;
        private readonly RoundClock _clock;
        private readonly IFireStatus _fire;
        private readonly IPlayerControl _control;
        private readonly LazyInject<IGameStateSwitcher> _switcher;
        private float _allClearTime;

        public PlayingState(RoundConfig config, RoundClock clock, IFireStatus fire, IPlayerControl control,
            LazyInject<IGameStateSwitcher> switcher)
        {
            _config = config;
            _clock = clock;
            _fire = fire;
            _control = control;
            _switcher = switcher;
        }

        public GamePhase Phase => GamePhase.Playing;

        public void Enter()
        {
            _clock.Start(_config.RoundSeconds);
            _control.Enabled = true;
            _allClearTime = 0f;
        }

        public void Tick(float deltaTime)
        {
            _clock.Advance(deltaTime);
            bool allClear = _fire.HasStarted && _fire.BurningCount == 0;
            _allClearTime = allClear ? _allClearTime + deltaTime : 0f;

            if (_clock.IsOver || _allClearTime >= _config.AllClearDelay)
            {
                _switcher.Value.Enter<ReportState>();
            }
        }

        public void Exit()
        {
            _clock.Stop();
        }
    }
}
