using FB.Game.Config;
using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>Pressure model: rises along the config curve while held, decays linearly when released.</summary>
    public sealed class HosePressure
    {
        private readonly HoseConfig _config;
        private float _holdTime;

        public HosePressure(HoseConfig config)
        {
            _config = config;
        }

        public float Value { get; private set; }

        public void Update(bool triggerHeld, float deltaTime)
        {
            if (triggerHeld)
            {
                _holdTime = Mathf.Min(_holdTime + deltaTime, _config.RampSeconds);
                Value = Mathf.Max(Value, Mathf.Clamp01(_config.PressureCurve.Evaluate(_holdTime / _config.RampSeconds)));
                return;
            }

            Value = Mathf.Max(0f, Value - _config.DecayPerSecond * deltaTime);
            // Keep the curve continuous if the trigger is pressed again before pressure dropped to zero.
            _holdTime = Value * _config.RampSeconds;
        }
    }
}
