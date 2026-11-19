using UnityEngine;

namespace FB.Game.Config
{
    [CreateAssetMenu(menuName = "FB/Round Config", fileName = "RoundConfig")]
    public sealed class RoundConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _briefingSeconds = 5f;
        [SerializeField, Min(1f)] private float _roundSeconds = 150f;
        [Tooltip("Pause after the last fire went out before the report, so the steam can be seen.")]
        [SerializeField, Min(0f)] private float _allClearDelay = 1.5f;

        [Header("Briefing")]
        [SerializeField] private string _briefingTitle = "ВИКЛИК";
        [SerializeField, TextArea] private string _briefingText = "Горить кухня. Спробуйте не зробити гірше.";

        public float BriefingSeconds => _briefingSeconds;
        public float RoundSeconds => _roundSeconds;
        public float AllClearDelay => _allClearDelay;
        public string BriefingTitle => _briefingTitle;
        public string BriefingText => _briefingText;
    }
}
