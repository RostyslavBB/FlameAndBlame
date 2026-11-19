using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Game.Config;
using FB.Game.Flow;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>
    /// Briefing sequence: the card swings up from below, the title slams in, the text types itself out, a siren
    /// glow pulses and a bar counts down to the start. When the round starts the card pops away.
    /// </summary>
    public sealed class BriefingPresenter : IInitializable, IDisposable, ITickable
    {
        private const float OutroSeconds = 0.35f;
        private const float CardDrop = -760f;
        private const string CountdownPrefix = "СТАРТ ЧЕРЕЗ ";

        private readonly BriefingView _view;
        private readonly RoundConfig _round;
        private readonly IBriefingClock _clock;
        private readonly UiConfig _config;
        private readonly IGameFlow _flow;
        private readonly SignalBus _signalBus;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private float _elapsed;
        private bool _shown;
        private int _shownSeconds = -1;
        private readonly UiPulse _secondPunch = new UiPulse(0.35f);

        public BriefingPresenter(BriefingView view, RoundConfig round, IBriefingClock clock, UiConfig config,
            IGameFlow flow, SignalBus signalBus)
        {
            _view = view;
            _round = round;
            _clock = clock;
            _config = config;
            _flow = flow;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _view.SetText(_round.BriefingTitle, _round.BriefingText);
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
            if (_flow.Phase == GamePhase.Briefing)
            {
                Intro(_lifetime.Token).Forget();
            }
            else
            {
                _view.SetAlpha(0f);
            }
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        public void Tick()
        {
            if (_flow.Phase != GamePhase.Briefing)
            {
                return;
            }

            _elapsed += UiTween.DeltaTime;
            _view.SetSiren(0.35f + 0.35f * Mathf.Sin(_elapsed * 9f));
            // Read the flow's own clock: a private UI timer drifts (huge unscaled delta on the first frame after load).
            float remaining = _clock.Remaining;
            _view.SetCountdown(remaining / Mathf.Max(0.01f, _clock.Duration));
            int seconds = Mathf.Max(1, Mathf.CeilToInt(remaining));
            if (seconds != _shownSeconds)
            {
                _shownSeconds = seconds;
                _view.SetCountdownLabel(CountdownPrefix + seconds);
                _secondPunch.Trigger();
            }

            _view.SetCountdownScale(1f + _secondPunch.Advance(UiTween.DeltaTime) * 0.3f);
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            if (signal.Phase == GamePhase.Briefing)
            {
                if (!_shown)
                {
                    Intro(_lifetime.Token).Forget();
                }
            }
            else if (_shown)
            {
                Outro(_lifetime.Token).Forget();
            }
        }

        private async UniTaskVoid Intro(CancellationToken token)
        {
            _shown = true;
            _elapsed = 0f;
            _shownSeconds = -1;
            _view.SetAlpha(1f);
            _view.SetVisibleCharacters(0);
            _view.SetTitleScale(0f);
            try
            {
                await UiTween.Run(_config.BriefingIntroSeconds, t =>
                {
                    float eased = UiTween.EaseOutBack(t);
                    _view.SetCard(Mathf.LerpUnclamped(CardDrop, 0f, eased), 1f, Mathf.LerpUnclamped(-8f, 0f, eased));
                }, token);

                UiTween.Run(0.45f, t => _view.SetTitleScale(Mathf.LerpUnclamped(2.2f, 1f, UiTween.EaseOutElastic(t))),
                    token).Forget();
                int length = _view.BodyLength;
                await UiTween.Run(length / _config.BriefingCharsPerSecond,
                    t => _view.SetVisibleCharacters(Mathf.CeilToInt(length * t)), token);
            }
            catch (OperationCanceledException)
            {
                // The round started or the scene is being disposed.
            }
        }

        private async UniTaskVoid Outro(CancellationToken token)
        {
            _shown = false;
            _view.SetVisibleCharacters(int.MaxValue);
            try
            {
                await UiTween.Run(OutroSeconds, t =>
                {
                    _view.SetCard(0f, Mathf.Lerp(1f, 1.15f, UiTween.EaseOutCubic(t)), 0f);
                    _view.SetAlpha(1f - t);
                }, token);
            }
            catch (OperationCanceledException)
            {
                // Scene disposed.
            }
        }
    }
}
