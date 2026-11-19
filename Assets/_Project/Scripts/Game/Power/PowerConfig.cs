using UnityEngine;

namespace FB.Game.Power
{
    [CreateAssetMenu(menuName = "FB/Power Config", fileName = "PowerConfig")]
    public sealed class PowerConfig : ScriptableObject
    {
        [Header("Sources")]
        [Tooltip("Prop id that is always live while the breaker is on.")]
        [SerializeField] private string _powerSourceId = "PowerStrip";
        [Tooltip("Water depth (m) a floor cell needs to conduct (thin film does not).")]
        [SerializeField, Min(0f)] private float _wetDepth = 0.002f;
        [SerializeField, Range(1f, 30f)] private float _tickRate = 5f;
        [SerializeField, Min(0.5f)] private float _breakerReach = 1.8f;

        [Header("Shock")]
        [SerializeField, Min(0f)] private float _shockCooldown = 1.2f;
        [Tooltip("Impulse (N*s) applied to the player; 70 kg body.")]
        [SerializeField, Min(0f)] private float _playerShockImpulse = 420f;
        [SerializeField, Range(0f, 1f)] private float _shockUpShare = 0.6f;
        [SerializeField, Range(0f, 1f)] private float _shockTrauma = 0.8f;
        [Tooltip("Medical bill in $ for each shock of the player.")]
        [SerializeField, Min(0)] private int _playerShockDamage = 150;

        [Header("Arc")]
        [SerializeField, Min(1)] private int _maxArcs = 4;
        [SerializeField, Min(0.05f)] private float _arcLifetime = 0.3f;
        [SerializeField, Min(0f)] private float _arcJitter = 0.12f;

        public string PowerSourceId => _powerSourceId;
        public float WetDepth => _wetDepth;
        public float TickRate => _tickRate;
        public float BreakerReach => _breakerReach;
        public float ShockCooldown => _shockCooldown;
        public float PlayerShockImpulse => _playerShockImpulse;
        public float ShockUpShare => _shockUpShare;
        public float ShockTrauma => _shockTrauma;
        public int PlayerShockDamage => _playerShockDamage;
        public int MaxArcs => _maxArcs;
        public float ArcLifetime => _arcLifetime;
        public float ArcJitter => _arcJitter;
    }
}
