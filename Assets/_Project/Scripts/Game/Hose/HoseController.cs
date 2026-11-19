using FB.Core.Input;
using FB.Game.Blame;
using FB.Game.CameraFx;
using FB.Game.Config;
using FB.Game.Player;
using UnityEngine;
using Zenject;

namespace FB.Game.Hose
{
    /// <summary>
    /// Facade of the hose: reads the trigger, drives pressure, the particle jet, recoil and camera shake.
    /// Every trigger press starts a new water Cause (Player, HoseSpray).
    /// </summary>
    public sealed class HoseController : IHoseFacade, IInitializable, ITickable, IFixedTickable
    {
        private const float SprayThreshold = 0.01f;

        private readonly HoseView _view;
        private readonly HoseConfig _config;
        private readonly HosePressure _pressure;
        private readonly IInputService _input;
        private readonly ICursorService _cursor;
        private readonly ICauseFactory _causes;
        private readonly IKnockbackReceiver _knockback;
        private readonly ICameraShake _shake;
        private readonly IPlayerControl _control;
        private ParticleSystem _particles;
        private bool _wasTriggered;

        public HoseController(HoseView view, HoseConfig config, HosePressure pressure, IInputService input,
            ICursorService cursor, ICauseFactory causes, IKnockbackReceiver knockback, ICameraShake shake,
            IPlayerControl control)
        {
            _view = view;
            _config = config;
            _pressure = pressure;
            _input = input;
            _cursor = cursor;
            _causes = causes;
            _knockback = knockback;
            _shake = shake;
            _control = control;
        }

        public float Pressure => _pressure.Value;
        public bool IsSpraying => _pressure.Value > SprayThreshold;
        public Vector3 NozzlePosition => _view.NozzleTip.position;
        public Vector3 SprayDirection => _view.NozzleTip.forward;
        public Cause CurrentCause { get; private set; }

        public void Initialize()
        {
            _particles = _view.Jet.Particles;
            ParticleSystem.MainModule main = _particles.main;
            main.maxParticles = _config.MaxParticles;
            main.startLifetime = _config.ParticleLifetime;
            main.gravityModifier = _config.ParticleGravity;
            main.startSize = _config.ParticleSize;
            ParticleSystem.ShapeModule shape = _particles.shape;
            shape.angle = _config.SpreadAngle;
            SetJet(0f);
            _particles.Play();
            if (_view.Mist != null)
            {
                _view.Mist.Play();
            }
        }

        public void Tick()
        {
            bool triggered = _control.Enabled && _input.FireHeld && _cursor.IsLocked;
            if (triggered && !_wasTriggered)
            {
                CurrentCause = _causes.Create(ActorId.Player, CauseAction.HoseSpray);
            }

            _wasTriggered = triggered;
            _pressure.Update(triggered, Time.deltaTime);
            SetJet(_pressure.Value);

            if (IsSpraying)
            {
                _shake.AddTrauma(_pressure.Value * _config.ShakePerSecond * Time.deltaTime);
            }
        }

        public void FixedTick()
        {
            if (!IsSpraying)
            {
                return;
            }

            float share = _config.RecoilCurve.Evaluate(_pressure.Value);
            Vector3 force = -SprayDirection * (_config.MaxRecoilForce * share);
            force.y *= _config.VerticalRecoilScale;
            _knockback.ApplyImpulse(force * Time.fixedDeltaTime);
        }

        private void SetJet(float pressure)
        {
            ParticleSystem.EmissionModule emission = _particles.emission;
            emission.rateOverTimeMultiplier = pressure > SprayThreshold ? _config.MaxEmissionRate * pressure : 0f;
            ParticleSystem.MainModule main = _particles.main;
            main.startSpeedMultiplier = Mathf.Lerp(_config.MinJetSpeed, _config.MaxJetSpeed, pressure);

            if (_view.Mist != null)
            {
                ParticleSystem.EmissionModule mist = _view.Mist.emission;
                mist.rateOverTimeMultiplier = pressure > SprayThreshold ? _config.MaxMistRate * pressure * pressure : 0f;
            }
        }
    }
}
