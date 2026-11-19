using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Game.Blame;
using FB.Game.Flow;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>
    /// HUD frame and the two numbers on top: slides the HUD in when the round starts, ticks the timer (red and
    /// pulsing near the end, shaking in the last seconds) and rolls the damage bill up with a punch on every hit.
    /// </summary>
    public sealed class HudPresenter : IInitializable, IDisposable, ITickable
    {
        private const float SlideOffset = 220f;
        private const float DamageRollSpeed = 6f;

        private readonly HudView _view;
        private readonly IRoundClock _clock;
        private readonly IBlameService _blame;
        private readonly IGameFlow _flow;
        private readonly SignalBus _signalBus;
        private readonly UiConfig _config;
        private readonly UiPulse _tick = new UiPulse(0.4f);
        private readonly UiPulse _damagePunch = new UiPulse(0.5f);
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private int _shownSeconds = -1;
        private float _rolledDamage;
        private int _shownDamage = -1;
        private int _targetDamage;

        public HudPresenter(HudView view, IRoundClock clock, IBlameService blame, IGameFlow flow, SignalBus signalBus,
            UiConfig config)
        {
            _view = view;
            _clock = clock;
            _blame = blame;
            _flow = flow;
            _signalBus = signalBus;
            _config = config;
        }

        public void Initialize()
        {
            bool playing = _flow.Phase == GamePhase.Playing;
            _view.SetAlpha(playing ? 1f : 0f);
            _view.SetSlide(playing ? 0f : SlideOffset);
            _view.SetDamage(Money.Of(0));
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        public void Tick()
        {
            if (_flow.Phase != GamePhase.Playing)
            {
                return;
            }

            float deltaTime = UiTween.DeltaTime;
            TickTimer(deltaTime);
            TickDamage(deltaTime);
        }

        private void TickTimer(float deltaTime)
        {
            int seconds = Mathf.CeilToInt(_clock.Remaining);
            bool warning = seconds <= _config.TimerWarningSeconds;
            if (seconds != _shownSeconds)
            {
                _shownSeconds = seconds;
                _view.SetTime($"{seconds / 60}:{seconds % 60:00}");
                if (warning)
                {
                    _tick.Trigger();
                }
            }

            float punch = _tick.Advance(deltaTime);
            Color face = warning
                ? Color.Lerp(_config.PanelColor, _config.WarningColor, 0.45f + 0.55f * _tick.Fade)
                : _config.PanelColor;
            Color text = warning ? Color.Lerp(Color.white, new Color(1f, 0.85f, 0.4f), _tick.Fade) : Color.white;
            Vector2 shake = seconds <= _config.TimerPanicSeconds ? UnityEngine.Random.insideUnitCircle * 3f : Vector2.zero;
            _view.SetTimerStyle(1f + punch * 0.22f, face, text, shake);
        }

        private void TickDamage(float deltaTime)
        {
            int target = _blame.Ledger.TotalDamage;
            if (target != _targetDamage)
            {
                _targetDamage = target;
                _damagePunch.Trigger();
            }

            // Roll toward the bill: fast for big jumps, but every dollar is visibly counted.
            float step = Mathf.Max(60f, (target - _rolledDamage) * DamageRollSpeed) * deltaTime;
            _rolledDamage = Mathf.MoveTowards(_rolledDamage, target, step);
            int shown = Mathf.RoundToInt(_rolledDamage);
            if (shown != _shownDamage)
            {
                _shownDamage = shown;
                _view.SetDamage(Money.Of(shown));
            }

            float punch = _damagePunch.Advance(deltaTime);
            _view.SetDamageStyle(1f + punch * 0.3f, Color.Lerp(Color.white, _config.AccentColor, _damagePunch.Fade));
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            if (signal.Phase == GamePhase.Playing)
            {
                SlideIn(_lifetime.Token).Forget();
            }
            else
            {
                _view.SetAlpha(0f);
            }
        }

        private async UniTaskVoid SlideIn(CancellationToken token)
        {
            try
            {
                await UiTween.Run(_config.HudIntroSeconds, t =>
                {
                    _view.SetAlpha(Mathf.Clamp01(t * 3f));
                    _view.SetSlide(Mathf.LerpUnclamped(SlideOffset, 0f, UiTween.EaseOutBack(t)));
                }, token);
            }
            catch (OperationCanceledException)
            {
                // Scene disposed mid-intro.
            }
        }
    }
}
