using UnityEngine;

namespace FB.Game.CameraFx
{
    [CreateAssetMenu(menuName = "FB/Juice Config", fileName = "JuiceConfig")]
    public sealed class JuiceConfig : ScriptableObject
    {
        [Header("Hit-stop + slow-mo on big blame events")]
        [SerializeField, Range(1, 3)] private int _minSeverity = 3;
        [SerializeField, Min(0f)] private float _hitStopSeconds = 0.07f;
        [SerializeField, Range(0f, 1f)] private float _hitStopTimeScale = 0.02f;
        [SerializeField, Min(0f)] private float _slowMoSeconds = 0.3f;
        [SerializeField, Range(0.05f, 1f)] private float _slowMoTimeScale = 0.35f;

        [Header("FOV kick at max hose pressure")]
        [SerializeField, Min(0f)] private float _fovKickDegrees = 8f;
        [SerializeField, Range(0f, 1f)] private float _fovKickFromPressure = 0.85f;
        [SerializeField, Min(0.1f)] private float _fovLerpSpeed = 8f;

        public int MinSeverity => _minSeverity;
        public float HitStopSeconds => _hitStopSeconds;
        public float HitStopTimeScale => _hitStopTimeScale;
        public float SlowMoSeconds => _slowMoSeconds;
        public float SlowMoTimeScale => _slowMoTimeScale;
        public float FovKickDegrees => _fovKickDegrees;
        public float FovKickFromPressure => _fovKickFromPressure;
        public float FovLerpSpeed => _fovLerpSpeed;
    }
}
