using FB.Game.Config;
using FB.Game.Player;
using UnityEngine;
using Zenject;

namespace FB.Game.Hose
{
    /// <summary>A hose has a length: past it, a spring pulls the player back towards the tap.</summary>
    public sealed class HoseLeash : IFixedTickable
    {
        private readonly PlayerBody _player;
        private readonly IKnockbackReceiver _knockback;
        private readonly HoseConfig _config;
        private readonly Transform _anchor;

        public HoseLeash(PlayerBody player, IKnockbackReceiver knockback, HoseConfig config,
            [Inject(Id = HoseIds.Anchor)] Transform anchor)
        {
            _player = player;
            _knockback = knockback;
            _config = config;
            _anchor = anchor;
        }

        public void FixedTick()
        {
            Vector3 toAnchor = _anchor.position - _player.Rigidbody.position;
            toAnchor.y = 0f;
            float distance = toAnchor.magnitude;
            float overshoot = distance - _config.MaxLength;
            if (overshoot <= 0f)
            {
                return;
            }

            Vector3 direction = toAnchor / distance;
            _knockback.ApplyImpulse(direction * (overshoot * _config.LeashStiffness * Time.fixedDeltaTime));
        }
    }
}
