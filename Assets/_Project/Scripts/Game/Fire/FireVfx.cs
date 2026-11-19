using FB.Core.Pooling;
using UnityEngine;

namespace FB.Game.Fire
{
    /// <summary>
    /// Pooled layered fire: flames, glowing core, base glow, embers and smoke, all sized to the burning object.
    /// Intensity grows with the object's heat, so a fresh fire is small and a fully developed one roars.
    /// The point light is rationed by <see cref="FireVisualService"/> for WebGL.
    /// </summary>
    public sealed class FireVfx : MonoBehaviour, IPoolItem
    {
        [SerializeField] private ParticleSystem[] _emitters;
        [SerializeField] private Light _light;
        [SerializeField, Min(0f)] private float _areaReference = 0.5f;
        [SerializeField, Min(0f)] private float _maxAreaFactor = 2.5f;

        private Transform _home;
        private float[] _baseRates;
        private float[] _baseSizes;
        private float _areaFactor = 1f;

        public bool HasLight => _light.enabled;

        private void Awake()
        {
            _home = transform.parent;
            _baseRates = new float[_emitters.Length];
            _baseSizes = new float[_emitters.Length];
            for (int i = 0; i < _emitters.Length; i++)
            {
                _baseRates[i] = _emitters[i].emission.rateOverTimeMultiplier;
                _baseSizes[i] = _emitters[i].main.startSizeMultiplier;
            }
        }

        public void Attach(Transform owner, Bounds bounds)
        {
            transform.SetParent(owner, true);
            transform.SetPositionAndRotation(
                new Vector3(bounds.center.x, bounds.min.y + bounds.size.y * 0.35f, bounds.center.z), Quaternion.identity);

            Vector3 size = bounds.size;
            Vector3 area = new Vector3(Mathf.Max(0.15f, size.x * 0.85f), Mathf.Max(0.1f, size.y * 0.6f),
                Mathf.Max(0.15f, size.z * 0.85f));
            _areaFactor = Mathf.Clamp(area.x * area.z / _areaReference, 0.35f, _maxAreaFactor);

            // Emitter boxes are rotated -90 on X so particles go up: their local Z is world Y.
            Vector3 shapeScale = new Vector3(area.x, area.z, area.y);
            for (int i = 0; i < _emitters.Length; i++)
            {
                ParticleSystem.ShapeModule shape = _emitters[i].shape;
                shape.scale = shapeScale;
                _emitters[i].Play(true);
            }

            _light.transform.localPosition = new Vector3(0f, size.y * 0.6f + 0.3f, 0f);
            SetIntensity(0.35f);
        }

        /// <summary>0.3 (just caught) .. 1.2 (raging): scales emission and flame size.</summary>
        public void SetIntensity(float intensity)
        {
            float sizeScale = Mathf.Lerp(0.55f, 1.3f, Mathf.InverseLerp(0.3f, 1.2f, intensity));
            for (int i = 0; i < _emitters.Length; i++)
            {
                ParticleSystem.EmissionModule emission = _emitters[i].emission;
                emission.rateOverTimeMultiplier = _baseRates[i] * _areaFactor * intensity;
                ParticleSystem.MainModule main = _emitters[i].main;
                main.startSizeMultiplier = _baseSizes[i] * sizeScale;
            }
        }

        public void SetLight(bool enabled) => _light.enabled = enabled;

        public void SetLightIntensity(float intensity) => _light.intensity = intensity;

        public void OnSpawned()
        {
        }

        public void OnDespawned()
        {
            for (int i = 0; i < _emitters.Length; i++)
            {
                _emitters[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            _light.enabled = false;
            transform.SetParent(_home, false);
        }
    }
}
