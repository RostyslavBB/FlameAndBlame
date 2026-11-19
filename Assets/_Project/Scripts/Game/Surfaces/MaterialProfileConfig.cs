using UnityEngine;

namespace FB.Game.Surfaces
{
    /// <summary>How a material reacts to fire and water. One asset per <see cref="MaterialKind"/>.</summary>
    [CreateAssetMenu(menuName = "FB/Material Profile", fileName = "MP_Material")]
    public sealed class MaterialProfileConfig : ScriptableObject
    {
        [SerializeField] private MaterialKind _kind;
        [SerializeField] private bool _flammable = true;
        [Tooltip("Heat at which the surface catches fire.")]
        [SerializeField, Min(0.01f)] private float _ignitionHeat = 1f;
        [Tooltip("Multiplier of heat received from burning neighbours.")]
        [SerializeField, Min(0f)] private float _flammability = 1f;
        [Tooltip("Seconds of burning before the surface is ruined (fuel).")]
        [SerializeField, Min(0.1f)] private float _burnSeconds = 20f;
        [Tooltip("Price in $ when a prop does not define its own.")]
        [SerializeField, Min(0)] private int _defaultValue = 100;
        [Tooltip("Can water alone ruin this material (see water damage rules).")]
        [SerializeField] private bool _waterSensitive;

        public MaterialKind Kind => _kind;
        public bool Flammable => _flammable;
        public float IgnitionHeat => _ignitionHeat;
        public float Flammability => _flammability;
        public float BurnSeconds => _burnSeconds;
        public int DefaultValue => _defaultValue;
        public bool WaterSensitive => _waterSensitive;

#if UNITY_EDITOR
        public void EditorSetup(MaterialKind kind, bool flammable, float ignitionHeat, float flammability,
            float burnSeconds, int defaultValue, bool waterSensitive)
        {
            _kind = kind;
            _flammable = flammable;
            _ignitionHeat = ignitionHeat;
            _flammability = flammability;
            _burnSeconds = burnSeconds;
            _defaultValue = defaultValue;
            _waterSensitive = waterSensitive;
        }
#endif
    }
}
