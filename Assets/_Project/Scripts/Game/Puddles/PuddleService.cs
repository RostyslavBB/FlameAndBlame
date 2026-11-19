using System;
using System.Collections.Generic;
using FB.Core.Pooling;
using FB.Game.Blame;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Puddles
{
    /// <summary>
    /// Water hitting the floor makes puddles: merges into a nearby puddle or spawns a pooled one, grows with water,
    /// dries over time. Each puddle carries Cause(Puddle) derived from the last water that reached it.
    /// </summary>
    public sealed class PuddleService : IPuddleField, IInitializable, IDisposable, ITickable
    {
        private readonly SignalBus _signalBus;
        private readonly IPool<PuddleView> _pool;
        private readonly ICauseFactory _causes;
        private readonly PuddleConfig _config;
        private readonly List<Puddle> _active;
        private readonly Stack<Puddle> _free;

        public PuddleService(SignalBus signalBus, IPool<PuddleView> pool, ICauseFactory causes, PuddleConfig config)
        {
            _signalBus = signalBus;
            _pool = pool;
            _causes = causes;
            _config = config;
            _active = new List<Puddle>(config.MaxPuddles);
            _free = new Stack<Puddle>(config.MaxPuddles);
            for (int i = 0; i < config.MaxPuddles; i++)
            {
                _free.Push(new Puddle());
            }
        }

        public IReadOnlyList<Puddle> Active => _active;

        public void Initialize()
        {
            _signalBus.Subscribe<WaterHitSignal>(OnWaterHit);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<WaterHitSignal>(OnWaterHit);
        }

        public Puddle FindAt(Vector3 point)
        {
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].Contains(point))
                {
                    return _active[i];
                }
            }

            return null;
        }

        public void Tick()
        {
            float dry = _config.DryPerSecond * Time.deltaTime;
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 30f);
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                Puddle puddle = _active[i];
                puddle.Water -= dry;
                if (puddle.Water < _config.MinWater)
                {
                    Release(i);
                    continue;
                }

                puddle.Radius = RadiusFor(puddle.Water);
                Color color = puddle.IsElectrified
                    ? Color.Lerp(_config.WaterColor, _config.ElectrifiedColor, pulse)
                    : _config.WaterColor;
                puddle.View.Show(puddle.Center, puddle.Radius, color);
            }
        }

        private void OnWaterHit(WaterHitSignal hit)
        {
            if (hit.Normal.y < _config.MinFloorNormalY || hit.Point.y > _config.FloorMaxHeight || hit.Cause == null)
            {
                return;
            }

            Puddle puddle = FindNear(hit.Point) ?? Spawn(hit.Point);
            if (puddle == null)
            {
                return;
            }

            puddle.Water += hit.Amount;
            puddle.Center = Vector3.Lerp(puddle.Center, hit.Point, hit.Amount / puddle.Water);
            puddle.Radius = RadiusFor(puddle.Water);
            if (puddle.LastWaterCause != hit.Cause)
            {
                puddle.LastWaterCause = hit.Cause;
                puddle.Cause = _causes.Create(hit.Cause.Instigator, CauseAction.Puddle, hit.Cause);
            }
        }

        private Puddle FindNear(Vector3 point)
        {
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].Contains(point, _config.MergeDistance))
                {
                    return _active[i];
                }
            }

            return null;
        }

        private Puddle Spawn(Vector3 point)
        {
            if (_free.Count == 0)
            {
                return null;
            }

            Puddle puddle = _free.Pop();
            puddle.View = _pool.Spawn();
            puddle.Center = point;
            puddle.Water = 0f;
            puddle.IsElectrified = false;
            puddle.PowerCause = null;
            puddle.LastWaterCause = null;
            _active.Add(puddle);
            return puddle;
        }

        private void Release(int index)
        {
            Puddle puddle = _active[index];
            _pool.Despawn(puddle.View);
            puddle.View = null;
            _active.RemoveAt(index);
            _free.Push(puddle);
        }

        private float RadiusFor(float water) =>
            Mathf.Clamp(Mathf.Sqrt(water) * _config.RadiusPerSqrtWater, _config.MinRadius, _config.MaxRadius);
    }
}
