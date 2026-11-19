using FB.Core.Pooling;
using UnityEngine;

namespace FB.Game.Effects
{
    /// <summary>
    /// Pooled particle burst (splash, steam, sparks). Lifetime is managed by <see cref="OneShotEffectSpawner{T}"/>.
    /// Concrete subclasses exist only to give each effect its own pool type.
    /// </summary>
    public abstract class OneShotEffect : MonoBehaviour, IPoolItem
    {
        [SerializeField] private ParticleSystem _particles;

        public void Play(Vector3 point, Vector3 direction)
        {
            transform.SetPositionAndRotation(point, Quaternion.LookRotation(direction));
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
