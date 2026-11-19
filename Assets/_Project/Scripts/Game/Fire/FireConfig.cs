using UnityEngine;

namespace FB.Game.Fire
{
    [CreateAssetMenu(menuName = "FB/Fire Config", fileName = "FireConfig")]
    public sealed class FireConfig : ScriptableObject
    {
        [Header("Simulation")]
        [SerializeField, Range(1f, 30f)] private float _tickRate = 10f;
        [Tooltip("Heat per second a burning surface gives to a neighbour at falloff 1.")]
        [SerializeField, Min(0f)] private float _heatPerSecond = 0.55f;
        [Tooltip("Max gap (m) between bounds for heat to travel.")]
        [SerializeField, Min(0.1f)] private float _spreadRange = 1.4f;
        [SerializeField, Min(0f)] private float _heatDecayPerSecond = 0.04f;
        [Tooltip("Burning surfaces regain heat (re-flare) while not watered.")]
        [SerializeField, Min(0f)] private float _burningRegrowPerSecond = 0.45f;
        [Tooltip("Max heat of a burning surface, as a multiple of its ignition heat.")]
        [SerializeField, Min(1f)] private float _maxHeatFactor = 2f;

        [Header("Water")]
        [SerializeField, Min(0f)] private float _dryPerSecond = 0.05f;
        [Tooltip("Water level above which a surface counts as Wet.")]
        [SerializeField, Min(0f)] private float _wetThreshold = 0.1f;
        [Tooltip("Water hitting near a burning object also cools it (radius in m).")]
        [SerializeField, Min(0f)] private float _waterSplashRadius = 0.6f;

        [Header("Round start")]
        [Tooltip("Prop id (GameObject name) that catches fire when the round starts.")]
        [SerializeField] private string _startSurfaceId = "Pan";

        [Header("Visuals")]
        [SerializeField, Min(1)] private int _maxFires = 30;
        [SerializeField, Range(0, 8)] private int _maxFireLights = 4;
        [SerializeField, Min(0f)] private float _fireLightIntensity = 2.2f;
        [SerializeField, Min(1)] private int _maxSteamPuffs = 6;
        [SerializeField, Min(1)] private int _maxSparkBursts = 4;
        [SerializeField, Min(0.1f)] private float _oneShotLifetime = 1.5f;

        [Header("Surface tint (color multipliers)")]
        [SerializeField, Range(0f, 1f)] private float _wetTint = 0.6f;
        [SerializeField, Range(0f, 1f)] private float _charredTint = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _brokenTint = 0.5f;

        public float WetTint => _wetTint;
        public float CharredTint => _charredTint;
        public float BrokenTint => _brokenTint;

        public float TickRate => _tickRate;
        public float HeatPerSecond => _heatPerSecond;
        public float SpreadRange => _spreadRange;
        public float HeatDecayPerSecond => _heatDecayPerSecond;
        public float BurningRegrowPerSecond => _burningRegrowPerSecond;
        public float MaxHeatFactor => _maxHeatFactor;
        public float DryPerSecond => _dryPerSecond;
        public float WetThreshold => _wetThreshold;
        public float WaterSplashRadius => _waterSplashRadius;
        public string StartSurfaceId => _startSurfaceId;
        public int MaxFires => _maxFires;
        public int MaxFireLights => _maxFireLights;
        public float FireLightIntensity => _fireLightIntensity;
        public int MaxSteamPuffs => _maxSteamPuffs;
        public int MaxSparkBursts => _maxSparkBursts;
        public float OneShotLifetime => _oneShotLifetime;
    }
}
