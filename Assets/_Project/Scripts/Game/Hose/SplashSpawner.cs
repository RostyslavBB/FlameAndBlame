using FB.Core.Pooling;
using FB.Game.Config;
using UnityEngine;
using Zenject;

namespace FB.Game.Hose
{
    /// <summary>Spawns pooled splashes with a rate limit and returns them to the pool when they expire.</summary>
    public sealed class SplashSpawner : ITickable
    {
        private readonly IPool<WaterSplash> _pool;
        private readonly HoseConfig _config;
        private readonly WaterSplash[] _active;
        private readonly float[] _expireAt;
        private int _count;
        private float _nextSpawnTime;

        public SplashSpawner(IPool<WaterSplash> pool, HoseConfig config)
        {
            _pool = pool;
            _config = config;
            _active = new WaterSplash[config.MaxActiveSplashes];
            _expireAt = new float[config.MaxActiveSplashes];
        }

        public void TrySpawn(Vector3 point, Vector3 normal)
        {
            float now = Time.time;
            if (now < _nextSpawnTime || _count >= _active.Length)
            {
                return;
            }

            _nextSpawnTime = now + _config.SplashCooldown;
            WaterSplash splash = _pool.Spawn();
            splash.Play(point, normal);
            _active[_count] = splash;
            _expireAt[_count] = now + _config.SplashLifetime;
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
