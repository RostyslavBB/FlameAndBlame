using FB.Game.Audio;
using FB.Game.Flow;
using FB.Game.Hose;
using FB.Game.Power;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>
    /// The live gauges: pressure bar (glows and trembles at full power), a crosshair that opens up with the
    /// pressure, and the onboarding tips and the breaker hint, popping in on a bouncy pill that gently bobs.
    /// </summary>
    public sealed class HudGaugesPresenter : IInitializable, ITickable
    {
        private const string HintOn = "<color=#FFD24D>[E]</color>  Вимкнути струм";
        private const string HintOff = "<color=#FFD24D>[E]</color>  Увімкнути струм";
        private const float HintPopSeconds = 0.3f;
        private const string SprayTip = "Затисни <color=#FFD24D>[ЛКМ]</color>  — гаси вогонь";
        private const string PowerTip = "Вода + струм = біда.  Щиток вимикає <color=#FFD24D>[E]</color>";
        private const float SprayTipMaxSeconds = 10f;
        private const float SprayToLearn = 0.6f;
        private const float PowerTipSeconds = 6f;

        private readonly HudView _view;
        private readonly IHoseFacade _hose;
        private readonly IBreakerInteraction _breaker;
        private readonly IPowerGrid _power;
        private readonly IGameFlow _flow;
        private readonly UiConfig _config;
        private readonly IAudioService _audio;
        private string _shownHint;
        private float _hintAlpha;
        private float _hintTime;
        private float _time;
        private int _tipStage;
        private float _sprayed;
        private float _powerTipStart;

        public HudGaugesPresenter(HudView view, IHoseFacade hose, IBreakerInteraction breaker, IPowerGrid power,
            IGameFlow flow, UiConfig config, IAudioService audio)
        {
            _view = view;
            _hose = hose;
            _breaker = breaker;
            _power = power;
            _flow = flow;
            _config = config;
            _audio = audio;
        }

        public void Initialize()
        {
            _view.SetHint(string.Empty);
            _view.SetHintStyle(0f, 1f, 0f);
        }

        public void Tick()
        {
            if (_flow.Phase != GamePhase.Playing)
            {
                return;
            }

            float deltaTime = UiTween.DeltaTime;
            _time += deltaTime;
            TickPressure();
            TickHint(deltaTime);
        }

        private void TickPressure()
        {
            float pressure = _hose.Pressure;
            float hot = Mathf.InverseLerp(0.75f, 1f, pressure);
            Color color = Color.Lerp(_config.PressureColor, _config.PressureMaxColor, hot);
            float glow = hot * (0.55f + 0.45f * Mathf.Sin(_time * 18f));
            Vector2 shake = pressure > 0.95f ? Random.insideUnitCircle * 2.5f : Vector2.zero;
            _view.SetPressure(pressure, color, glow, shake);

            float breathe = 1f + Mathf.Sin(_time * 3f) * 0.04f;
            _view.SetCrosshair((1f + pressure * 0.6f) * breathe, Color.Lerp(Color.white, color, pressure));
        }

        /// <summary>
        /// Onboarding tips at the start of the round: "hold LMB" until the player has actually sprayed a bit, then
        /// a short warning about water and electricity. The breaker prompt always wins when it is in reach.
        /// </summary>
        private string CurrentTip(float deltaTime)
        {
            if (_tipStage == 0)
            {
                if (_hose.Pressure > 0.5f)
                {
                    _sprayed += deltaTime;
                }

                if (_sprayed < SprayToLearn && _time < SprayTipMaxSeconds)
                {
                    return SprayTip;
                }

                _tipStage = 1;
                _powerTipStart = _time;
            }

            if (_tipStage == 1)
            {
                if (_time - _powerTipStart < PowerTipSeconds)
                {
                    return PowerTip;
                }

                _tipStage = 2;
            }

            return null;
        }

        private void TickHint(float deltaTime)
        {
            string hint = _breaker.InReach ? (_power.IsOn ? HintOn : HintOff) : CurrentTip(deltaTime);
            if (hint != null && !ReferenceEquals(hint, _shownHint))
            {
                _view.SetHint(hint);
                _hintTime = 0f;
                _audio.PlayUi(SoundId.Pop, 0.5f);
            }

            _shownHint = hint;
            _hintTime += deltaTime;
            _hintAlpha = Mathf.MoveTowards(_hintAlpha, hint != null ? 1f : 0f, deltaTime * 6f);
            float pop = Mathf.LerpUnclamped(0.5f, 1f, UiTween.EaseOutBack(Mathf.Clamp01(_hintTime / HintPopSeconds)));
            _view.SetHintStyle(_hintAlpha, hint != null ? pop : 1f, Mathf.Sin(_time * 4f) * 4f);
        }
    }
}
