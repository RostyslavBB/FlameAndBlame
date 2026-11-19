using FB.Core.Pooling;
using UnityEngine;
using Zenject;

namespace FB.Game.Effects
{
    /// <summary>Spawns pooled one-shot effects with a rate limit and returns them to the pool when they expire.</summary>
    public sealed class OneShotEffectSpawner<T> : ITickable where T : OneShotEffect
    {
        private readonly IPool<T> _pool;
        private readonly EffectSpawnSettings _settings;
        private readonly T[] _active;
        private readonly float[] _expireAt;
        private int _count;
        private float _nextSpawnTime;

        public OneShotEffectSpawner(IPool<T> pool, EffectSpawnSettings settings)
        {
            _pool = pool;
            _settings = settings;
            _active = new T[settings.MaxActive];
            _expireAt = new float[settings.MaxActive];
        }

        public void TrySpawn(Vector3 point, Vector3 direction)
        {
            float now = Time.time;
            if (now < _nextSpawnTime || _count >= _active.Length)
            {
                return;
            }

            _nextSpawnTime = now + _settings.Cooldown;
            T effect = _pool.Spawn();
            effect.Play(point, direction);
            _active[_count] = effect;
            _expireAt[_count] = now + _settings.Lifetime;
            _count++;
        }

        public void Tick()
        {
            float now = Time.time;
            for (int i = _count - 1; i >= 0; i--)
            {
                if (now < _expireAt[i])
                {
                    continue;
                }

                _pool.Despawn(_active[i]);
                _count--;
                _active[i] = _active[_count];
                _expireAt[i] = _expireAt[_count];
                _active[_count] = null;
            }
        }
    }
}
