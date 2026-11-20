using UnityEngine;

namespace FB.Game.Smoke
{
    [CreateAssetMenu(menuName = "FB/Smoke Config", fileName = "SmokeConfig")]
    public sealed class SmokeConfig : ScriptableObject
    {
        [Header("Amount (0..1)")]
        [Tooltip("Smoke added per second for every burning object.")]
        [SerializeField, Min(0f)] private float _perFirePerSecond = 0.014f;
        [Tooltip("Smoke cleared per second (vents, open doorways).")]
        [SerializeField, Min(0f)] private float _clearPerSecond = 0.012f;
        [Tooltip("Faster clearing once every fire is out.")]
        [SerializeField, Min(0f)] private float _clearWhenOutPerSecond = 0.03f;
        [SerializeField, Min(0.1f)] private float _visualLerpSpeed = 1.5f;

        [Header("Ceiling layer")]
        [Tooltip("Height of the lowest smoke layer when there is almost no smoke.")]
        [SerializeField] private float _layerTopY = 2.55f;
        [Tooltip("Height of the lowest smoke layer when the apartment is full of smoke (just above the eyes).")]
        [SerializeField] private float _layerBottomY = 1.85f;

        [Tooltip("Ceiling smoke puffs per second at full smoke (soft billboards under the ceiling).")]
        [SerializeField, Min(0f)] private float _maxPuffsPerSecond = 5f;

        [Header("Haze")]
        [SerializeField, Min(0f)] private float _maxFogDensity = 0.09f;
        [SerializeField] private Color _fogColor = new Color(0.34f, 0.31f, 0.3f);

        public float PerFirePerSecond => _perFirePerSecond;
        public float ClearPerSecond => _clearPerSecond;
        public float ClearWhenOutPerSecond => _clearWhenOutPerSecond;
        public float VisualLerpSpeed => _visualLerpSpeed;
        public float LayerTopY => _layerTopY;
        public float LayerBottomY => _layerBottomY;
        public float MaxPuffsPerSecond => _maxPuffsPerSecond;
        public float MaxFogDensity => _maxFogDensity;
        public Color FogColor => _fogColor;
    }
}
