using UnityEngine;

namespace FB.Game.Config
{
    [CreateAssetMenu(menuName = "FB/Hose Config", fileName = "HoseConfig")]
    public sealed class HoseConfig : ScriptableObject
    {
        [Header("Pressure")]
        [Tooltip("Pressure (0..1) over normalized hold time (0..1 = 0..RampSeconds).")]
        [SerializeField] private AnimationCurve _pressureCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0.4f), new Keyframe(0.6f, 0.55f), new Keyframe(1f, 1f, 2f, 0f));
        [SerializeField, Min(0.1f)] private float _rampSeconds = 1.6f;
        [Tooltip("Pressure lost per second after releasing the trigger.")]
        [SerializeField, Min(0.1f)] private float _decayPerSecond = 3f;

        [Header("Jet")]
        [SerializeField, Min(0f)] private float _maxEmissionRate = 300f;
        [SerializeField, Min(0f)] private float _minJetSpeed = 5f;
        [SerializeField, Min(0f)] private float _maxJetSpeed = 17f;
        [SerializeField, Range(0f, 15f)] private float _spreadAngle = 2.5f;
        [SerializeField, Min(16)] private int _maxParticles = 600;
        [SerializeField, Min(0.1f)] private float _particleLifetime = 1.4f;
        [SerializeField, Min(0f)] private float _particleGravity = 0.6f;
        [SerializeField, Min(0.01f)] private float _particleSize = 0.07f;
        [Tooltip("Mist particles per second at full pressure (fire-hose spray around the jet).")]
        [SerializeField, Min(0f)] private float _maxMistRate = 45f;

        [Header("Recoil")]
        [Tooltip("Share of max recoil (0..1) by pressure (0..1). Steep at the end: max pressure overpowers the legs.")]
        [SerializeField] private AnimationCurve _recoilCurve = new AnimationCurve(
            new Keyframe(0f, 0f), new Keyframe(0.7f, 0.2f), new Keyframe(1f, 1f, 3f, 0f));
        [Tooltip("Newtons at full pressure. Player legs resist ~mass * acceleration (70 kg * 45 = 3150 N).")]
        [SerializeField, Min(0f)] private float _maxRecoilForce = 3900f;
        [SerializeField, Range(0f, 1f)] private float _verticalRecoilScale = 0.4f;
        [Tooltip("Camera trauma added per second at full pressure.")]
        [SerializeField, Min(0f)] private float _shakePerSecond = 1.4f;

        [Header("Hose")]
        [SerializeField, Min(1f)] private float _maxLength = 18f;
        [Tooltip("Pull-back force in Newtons per meter over the max length.")]
        [SerializeField, Min(0f)] private float _leashStiffness = 2500f;
        [Tooltip("Visual hose length as a multiple of the leash length: slack to go around door corners.")]
        [SerializeField, Range(1f, 2f)] private float _ropeSlack = 1.3f;
        [SerializeField, Range(4, 64)] private int _ropePoints = 60;
        [SerializeField, Min(0.01f)] private float _ropeWidth = 0.1f;
        [SerializeField, Range(0.8f, 1f)] private float _ropeDamping = 0.97f;
        [SerializeField, Range(1, 30)] private int _ropeIterations = 16;

        [Header("Water effects")]
        [SerializeField, Min(0f)] private float _waterPerParticle = 0.02f;
        [Tooltip("Impulse per particle hit, multiplied by particle velocity.")]
        [SerializeField, Min(0f)] private float _pushPerParticle = 0.02f;
        [SerializeField, Min(1)] private int _maxActiveSplashes = 12;
        [SerializeField, Min(0.05f)] private float _splashLifetime = 0.6f;
        [SerializeField, Min(0f)] private float _splashCooldown = 0.05f;

        public AnimationCurve PressureCurve => _pressureCurve;
        public float RampSeconds => _rampSeconds;
        public float DecayPerSecond => _decayPerSecond;
        public float MaxEmissionRate => _maxEmissionRate;
        public float MinJetSpeed => _minJetSpeed;
        public float MaxJetSpeed => _maxJetSpeed;
        public float SpreadAngle => _spreadAngle;
        public int MaxParticles => _maxParticles;
        public float ParticleLifetime => _particleLifetime;
        public float ParticleGravity => _particleGravity;
        public float ParticleSize => _particleSize;
        public float MaxMistRate => _maxMistRate;
        public AnimationCurve RecoilCurve => _recoilCurve;
        public float MaxRecoilForce => _maxRecoilForce;
        public float VerticalRecoilScale => _verticalRecoilScale;
        public float ShakePerSecond => _shakePerSecond;
        public float MaxLength => _maxLength;
        public float LeashStiffness => _leashStiffness;
        public float RopeSlack => _ropeSlack;
        public int RopePoints => _ropePoints;
        public float RopeWidth => _ropeWidth;
        public float RopeDamping => _ropeDamping;
        public int RopeIterations => _ropeIterations;
        public float WaterPerParticle => _waterPerParticle;
        public float PushPerParticle => _pushPerParticle;
        public int MaxActiveSplashes => _maxActiveSplashes;
        public float SplashLifetime => _splashLifetime;
        public float SplashCooldown => _splashCooldown;
    }
}
