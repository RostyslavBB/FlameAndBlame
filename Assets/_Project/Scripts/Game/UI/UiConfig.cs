using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.UI
{
    [CreateAssetMenu(menuName = "FB/UI Config", fileName = "UiConfig")]
    public sealed class UiConfig : ScriptableObject
    {
        [Header("Blame feed")]
        [SerializeField, Range(1, 10)] private int _feedMaxEntries = 3;
        [SerializeField, Min(0.5f)] private float _feedEntrySeconds = 5f;
        [SerializeField, Min(0.01f)] private float _feedPopSeconds = 0.35f;
        [SerializeField, Min(0.01f)] private float _feedFadeSeconds = 0.4f;
        [SerializeField, Min(0.01f)] private float _feedShakeSeconds = 0.35f;
        [SerializeField, Min(0f)] private float _feedShakePixels = 14f;
        [SerializeField, Range(1, 3)] private int _feedShakeMinSeverity = 3;
        [Tooltip("Lines below this severity (small damage, fire spreading to the next item) are not shown; they still count in the bill.")]
        [SerializeField, Range(1, 3)] private int _feedMinSeverity = 2;
        [Tooltip("Minimum seconds between two lines; a line arriving sooner is dropped unless it has the top severity.")]
        [SerializeField, Min(0f)] private float _feedCooldownSeconds = 3f;
        [Tooltip("Events that never get a feed line (they still count in the bill). " +
            "The player's own shock already has its full-screen effect, so it is hidden by default.")]
        [SerializeField] private NarrativeEvent[] _feedHiddenEvents = { NarrativeEvent.ShockPlayer };

        [Header("Report")]
        [SerializeField, Min(0.01f)] private float _reportFadeSeconds = 0.3f;
        [SerializeField, Min(0.01f)] private float _counterSeconds = 1.4f;
        [SerializeField, Min(0f)] private float _stampDelaySeconds = 0.4f;
        [SerializeField, Min(0.01f)] private float _stampSeconds = 0.22f;

        [Header("HUD")]
        [SerializeField] private Color _pressureColor = new Color(0.45f, 0.8f, 1f);
        [SerializeField] private Color _pressureMaxColor = new Color(1f, 0.3f, 0.25f);
        [SerializeField] private Color _panelColor = new Color(0.1f, 0.08f, 0.11f, 0.88f);
        [SerializeField] private Color _warningColor = new Color(0.86f, 0.2f, 0.15f, 0.95f);
        [SerializeField] private Color _accentColor = new Color(1f, 0.42f, 0.12f);
        [SerializeField, Min(0.05f)] private float _hudIntroSeconds = 0.65f;
        [Tooltip("Under this many seconds left the timer turns red and ticks.")]
        [SerializeField, Min(0)] private int _timerWarningSeconds = 30;
        [Tooltip("Under this many seconds left the timer also shakes.")]
        [SerializeField, Min(0)] private int _timerPanicSeconds = 10;

        [Header("Briefing")]
        [SerializeField, Min(0.05f)] private float _briefingIntroSeconds = 0.6f;
        [Tooltip("Typewriter speed of the briefing text.")]
        [SerializeField, Min(1f)] private float _briefingCharsPerSecond = 70f;

        public int FeedMaxEntries => _feedMaxEntries;
        public float FeedEntrySeconds => _feedEntrySeconds;
        public float FeedPopSeconds => _feedPopSeconds;
        public float FeedFadeSeconds => _feedFadeSeconds;
        public float FeedShakeSeconds => _feedShakeSeconds;
        public float FeedShakePixels => _feedShakePixels;
        public int FeedShakeMinSeverity => _feedShakeMinSeverity;
        public int FeedMinSeverity => _feedMinSeverity;
        public float FeedCooldownSeconds => _feedCooldownSeconds;

        public bool IsHiddenInFeed(NarrativeEvent kind) => System.Array.IndexOf(_feedHiddenEvents, kind) >= 0;

        public float ReportFadeSeconds => _reportFadeSeconds;
        public float CounterSeconds => _counterSeconds;
        public float StampDelaySeconds => _stampDelaySeconds;
        public float StampSeconds => _stampSeconds;
        public Color PressureColor => _pressureColor;
        public Color PressureMaxColor => _pressureMaxColor;
        public Color PanelColor => _panelColor;
        public Color WarningColor => _warningColor;
        public Color AccentColor => _accentColor;
        public float HudIntroSeconds => _hudIntroSeconds;
        public int TimerWarningSeconds => _timerWarningSeconds;
        public int TimerPanicSeconds => _timerPanicSeconds;
        public float BriefingIntroSeconds => _briefingIntroSeconds;
        public float BriefingCharsPerSecond => _briefingCharsPerSecond;
    }
}
