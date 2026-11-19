using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>Passive briefing card shown before the round: title, typed-out text, siren glow, countdown bar.</summary>
    public sealed class BriefingView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _card;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private Image _siren;
        [SerializeField] private Image _countdown;
        [SerializeField] private TMP_Text _countdownLabel;

        public void SetText(string title, string body)
        {
            _title.text = title;
            _body.text = body;
        }

        public int BodyLength => _body.text.Length;

        public void SetAlpha(float alpha)
        {
            _group.alpha = alpha;
            gameObject.SetActive(alpha > 0f);
        }

        public void SetCard(float offsetY, float scale, float rotation)
        {
            _card.anchoredPosition = new Vector2(0f, offsetY);
            _card.localScale = new Vector3(scale, scale, 1f);
            _card.localRotation = Quaternion.Euler(0f, 0f, rotation);
        }

        public void SetTitleScale(float scale) => _title.rectTransform.localScale = new Vector3(scale, scale, 1f);

        public void SetVisibleCharacters(int count) => _body.maxVisibleCharacters = count;

        public void SetSiren(float alpha)
        {
            Color color = _siren.color;
            color.a = alpha;
            _siren.color = color;
        }

        public void SetCountdown(float fill) => _countdown.fillAmount = fill;

        public void SetCountdownLabel(string text) => _countdownLabel.text = text;

        public void SetCountdownScale(float scale) => _countdownLabel.rectTransform.localScale = new Vector3(scale, scale, 1f);
    }
}
