using FB.Game.Hose;
using FB.Game.Player;
using UnityEngine;
using Zenject;

namespace FB.Game.CameraFx
{
    /// <summary>Widens the field of view near max hose pressure: the jet feels like it is about to rip the arms off.</summary>
    public sealed class FovKick : IInitializable, ILateTickable
    {
        private readonly Camera _camera;
        private readonly IHoseFacade _hose;
        private readonly JuiceConfig _config;
        private float _baseFov;

        public FovKick(PlayerBody player, IHoseFacade hose, JuiceConfig config)
        {
            _camera = player.Camera;
            _hose = hose;
            _config = config;
        }

        public void Initialize()
        {
            _baseFov = _camera.fieldOfView;
        }

        public void LateTick()
        {
            float kick = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(_config.FovKickFromPressure, 1f, _hose.Pressure));
            float target = _baseFov + kick * _config.FovKickDegrees;
            float blend = 1f - Mathf.Exp(-_config.FovLerpSpeed * Time.unscaledDeltaTime);
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, target, blend);
        }
    }
}
