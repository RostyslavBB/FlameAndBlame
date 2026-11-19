using FB.Core.Input;
using FB.Game.Config;
using UnityEngine;
using Zenject;

namespace FB.Game.Player
{
    /// <summary>
    /// Force-based movement: steers the horizontal velocity towards the wished velocity with a capped
    /// acceleration. Because braking is capped too, external impulses (recoil, shocks) visibly push the player,
    /// and low <see cref="IPlayerTraction.GroundControl"/> makes the floor slippery.
    /// </summary>
    public sealed class PlayerMovement : ITickable, IFixedTickable
    {
        private readonly PlayerBody _body;
        private readonly PlayerConfig _config;
        private readonly IInputService _input;
        private readonly IPlayerTraction _traction;
        private readonly PlayerGroundProbe _ground;
        private bool _jumpQueued;

        public PlayerMovement(PlayerBody body, PlayerConfig config, IInputService input,
            IPlayerTraction traction, PlayerGroundProbe ground)
        {
            _body = body;
            _config = config;
            _input = input;
            _traction = traction;
            _ground = ground;
        }

        public void Tick()
        {
            // Input is sampled per frame, physics runs at a fixed rate: buffer the press until the next step.
            _jumpQueued |= _input.JumpPressed;
        }

        public void FixedTick()
        {
            Rigidbody rigidbody = _body.Rigidbody;
            _ground.Probe();
            bool grounded = _ground.IsGrounded;

            Vector3 velocity = rigidbody.linearVelocity;
            Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
            Vector3 wished = WishedDirection() * _config.MaxSpeed;
            float control = grounded ? _traction.GroundControl : _config.AirControl;
            float maxAcceleration = _config.Acceleration * control;

            Vector3 acceleration = Vector3.ClampMagnitude((wished - horizontal) / Time.fixedDeltaTime, maxAcceleration);
            rigidbody.AddForce(acceleration, ForceMode.Acceleration);

            if (!grounded)
            {
                rigidbody.AddForce(Physics.gravity * (_config.FallGravityMultiplier - 1f), ForceMode.Acceleration);
            }

            if (_jumpQueued)
            {
                _jumpQueued = false;
                if (grounded)
                {
                    rigidbody.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
                    rigidbody.AddForce(Vector3.up * _config.JumpVelocity, ForceMode.VelocityChange);
                }
            }
        }

        private Vector3 WishedDirection()
        {
            Vector2 move = Vector2.ClampMagnitude(_input.Move, 1f);
            Transform pivot = _body.CameraPivot;
            Vector3 forward = Vector3.ProjectOnPlane(pivot.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(pivot.right, Vector3.up).normalized;
            return forward * move.y + right * move.x;
        }
    }
}
