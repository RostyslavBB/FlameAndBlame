using FB.Game.Config;
using UnityEngine;

namespace FB.Game.Player
{
    /// <summary>Sphere-cast ground check without allocations; ignores the player's own collider.</summary>
    public sealed class PlayerGroundProbe
    {
        private const int MaxHits = 8;

        private readonly PlayerBody _body;
        private readonly PlayerConfig _config;
        private readonly RaycastHit[] _hits = new RaycastHit[MaxHits];

        public PlayerGroundProbe(PlayerBody body, PlayerConfig config)
        {
            _body = body;
            _config = config;
        }

        public bool IsGrounded { get; private set; }

        public void Probe()
        {
            float radius = _body.Collider.radius;
            Vector3 origin = _body.Rigidbody.position + Vector3.up * (radius + 0.05f);
            int count = Physics.SphereCastNonAlloc(origin, radius * 0.9f, Vector3.down, _hits,
                _config.GroundCheckDistance + 0.05f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            IsGrounded = false;
            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = _hits[i];
                if (hit.collider.attachedRigidbody == _body.Rigidbody)
                {
                    continue;
                }

                if (hit.normal.y >= _config.MinGroundNormalY)
                {
                    IsGrounded = true;
                    return;
                }
            }
        }
    }
}
