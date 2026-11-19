using UnityEngine;

namespace FB.Game.Config
{
    [CreateAssetMenu(menuName = "FB/Player Config", fileName = "PlayerConfig")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float _maxSpeed = 5f;
        [Tooltip("Max acceleration (m/s^2) used to reach the wished velocity. Also acts as braking.")]
        [SerializeField, Min(0f)] private float _acceleration = 45f;
        [Tooltip("Share of acceleration available in the air (0..1).")]
        [SerializeField, Range(0f, 1f)] private float _airControl = 0.2f;
        [SerializeField, Min(0f)] private float _jumpVelocity = 5.2f;
        [Tooltip("Extra gravity multiplier while airborne, makes jumps less floaty.")]
        [SerializeField, Min(1f)] private float _fallGravityMultiplier = 1.6f;

        [Header("Ground check")]
        [SerializeField, Min(0f)] private float _groundCheckDistance = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _minGroundNormalY = 0.6f;

        [Header("Look")]
        [Tooltip("Degrees per pixel of mouse movement.")]
        [SerializeField, Min(0f)] private float _mouseSensitivity = 0.12f;
        [Tooltip("Extra multiplier applied only in WebGL builds (browsers report different deltas).")]
        [SerializeField, Min(0f)] private float _webGlSensitivityMultiplier = 1f;
        [SerializeField, Range(10f, 89f)] private float _pitchLimit = 85f;

        [Header("Camera shake")]
        [SerializeField, Min(0f)] private float _shakeMaxAngle = 2.5f;
        [SerializeField, Min(0f)] private float _shakeMaxOffset = 0.04f;
        [Tooltip("Trauma lost per second (trauma is 0..1).")]
        [SerializeField, Min(0f)] private float _shakeDecay = 1.6f;
        [SerializeField, Min(0f)] private float _shakeFrequency = 22f;

        public float ShakeMaxAngle => _shakeMaxAngle;
        public float ShakeMaxOffset => _shakeMaxOffset;
        public float ShakeDecay => _shakeDecay;
        public float ShakeFrequency => _shakeFrequency;

        public float MaxSpeed => _maxSpeed;
        public float Acceleration => _acceleration;
        public float AirControl => _airControl;
        public float JumpVelocity => _jumpVelocity;
        public float FallGravityMultiplier => _fallGravityMultiplier;
        public float GroundCheckDistance => _groundCheckDistance;
        public float MinGroundNormalY => _minGroundNormalY;
        public float PitchLimit => _pitchLimit;

        public float MouseSensitivity => Application.platform == RuntimePlatform.WebGLPlayer
            ? _mouseSensitivity * _webGlSensitivityMultiplier
            : _mouseSensitivity;
    }
}
