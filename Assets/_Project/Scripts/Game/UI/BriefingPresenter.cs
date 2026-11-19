using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Game.Config;
using FB.Game.Flow;
using FB.Game.Signals;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>Shows the briefing card during the Briefing phase and fades it out when the round starts.</summary>
    public sealed class BriefingPresenter : IInitializable, IDisposable
    {
        private const float FadeSeconds = 0.35f;

        private readonly BriefingView _view;
        private readonly RoundConfig _config;
        private readonly IGameFlow _flow;
        private readonly SignalBus _signalBus;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public BriefingPresenter(BriefingView view, RoundConfig config, IGameFlow flow, SignalBus signalBus)
        {
            _view = view;
            _config = config;
            _flow = flow;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _view.SetText(_config.BriefingTitle, _config.BriefingText);
            _view.SetAlpha(_flow.Phase == GamePhase.Briefing ? 1f : 0f);
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            if (signal.Phase == GamePhase.Briefing)
            {
                _view.SetAlpha(1f);
            }
            else
            {
                UiTween.Run(FadeSeconds, t => _view.SetAlpha(1f - t), _lifetime.Token).Forget();
            }
        }
    }
}
