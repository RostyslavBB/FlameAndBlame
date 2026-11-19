using System.Collections.Generic;
using FB.Game.Blame;
using FB.Game.CameraFx;
using FB.Game.Player;
using FB.Game.Puddles;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Power
{
    /// <summary>
    /// Anyone standing in an electrified puddle gets shocked: impulse, camera shake, an arc and an
    /// ElectrocutionSignal whose cause joins the puddle (water) and the power source. Jumping avoids it.
    /// </summary>
    public sealed class ShockService : ITickable
    {
        private const float FloorContactHeight = 0.3f;

        private readonly IPuddleField _puddles;
        private readonly SurfaceRegistry _surfaces;
        private readonly PlayerBody _player;
        private readonly PlayerGroundProbe _ground;
        private readonly IKnockbackReceiver _knockback;
        private readonly ICameraShake _shake;
        private readonly ArcSpawner _arcs;
        private readonly ICauseFactory _causes;
        private readonly SignalBus _signalBus;
        private readonly PowerConfig _config;
        private float _nextPlayerShock;
        private float[] _nextSurfaceShock;
        private Rigidbody[] _bodies;

        public ShockService(IPuddleField puddles, SurfaceRegistry surfaces, PlayerBody player, PlayerGroundProbe ground,
            IKnockbackReceiver knockback, ICameraShake shake, ArcSpawner arcs, ICauseFactory causes,
            SignalBus signalBus, PowerConfig config)
        {
            _puddles = puddles;
            _surfaces = surfaces;
            _player = player;
            _ground = ground;
            _knockback = knockback;
            _shake = shake;
            _arcs = arcs;
            _causes = causes;
            _signalBus = signalBus;
            _config = config;
        }

        public void Tick()
        {
            float now = Time.time;
            TryShockPlayer(now);
            TryShockSurfaces(now);
        }

        private void TryShockPlayer(float now)
        {
            if (now < _nextPlayerShock || !_ground.IsGrounded)
            {
                return;
            }

            Vector3 feet = _player.Rigidbody.position;
            Puddle puddle = _puddles.FindAt(feet);
            if (puddle == null || !puddle.IsElectrified)
            {
                return;
            }

            _nextPlayerShock = now + _config.ShockCooldown;
            Vector2 side = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(side.x, 0f, side.y) * (1f - _config.ShockUpShare) + Vector3.up * _config.ShockUpShare;
            _knockback.ApplyImpulse(direction.normalized * _config.PlayerShockImpulse);
            _shake.AddTrauma(_config.ShockTrauma);
            _arcs.Spawn(puddle.Center, feet + Vector3.up * 1.1f);
            _signalBus.Fire(new ElectrocutionSignal(true, null, feet, _config.PlayerShockDamage, ShockCause(puddle)));
        }

        private void TryShockSurfaces(float now)
        {
            IReadOnlyList<Surface> surfaces = _surfaces.All;
            CacheBodies(surfaces);
            for (int i = 0; i < surfaces.Count; i++)
            {
                Surface surface = surfaces[i];
                Rigidbody body = _bodies[i];
                if (body == null || now < _nextSurfaceShock[i])
                {
                    continue;
                }

                Bounds bounds = surface.Bounds;
                if (bounds.min.y > FloorContactHeight)
                {
                    continue;
                }

                Puddle puddle = FindElectrifiedTouching(bounds);
                if (puddle == null)
                {
                    continue;
                }

                _nextSurfaceShock[i] = now + _config.ShockCooldown * 2f;
                body.AddForce(Vector3.up * _config.ObjectShockImpulse, ForceMode.Impulse);

                _arcs.Spawn(puddle.Center, bounds.center);
                _signalBus.Fire(new ElectrocutionSignal(false, surface, bounds.center, 0, ShockCause(puddle)));
            }
        }

        // Only loose props (chairs, lamp, table) visibly jump; static furniture is ignored.
        private void CacheBodies(IReadOnlyList<Surface> surfaces)
        {
            if (_bodies != null)
            {
                return;
            }

            _bodies = new Rigidbody[surfaces.Count];
            _nextSurfaceShock = new float[surfaces.Count];
            for (int i = 0; i < surfaces.Count; i++)
            {
                _bodies[i] = surfaces[i].Root.GetComponent<Rigidbody>();
            }
        }

        private Puddle FindElectrifiedTouching(Bounds bounds)
        {
            IReadOnlyList<Puddle> puddles = _puddles.Active;
            for (int i = 0; i < puddles.Count; i++)
            {
                if (puddles[i].IsElectrified && puddles[i].Touches(bounds, 0f))
                {
                    return puddles[i];
                }
            }

            return null;
        }

        private Cause ShockCause(Puddle puddle) =>
            _causes.Create(ActorId.Building, CauseAction.Electrocution, puddle.Cause, puddle.PowerCause);
    }
}
