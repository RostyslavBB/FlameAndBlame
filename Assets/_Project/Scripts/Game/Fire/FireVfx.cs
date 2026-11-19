using FB.Core.Pooling;
using UnityEngine;

namespace FB.Game.Fire
{
    /// <summary>
    /// Pooled fire visual: flames + smoke sized to the burning object's bounds, plus an optional point light
    /// (lights are rationed by <see cref="FireVisualService"/> for WebGL).
    /// </summary>
    public sealed class FireVfx : MonoBehaviour, IPoolItem
    {
        [SerializeField] private ParticleSystem _flames;
        [SerializeField] private ParticleSystem _smoke;
        [SerializeField] private Light _light;
        [SerializeField, Min(0f)] private float _flamesPerSquareMeter = 40f;
        [SerializeField, Min(0f)] private float _minRate = 12f;
        [SerializeField, Min(0f)] private float _maxRate = 60f;

        private Transform _home;

        public bool HasLight => _light.enabled;

        private void Awake()
        {
            _home = transform.parent;
        }

        public void Attach(Transform owner, Bounds bounds)
        {
            transform.SetParent(owner, true);
            transform.SetPositionAndRotation(
                new Vector3(bounds.center.x, bounds.min.y + bounds.size.y * 0.4f, bounds.center.z), Quaternion.identity);

            Vector3 size = bounds.size;
            Vector3 area = new Vector3(Mathf.Max(0.15f, size.x * 0.8f), Mathf.Max(0.1f, size.y * 0.5f),
                Mathf.Max(0.15f, size.z * 0.8f));
            // The emitter box is rotated -90 on X so particles go up: its local Z is world Y.
            Vector3 shapeScale = new Vector3(area.x, area.z, area.y);
            ParticleSystem.ShapeModule flameShape = _flames.shape;
            flameShape.scale = shapeScale;
            ParticleSystem.ShapeModule smokeShape = _smoke.shape;
            smokeShape.scale = shapeScale;

            ParticleSystem.EmissionModule emission = _flames.emission;
            emission.rateOverTimeMultiplier = Mathf.Clamp(area.x * area.z * _flamesPerSquareMeter, _minRate, _maxRate);
            _light.transform.localPosition = new Vector3(0f, size.y * 0.6f + 0.2f, 0f);

            _flames.Play(true);
            _smoke.Play(true);
        }

        public void SetLight(bool enabled) => _light.enabled = enabled;

        public void SetLightIntensity(float intensity) => _light.intensity = intensity;

        public void OnSpawned()
        {
        }

        public void OnDespawned()
        {
            _flames.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _light.enabled = false;
            transform.SetParent(_home, false);
        }
    }
}
