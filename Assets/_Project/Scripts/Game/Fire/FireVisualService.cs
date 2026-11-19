using System;
using System.Collections.Generic;
using FB.Core.Pooling;
using FB.Game.Effects;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Fire
{
    /// <summary>
    /// Observer of fire signals: pooled flames per burning surface, a budget of point lights (WebGL),
    /// steam when a fire is put out, sparks on short circuits.
    /// </summary>
    public sealed class FireVisualService : IInitializable, IDisposable, ITickable
    {
        private readonly SignalBus _signalBus;
        private readonly IPool<FireVfx> _pool;
        private readonly OneShotEffectSpawner<SteamPuff> _steam;
        private readonly OneShotEffectSpawner<SparkBurst> _sparks;
        private readonly FireConfig _config;
        private readonly SurfaceRegistry _surfaces;
        private readonly Dictionary<ISurface, FireVfx> _active;
        private int _lightsInUse;

        public FireVisualService(SignalBus signalBus, IPool<FireVfx> pool, OneShotEffectSpawner<SteamPuff> steam,
            OneShotEffectSpawner<SparkBurst> sparks, FireConfig config, SurfaceRegistry surfaces)
        {
            _signalBus = signalBus;
            _pool = pool;
            _steam = steam;
            _sparks = sparks;
            _config = config;
            _surfaces = surfaces;
            _active = new Dictionary<ISurface, FireVfx>(config.MaxFires);
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FireSpreadSignal>(OnFireSpread);
            _signalBus.Subscribe<FireExtinguishedSignal>(OnFireExtinguished);
            _signalBus.Subscribe<ItemRuinedSignal>(OnItemRuined);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<FireSpreadSignal>(OnFireSpread);
            _signalBus.Unsubscribe<FireExtinguishedSignal>(OnFireExtinguished);
            _signalBus.Unsubscribe<ItemRuinedSignal>(OnItemRuined);
        }

        public void Tick()
        {
            float time = Time.time * 9f;
            foreach (KeyValuePair<ISurface, FireVfx> pair in _active)
            {
                // Fire grows with the heat of what burns: small when it just caught, raging at full heat.
                Surface surface = _surfaces.All[pair.Key.Id];
                float heat = Mathf.Clamp01(surface.Heat / (surface.Profile.IgnitionHeat * _config.MaxHeatFactor));
                pair.Value.SetIntensity(Mathf.Lerp(0.35f, 1.2f, heat));

                if (pair.Value.HasLight)
                {
                    float flicker = 0.75f + 0.5f * Mathf.PerlinNoise(time, pair.Key.Id * 1.37f);
                    pair.Value.SetLightIntensity(_config.FireLightIntensity * flicker);
                }
            }
        }

        private void OnFireSpread(FireSpreadSignal signal)
        {
            if (_active.ContainsKey(signal.Target) || _active.Count >= _config.MaxFires)
            {
                return;
            }

            FireVfx vfx = _pool.Spawn();
            vfx.Attach(signal.Target.Root, signal.Target.Bounds);
            _active.Add(signal.Target, vfx);
            if (_lightsInUse < _config.MaxFireLights)
            {
                vfx.SetLight(true);
                _lightsInUse++;
            }
        }

        private void OnFireExtinguished(FireExtinguishedSignal signal)
        {
            Release(signal.Target);
            _steam.TrySpawn(TopOf(signal.Target), Vector3.up);
        }

        private void OnItemRuined(ItemRuinedSignal signal)
        {
            Release(signal.Entity);
            if (signal.Reason == RuinReason.ShortCircuit)
            {
                _sparks.TrySpawn(TopOf(signal.Entity), Vector3.up);
            }
        }

        private void Release(ISurface surface)
        {
            if (!_active.TryGetValue(surface, out FireVfx vfx))
            {
                return;
            }

            if (vfx.HasLight)
            {
                _lightsInUse--;
            }

            _active.Remove(surface);
            _pool.Despawn(vfx);
            HandOverLight();
        }

        private void HandOverLight()
        {
            foreach (FireVfx vfx in _active.Values)
            {
                if (_lightsInUse >= _config.MaxFireLights)
                {
                    return;
                }

                if (!vfx.HasLight)
                {
                    vfx.SetLight(true);
                    _lightsInUse++;
                }
            }
        }

        private static Vector3 TopOf(ISurface surface)
        {
            Bounds bounds = surface.Bounds;
            return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        }
    }
}
