using UnityEngine;

namespace FB.Game.Puddles
{
    [CreateAssetMenu(menuName = "FB/Puddle Config", fileName = "PuddleConfig")]
    public sealed class PuddleConfig : ScriptableObject
    {
        [Header("Detection")]
        [Tooltip("Water hits below this height with an upward normal count as floor hits.")]
        [SerializeField, Min(0f)] private float _floorMaxHeight = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _minFloorNormalY = 0.7f;

        [Header("Shape")]
        [SerializeField, Min(1)] private int _maxPuddles = 24;
        [Tooltip("Radius = sqrt(water) * this, clamped.")]
        [SerializeField, Min(0f)] private float _radiusPerSqrtWater = 0.9f;
        [SerializeField, Min(0.05f)] private float _minRadius = 0.25f;
        [SerializeField, Min(0.1f)] private float _maxRadius = 1.2f;
        [Tooltip("New water this close to a puddle edge is added to it.")]
        [SerializeField, Min(0f)] private float _mergeDistance = 0.3f;
        [SerializeField, Min(0f)] private float _dryPerSecond = 0.02f;
        [SerializeField, Min(0f)] private float _minWater = 0.03f;

        [Header("Player")]
        [Tooltip("Ground control while standing in a puddle (1 = dry floor).")]
        [SerializeField, Range(0f, 1f)] private float _slipGroundControl = 0.15f;

        [Header("Visuals")]
        [SerializeField] private Color _waterColor = new Color(0.36f, 0.72f, 0.95f);
        [SerializeField] private Color _electrifiedColor = new Color(0.75f, 0.97f, 1f);

        public float FloorMaxHeight => _floorMaxHeight;
        public float MinFloorNormalY => _minFloorNormalY;
        public int MaxPuddles => _maxPuddles;
        public float RadiusPerSqrtWater => _radiusPerSqrtWater;
        public float MinRadius => _minRadius;
        public float MaxRadius => _maxRadius;
        public float MergeDistance => _mergeDistance;
        public float DryPerSecond => _dryPerSecond;
        public float MinWater => _minWater;
        public float SlipGroundControl => _slipGroundControl;
        public Color WaterColor => _waterColor;
        public Color ElectrifiedColor => _electrifiedColor;
    }
}
