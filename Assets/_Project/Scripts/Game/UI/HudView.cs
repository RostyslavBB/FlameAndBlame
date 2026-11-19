using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>
    /// Passive HUD: round timer, running damage bill, hose pressure bar, crosshair and interaction hint.
    /// The presenters drive every number and every animation through these setters.
    /// </summary>
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _top;
        [SerializeField] private RectTransform _bottom;
        [SerializeField] private RectTransform _timerRoot;
        [SerializeField] private Image _timerFace;
        [SerializeField] private TMP_Text _timer;
        [SerializeField] private RectTransform _damageRoot;
        [SerializeField] private TMP_Text _damage;
        [SerializeField] private RectTransform _pressureRoot;
        [SerializeField] private Image _pressureFill;
        [SerializeField] private Image _pressureGlow;
        [SerializeField] private TMP_Text _pressureLabel;
        [SerializeField] private RectTransform _crosshair;
        [SerializeField] private Image _crosshairRing;
        [SerializeField] private Image _crosshairDot;
        [SerializeField] private RectTransform _hintRoot;
        [SerializeField] private CanvasGroup _hintGroup;
        [SerializeField] private TMP_Text _hint;

        private Vector2 _timerHome;
        private Vector2 _pressureHome;
        private Vector2 _hintHome;

        private void Awake()
        {
            _timerHome = _timerRoot.anchoredPosition;
            _pressureHome = _pressureRoot.anchoredPosition;
            _hintHome = _hintRoot.anchoredPosition;
        }

        public void SetAlpha(float alpha) => _group.alpha = alpha;

        /// <summary>Intro slide: the top bar comes down and the pressure bar comes up from off-screen.</summary>
        public void SetSlide(float offset)
        {
            _top.anchoredPosition = new Vector2(0f, offset);
            _bottom.anchoredPosition = new Vector2(0f, -offset);
        }

        public void SetTime(string text) => _timer.text = text;

        public void SetTimerStyle(float scale, Color face, Color text, Vector2 shake)
        {
            _timerRoot.localScale = new Vector3(scale, scale, 1f);
            _timerRoot.anchoredPosition = _timerHome + shake;
            _timerFace.color = face;
            _timer.color = text;
        }

        public void SetDamage(string text) => _damage.text = text;

        public void SetDamageStyle(float scale, Color color)
        {
            _damageRoot.localScale = new Vector3(scale, scale, 1f);
            _damage.color = color;
        }

        public void SetPressure(float fill, Color color, float glow, Vector2 shake)
        {
            _pressureFill.fillAmount = fill;
            _pressureFill.color = color;
            Color glowColor = color;
            glowColor.a = glow;
            _pressureGlow.color = glowColor;
            _pressureRoot.anchoredPosition = _pressureHome + shake;
            _pressureLabel.color = Color.Lerp(Color.white, color, glow);
        }

        public void SetCrosshair(float scale, Color color)
        {
            _crosshair.localScale = new Vector3(scale, scale, 1f);
            _crosshairRing.color = color;
            _crosshairDot.color = color;
        }

        public void SetHint(string text) => _hint.text = text;

        public void SetHintStyle(float alpha, float scale, float bob)
        {
            _hintGroup.alpha = alpha;
            _hintRoot.localScale = new Vector3(scale, scale, 1f);
            _hintRoot.anchoredPosition = _hintHome + new Vector2(0f, bob);
        }
    }
}
