using System.Collections.Generic;
using FB.Core.Pooling;
using FB.Game.Blame;
using FB.Game.Effects;
using FB.Game.Fire;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Civilian
{
    /// <summary>
    /// The neighbour as something that burns: catches fire next to a burning item (unless still wet), carries
    /// pooled flames around while he panics, and goes out when hosed or after a while. The burn cause chains to
    /// the fire he stood next to, so the blame reaches whoever started that fire.
    /// </summary>
    public sealed class CivilianBurning
    {
        private const float CheckInterval = 0.25f;
        private static readonly Bounds FlameBounds = new Bounds(new Vector3(0f, 0.85f, 0f), new Vector3(0.5f, 1.4f, 0.5f));

        private readonly CivilianBody _body;
        private readonly SurfaceRegistry _registry;
        private readonly IPool<FireVfx> _flames;
        private readonly OneShotEffectSpawner<SteamPuff> _steam;
        private readonly ICauseFactory _causes;
        private readonly SignalBus _signalBus;
        private FireVfx _vfx;
        private float _nextCheck;
        private float _burnLeft;
        private float _wetUntil;

        public CivilianBurning(CivilianBody body, SurfaceRegistry registry, IPool<FireVfx> flames,
            OneShotEffectSpawner<SteamPuff> steam, ICauseFactory causes, SignalBus signalBus)
        {
            _body = body;
            _registry = registry;
            _flames = flames;
            _steam = steam;
            _causes = causes;
            _signalBus = signalBus;
        }

        /// <summary>Returns true on the frame he catches fire.</summary>
        public bool Tick(float deltaTime)
        {
            if (_body.IsBurning)
            {
                _burnLeft -= deltaTime;
                if (_burnLeft <= 0f)
                {
                    PutOut();
                }

                return false;
            }

            float now = Time.time;
            if (now < _nextCheck || now < _wetUntil)
            {
                return false;
            }

            _nextCheck = now + CheckInterval;
            if (!TryFindFlames(out Surface fire))
            {
                return false;
            }

            Ignite(fire);
            return true;
        }

        /// <summary>Hose water: puts him out and keeps him fireproof for a few seconds.</summary>
        public void Soak()
        {
            _wetUntil = Time.time + _body.Config.WetSeconds;
            if (_body.IsBurning)
            {
                PutOut();
            }
        }

        public void Dispose()
        {
            if (_vfx != null)
            {
                _flames.Despawn(_vfx);
                _vfx = null;
            }
        }

        private void Ignite(Surface fire)
        {
            Cause cause = _causes.Create(ActorId.Fire, CauseAction.Burn, fire.Statuses.CauseOf(SurfaceStatus.Burning));
            _body.BurnCause = cause;
            _burnLeft = _body.Config.BurnSeconds;

            Transform root = _body.View.transform;
            _vfx = _flames.Spawn();
            Bounds bounds = new Bounds(root.TransformPoint(FlameBounds.center), FlameBounds.size);
            _vfx.Attach(root, bounds);
            _vfx.SetIntensity(0.9f);
            _signalBus.Fire(new CivilianIncidentSignal(CivilianIncident.Ignited, null, _body.Config.BurnDamage, cause));
        }

        private void PutOut()
        {
            _body.BurnCause = null;
            Dispose();
            _steam.TrySpawn(_body.Position + Vector3.up * 1.2f, Vector3.up);
        }

        private bool TryFindFlames(out Surface fire)
        {
            fire = null;
            Vector3 center = _body.Position + Vector3.up * 0.8f;
            float bestSqr = _body.Config.IgniteDistance * _body.Config.IgniteDistance;
            IReadOnlyList<Surface> all = _registry.All;
            for (int i = 0; i < all.Count; i++)
            {
                if (!all[i].IsBurning)
                {
                    continue;
                }

                float sqr = (all[i].Bounds.ClosestPoint(center) - center).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    fire = all[i];
                }
            }

            return fire != null;
        }
    }
}
