using FB.Game.Config;
using FB.Game.Player;
using UnityEngine;
using Zenject;

namespace FB.Game.CameraFx
{
    /// <summary>Perlin-noise shake applied to the camera's local transform (the pivot keeps the look rotation).</summary>
    public sealed class CameraShaker : ICameraShake, ILateTickable
    {
        private readonly Transform _camera;
        private readonly PlayerConfig _config;
        private float _trauma;
        private float _time;

        public CameraShaker(PlayerBody player, PlayerConfig config)
        {
            _camera = player.Camera.transform;
            _config = config;
        }

        public void AddTrauma(float amount)
        {
            _trauma = Mathf.Clamp01(_trauma + amount);
        }

        public void LateTick()
        {
            _trauma = Mathf.Max(0f, _trauma - _config.ShakeDecay * Time.unscaledDeltaTime);
            _time += Time.unscaledDeltaTime * _config.ShakeFrequency;
            float shake = _trauma * _trauma;

            _camera.localPosition = new Vector3(Noise(0f), Noise(10f), 0f) * (_config.ShakeMaxOffset * shake);
            _camera.localRotation = Quaternion.Euler(
                Noise(20f) * _config.ShakeMaxAngle * shake,
                Noise(30f) * _config.ShakeMaxAngle * shake,
                Noise(40f) * _config.ShakeMaxAngle * shake);
        }

        private float Noise(float seed) => Mathf.PerlinNoise(seed, _time) * 2f - 1f;
    }
}
