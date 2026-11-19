using System;
using System.Collections.Generic;
using FB.Game.Civilian;
using FB.Game.Flow;
using FB.Game.Hose;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Audio
{
    /// <summary>
    /// Observer that gives every gameplay signal a sound (whoomph on ignition, hiss on extinguish, zaps, sparks,
    /// the neighbour's yelps) and drives the two loops: the hose hiss follows the pressure, the fire crackle sits
    /// on the nearest fire and grows with the number of fires.
    /// </summary>
    public sealed class GameSoundEvents : IInitializable, IDisposable, ITickable
    {
        private const float FireScanInterval = 0.25f;

        private readonly IAudioService _audio;
        private readonly SignalBus _signalBus;
        private readonly IHoseFacade _hose;
        private readonly SurfaceRegistry _surfaces;
        private readonly CivilianBody _neighbour;
        private readonly IGameFlow _flow;
        private float _nextFireScan;
        private int _fires;
        private Vector3 _nearestFire;

        public GameSoundEvents(IAudioService audio, SignalBus signalBus, IHoseFacade hose, SurfaceRegistry surfaces,
            CivilianBody neighbour, IGameFlow flow)
        {
            _audio = audio;
            _signalBus = signalBus;
            _hose = hose;
            _surfaces = surfaces;
            _neighbour = neighbour;
            _flow = flow;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FireSpreadSignal>(OnFireSpread);
            _signalBus.Subscribe<FireExtinguishedSignal>(OnFireExtinguished);
            _signalBus.Subscribe<ElectrocutionSignal>(OnElectrocution);
            _signalBus.Subscribe<ItemRuinedSignal>(OnItemRuined);
            _signalBus.Subscribe<CivilianIncidentSignal>(OnCivilianIncident);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<FireSpreadSignal>(OnFireSpread);
            _signalBus.Unsubscribe<FireExtinguishedSignal>(OnFireExtinguished);
            _signalBus.Unsubscribe<ElectrocutionSignal>(OnElectrocution);
            _signalBus.Unsubscribe<ItemRuinedSignal>(OnItemRuined);
            _signalBus.Unsubscribe<CivilianIncidentSignal>(OnCivilianIncident);
        }

        public void Tick()
        {
            bool playing = _flow.Phase == GamePhase.Playing;
            float pressure = playing ? _hose.Pressure : 0f;
            _audio.SetLoop(SoundId.HoseLoop, pressure, 0.85f + pressure * 0.3f, _hose.NozzlePosition);

            if (Time.unscaledTime >= _nextFireScan)
            {
                _nextFireScan = Time.unscaledTime + FireScanInterval;
                ScanFires(_hose.NozzlePosition);
            }

            float fireVolume = playing && _fires > 0 ? Mathf.Min(1f, 0.45f + _fires * 0.12f) : 0f;
            _audio.SetLoop(SoundId.FireLoop, fireVolume, 1f, _nearestFire);
        }

        private void ScanFires(Vector3 listener)
        {
            _fires = 0;
            float best = float.MaxValue;
            IReadOnlyList<Surface> all = _surfaces.All;
            for (int i = 0; i < all.Count; i++)
            {
                if (!all[i].IsBurning)
                {
                    continue;
                }

                _fires++;
                Vector3 position = all[i].Root.position;
                float distance = (position - listener).sqrMagnitude;
                if (distance < best)
                {
                    best = distance;
                    _nearestFire = position;
                }
            }
        }

        private void OnFireSpread(FireSpreadSignal signal) =>
            _audio.Play(SoundId.Ignite, signal.Target.Root.position);

        private void OnFireExtinguished(FireExtinguishedSignal signal) =>
            _audio.Play(SoundId.Steam, signal.Target.Root.position);

        private void OnElectrocution(ElectrocutionSignal signal)
        {
            if (signal.IsPlayer)
            {
                _audio.PlayUi(SoundId.Zap);
            }
            else
            {
                _audio.Play(SoundId.Zap, signal.Point, 0.8f);
            }
        }

        private void OnItemRuined(ItemRuinedSignal signal)
        {
            Vector3 position = signal.Entity.Root.position;
            _audio.Play(signal.Reason == RuinReason.ShortCircuit ? SoundId.Spark : SoundId.Impact, position, 1f, 0.7f);
        }

        private void OnCivilianIncident(CivilianIncidentSignal signal)
        {
            Vector3 position = _neighbour.Position + Vector3.up;
            switch (signal.Incident)
            {
                case CivilianIncident.Shocked:
                    _audio.Play(SoundId.Zap, position);
                    break;
                case CivilianIncident.Ignited:
                    _audio.Play(SoundId.Ignite, position);
                    break;
            }
        }
    }
}
