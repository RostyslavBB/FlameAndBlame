using FB.Core.StateMachine;
using FB.Game.Config;
using Zenject;

namespace FB.Game.Flow
{
    /// <summary>The actual round. Ends when the clock runs out (fire-out condition is added with the fire module).</summary>
    public sealed class PlayingState : IGameState, ITickableState
    {
        private readonly RoundConfig _config;
        private readonly RoundClock _clock;
        private readonly LazyInject<IGameStateSwitcher> _switcher;

        public PlayingState(RoundConfig config, RoundClock clock, LazyInject<IGameStateSwitcher> switcher)
        {
            _config = config;
            _clock = clock;
            _switcher = switcher;
        }

        public GamePhase Phase => GamePhase.Playing;

        public void Enter()
        {
            _clock.Start(_config.RoundSeconds);
        }

        public void Tick(float deltaTime)
        {
            _clock.Advance(deltaTime);
            if (_clock.IsOver)
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
