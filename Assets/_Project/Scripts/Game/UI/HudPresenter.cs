using System;
using FB.Game.Flow;
using FB.Game.Hose;
using FB.Game.Power;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>Timer, pressure bar and the breaker hint. Strings are rebuilt only when their value changes.</summary>
    public sealed class HudPresenter : IInitializable, IDisposable, ITickable
    {
        private const string HintOn = "[E] Вимкнути струм";
        private const string HintOff = "[E] Увімкнути струм";

        private readonly HudView _view;
        private readonly IRoundClock _clock;
        private readonly IHoseFacade _hose;
        private readonly IBreakerInteraction _breaker;
        private readonly IPowerGrid _power;
        private readonly IGameFlow _flow;
        private readonly SignalBus _signalBus;
        private readonly UiConfig _config;
        private int _shownSeconds = -1;
        private string _shownHint;

        public HudPresenter(HudView view, IRoundClock clock, IHoseFacade hose, IBreakerInteraction breaker,
            IPowerGrid power, IGameFlow flow, SignalBus signalBus, UiConfig config)
        {
            _view = view;
            _clock = clock;
            _hose = hose;
            _breaker = breaker;
            _power = power;
            _flow = flow;
            _signalBus = signalBus;
            _config = config;
        }

        public void Initialize()
        {
            _view.SetHint(null);
            _view.SetVisible(_flow.Phase == GamePhase.Playing);
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Tick()
        {
            if (_flow.Phase != GamePhase.Playing)
            {
                return;
            }

            int seconds = Mathf.CeilToInt(_clock.Remaining);
            if (seconds != _shownSeconds)
            {
                _shownSeconds = seconds;
                _view.SetTime($"{seconds / 60}:{seconds % 60:00}");
            }

            float pressure = _hose.Pressure;
            Color color = Color.Lerp(_config.PressureColor, _config.PressureMaxColor, Mathf.InverseLerp(0.7f, 1f, pressure));
            _view.SetPressure(pressure, color);

            string hint = _breaker.InReach ? (_power.IsOn ? HintOn : HintOff) : null;
            if (!ReferenceEquals(hint, _shownHint))
            {
                _shownHint = hint;
                _view.SetHint(hint);
            }
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            _view.SetVisible(signal.Phase == GamePhase.Playing);
        }
    }
}
