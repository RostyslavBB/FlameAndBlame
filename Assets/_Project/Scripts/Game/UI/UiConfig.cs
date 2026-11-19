using UnityEngine;

namespace FB.Game.UI
{
    [CreateAssetMenu(menuName = "FB/UI Config", fileName = "UiConfig")]
    public sealed class UiConfig : ScriptableObject
    {
        [Header("Blame feed")]
        [SerializeField, Range(1, 10)] private int _feedMaxEntries = 5;
        [SerializeField, Min(0.5f)] private float _feedEntrySeconds = 7f;
        [SerializeField, Min(0.01f)] private float _feedPopSeconds = 0.15f;
        [SerializeField, Min(0.01f)] private float _feedFadeSeconds = 0.4f;
        [SerializeField, Min(0.01f)] private float _feedShakeSeconds = 0.35f;
        [SerializeField, Min(0f)] private float _feedShakePixels = 14f;
        [SerializeField, Range(1, 3)] private int _feedShakeMinSeverity = 2;

        [Header("Report")]
        [SerializeField, Min(0.01f)] private float _reportFadeSeconds = 0.3f;
        [SerializeField, Min(0.01f)] private float _counterSeconds = 1.4f;
        [SerializeField, Min(0f)] private float _stampDelaySeconds = 0.4f;
        [SerializeField, Min(0.01f)] private float _stampSeconds = 0.22f;

        [Header("HUD")]
        [SerializeField] private Color _pressureColor = new Color(0.45f, 0.8f, 1f);
        [SerializeField] private Color _pressureMaxColor = new Color(1f, 0.3f, 0.25f);

        public int FeedMaxEntries => _feedMaxEntries;
        public float FeedEntrySeconds => _feedEntrySeconds;
        public float FeedPopSeconds => _feedPopSeconds;
        public float FeedFadeSeconds => _feedFadeSeconds;
        public float FeedShakeSeconds => _feedShakeSeconds;
        public float FeedShakePixels => _feedShakePixels;
        public int FeedShakeMinSeverity => _feedShakeMinSeverity;
        public float ReportFadeSeconds => _reportFadeSeconds;
        public float CounterSeconds => _counterSeconds;
        public float StampDelaySeconds => _stampDelaySeconds;
        public float StampSeconds => _stampSeconds;
        public Color PressureColor => _pressureColor;
        public Color PressureMaxColor => _pressureMaxColor;
    }
}
