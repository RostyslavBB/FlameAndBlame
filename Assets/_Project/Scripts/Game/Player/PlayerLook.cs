using FB.Core.Input;
using FB.Game.Config;
using UnityEngine;
using Zenject;

namespace FB.Game.Player
{
    /// <summary>
    /// Mouse look. Only the camera pivot rotates; the rigidbody keeps frozen rotation, so physics stays stable.
    /// </summary>
    public sealed class PlayerLook : ITickable
    {
        private readonly PlayerBody _body;
        private readonly PlayerConfig _config;
        private readonly IInputService _input;
        private readonly ICursorService _cursor;
        private readonly IPlayerControl _control;
        private float _yaw;
        private float _pitch;

        public PlayerLook(PlayerBody body, PlayerConfig config, IInputService input, ICursorService cursor,
            IPlayerControl control)
        {
            _body = body;
            _config = config;
            _input = input;
            _cursor = cursor;
            _control = control;
        }

        public void Tick()
        {
            if (!_cursor.IsLocked || !_control.Enabled)
            {
                return;
            }

            Vector2 delta = _input.Look * _config.MouseSensitivity;
            _yaw += delta.x;
            _pitch = Mathf.Clamp(_pitch - delta.y, -_config.PitchLimit, _config.PitchLimit);
            _body.CameraPivot.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}
