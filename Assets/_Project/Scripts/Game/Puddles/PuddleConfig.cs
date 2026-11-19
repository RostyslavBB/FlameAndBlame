using UnityEngine;

namespace FB.Game.Puddles
{
    [CreateAssetMenu(menuName = "FB/Puddle Config", fileName = "PuddleConfig")]
    public sealed class PuddleConfig : ScriptableObject
    {
        [Header("Grid")]
        [SerializeField, Range(0.05f, 0.3f)] private float _cellSize = 0.1f;
        [SerializeField, Range(5f, 60f)] private float _simulationRate = 30f;

        [Header("Water")]
        [Tooltip("Cubic meters of water per hose water unit.")]
        [SerializeField, Min(0f)] private float _volumePerWater = 0.004f;
        [Tooltip("Share of water that drips to the floor after hitting something else first.")]
        [SerializeField, Range(0f, 1f)] private float _nonFloorShare = 0.85f;
        [Tooltip("Fraction of the depth difference that flows to a neighbour cell per step (stable below 0.25).")]
        [SerializeField, Range(0.01f, 0.24f)] private float _flowRate = 0.2f;
        [Tooltip("Water thinner than this sticks to the floor and stops spreading: it forms the puddle edge.")]
        [SerializeField, Min(0f)] private float _filmThickness = 0.003f;
        [SerializeField, Min(0f)] private float _evaporationPerSecond = 0.00003f;
        [Tooltip("Max water depth (m): the level can rise up to this (about a doorsill).")]
        [SerializeField, Min(0.01f)] private float _maxDepth = 0.12f;

        [Header("Player and objects")]
        [SerializeField, Min(0.0001f)] private float _fullSlipDepth = 0.004f;
        [SerializeField, Range(0f, 1f)] private float _slipGroundControl = 0.3f;
        [SerializeField, Min(0.05f)] private float _soakInterval = 0.5f;
        [SerializeField, Min(0f)] private float _soakPerSecond = 0.25f;

        [Header("Ripples")]
        [SerializeField, Range(1, 8)] private int _maxRipples = 8;
        [SerializeField, Min(0.02f)] private float _rippleInterval = 0.12f;

        public float CellSize => _cellSize;
        public float SimulationRate => _simulationRate;
        public float VolumePerWater => _volumePerWater;
        public float NonFloorShare => _nonFloorShare;
        public float FlowRate => _flowRate;
        public float FilmThickness => _filmThickness;
        public float EvaporationPerSecond => _evaporationPerSecond;
        public float MaxDepth => _maxDepth;
        public float FullSlipDepth => _fullSlipDepth;
        public float SlipGroundControl => _slipGroundControl;
        public float SoakInterval => _soakInterval;
        public float SoakPerSecond => _soakPerSecond;
        public int MaxRipples => _maxRipples;
        public float RippleInterval => _rippleInterval;
    }
}
