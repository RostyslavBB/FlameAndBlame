using UnityEngine;

namespace FB.Game.Config
{
    [CreateAssetMenu(menuName = "FB/Round Config", fileName = "RoundConfig")]
    public sealed class RoundConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _briefingSeconds = 3f;
        [SerializeField, Min(1f)] private float _roundSeconds = 150f;

        public float BriefingSeconds => _briefingSeconds;
        public float RoundSeconds => _roundSeconds;
    }
}
