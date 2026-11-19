using UnityEngine;

namespace FB.Game.Civilian
{
    [CreateAssetMenu(menuName = "FB/Civilian Config", fileName = "CivilianConfig")]
    public sealed class CivilianConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField, Min(0.1f)] private float _walkSpeed = 1.5f;
        [SerializeField, Min(0.1f)] private float _panicSpeed = 3.6f;
        [SerializeField] private Vector2 _idleSeconds = new Vector2(0.8f, 2.5f);

        [Header("Panic")]
        [Tooltip("A fire closer than this (m) makes the neighbour panic.")]
        [SerializeField, Min(0.5f)] private float _panicRadius = 3.2f;
        [SerializeField, Min(0.5f)] private float _panicSeconds = 6f;

        [Header("Chaos")]
        [SerializeField, Min(0.1f)] private float _knockRadius = 0.55f;
        [SerializeField, Min(0f)] private float _knockSpeed = 3f;
        [Tooltip("Seconds before the same item can be knocked (and reported) again.")]
        [SerializeField, Min(0.1f)] private float _knockCooldown = 6f;
        [SerializeField, Min(0)] private int _knockDamage = 40;

        [Header("Getting hit")]
        [Tooltip("Hose water (units) that knocks the neighbour off his feet.")]
        [SerializeField, Min(0f)] private float _waterToKnockDown = 0.5f;
        [SerializeField, Min(0.5f)] private float _knockedSeconds = 3f;
        [SerializeField, Min(0)] private int _soakedDamage = 100;
        [SerializeField, Min(0)] private int _shockedDamage = 150;

        public float WalkSpeed => _walkSpeed;
        public float PanicSpeed => _panicSpeed;
        public Vector2 IdleSeconds => _idleSeconds;
        public float PanicRadius => _panicRadius;
        public float PanicSeconds => _panicSeconds;
        public float KnockRadius => _knockRadius;
        public float KnockSpeed => _knockSpeed;
        public float KnockCooldown => _knockCooldown;
        public int KnockDamage => _knockDamage;
        public float WaterToKnockDown => _waterToKnockDown;
        public float KnockedSeconds => _knockedSeconds;
        public int SoakedDamage => _soakedDamage;
        public int ShockedDamage => _shockedDamage;
    }
}
