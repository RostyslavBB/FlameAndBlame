using FB.Core.Pooling;
using UnityEngine;
using Zenject;

namespace FB.Game.Power
{
    /// <summary>Shows short-lived jittering arcs between two points; pooled and capped.</summary>
    public sealed class ArcSpawner : ITickable
    {
        private readonly IPool<ElectricArc> _pool;
        private readonly PowerConfig _config;
        private readonly ElectricArc[] _arcs;
        private readonly Vector3[] _from;
        private readonly Vector3[] _to;
        private readonly float[] _expireAt;
        private int _count;

        public ArcSpawner(IPool<ElectricArc> pool, PowerConfig config)
        {
            _pool = pool;
            _config = config;
            _arcs = new ElectricArc[config.MaxArcs];
            _from = new Vector3[config.MaxArcs];
            _to = new Vector3[config.MaxArcs];
            _expireAt = new float[config.MaxArcs];
        }

        public void Spawn(Vector3 from, Vector3 to)
        {
            if (_count >= _arcs.Length)
            {
                return;
            }

            _arcs[_count] = _pool.Spawn();
            _from[_count] = from;
            _to[_count] = to;
            _expireAt[_count] = Time.time + _config.ArcLifetime;
            _count++;
        }

        public void Tick()
        {
            float now = Time.time;
            for (int i = _count - 1; i >= 0; i--)
            {
                if (now >= _expireAt[i])
                {
                    Remove(i);
                    continue;
                }

                Jitter(_arcs[i], _from[i], _to[i]);
            }
        }

        private void Jitter(ElectricArc arc, Vector3 from, Vector3 to)
        {
            int last = arc.PointCount - 1;
            for (int p = 0; p <= last; p++)
            {
                float t = p / (float)last;
                Vector3 offset = p == 0 || p == last
                    ? Vector3.zero
                    : Random.insideUnitSphere * (_config.ArcJitter * Mathf.Sin(t * Mathf.PI));
                arc.SetPoint(p, Vector3.Lerp(from, to, t) + offset);
            }
        }

        private void Remove(int index)
        {
            _pool.Despawn(_arcs[index]);
            _count--;
            _arcs[index] = _arcs[_count];
            _from[index] = _from[_count];
            _to[index] = _to[_count];
            _expireAt[index] = _expireAt[_count];
            _arcs[_count] = null;
        }
    }
}
