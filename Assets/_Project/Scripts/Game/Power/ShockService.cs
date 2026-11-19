using FB.Game.Blame;
using FB.Game.CameraFx;
using FB.Game.Player;
using FB.Game.Puddles;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Power
{
    /// <summary>
    /// Electricity only hurts the player: standing in a live puddle gives an impulse, camera shake, an arc and an
    /// ElectrocutionSignal whose cause joins the puddle under the feet (water) and the live power source.
    /// Jumping avoids it.
    /// </summary>
    public sealed class ShockService : ITickable
    {
        private readonly IPuddles _puddles;
        private readonly IElectricity _electricity;
        private readonly PlayerBody _player;
        private readonly PlayerGroundProbe _ground;
        private readonly IKnockbackReceiver _knockback;
        private readonly ICameraShake _shake;
        private readonly ArcSpawner _arcs;
        private readonly ICauseFactory _causes;
        private readonly SignalBus _signalBus;
        private readonly PowerConfig _config;
        private float _nextShock;

        public ShockService(IPuddles puddles, IElectricity electricity, PlayerBody player, PlayerGroundProbe ground,
            IKnockbackReceiver knockback, ICameraShake shake, ArcSpawner arcs, ICauseFactory causes,
            SignalBus signalBus, PowerConfig config)
        {
            _puddles = puddles;
            _electricity = electricity;
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
            Vector3 feet = _player.Rigidbody.position;
            if (now < _nextShock || !_ground.IsGrounded || !_electricity.IsElectrifiedAt(feet))
            {
                return;
            }

            _nextShock = now + _config.ShockCooldown;
            Vector2 side = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(side.x, 0f, side.y) * (1f - _config.ShockUpShare) +
                                Vector3.up * _config.ShockUpShare;
            _knockback.ApplyImpulse(direction.normalized * _config.PlayerShockImpulse);
            _shake.AddTrauma(_config.ShockTrauma);

            Vector2 offset = Random.insideUnitCircle * 0.8f;
            _arcs.Spawn(new Vector3(feet.x + offset.x, 0.01f, feet.z + offset.y), feet + Vector3.up * 1.1f);

            Cause cause = _causes.Create(ActorId.Building, CauseAction.Electrocution, _puddles.CauseAt(feet),
                _electricity.PowerCause);
            _signalBus.Fire(new ElectrocutionSignal(true, null, feet, _config.PlayerShockDamage, cause));
        }
    }
}
