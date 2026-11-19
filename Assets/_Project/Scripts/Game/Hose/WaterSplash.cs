using FB.Core.Pooling;
using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>Pooled burst of droplets at a water impact point. Lifetime is managed by <see cref="SplashSpawner"/>.</summary>
    public sealed class WaterSplash : MonoBehaviour, IPoolItem
    {
        [SerializeField] private ParticleSystem _particles;

        public void Play(Vector3 point, Vector3 normal)
        {
            transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
            _particles.Play(true);
        }

        public void OnSpawned()
        {
        }

        public void OnDespawned()
        {
            _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
