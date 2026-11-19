using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>Passive card of the blame chain on the report: colored strip, title, subtitle, arrow to the next card.</summary>
    public sealed class ChainLinkView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _card;
        [SerializeField] private Image _strip;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _subtitle;
        [SerializeField] private TMP_Text _arrow;

        public void Set(ChainLink link, bool last)
        {
            _strip.color = link.Color;
            _title.text = link.Title;
            _title.color = Color.Lerp(link.Color, Color.black, 0.25f);
            _subtitle.text = link.Subtitle;
            _arrow.enabled = !last;
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);

        /// <summary>0 = hidden, 1 = in place; the card pops and the arrow follows.</summary>
        public void SetReveal(float card, float arrow)
        {
            _group.alpha = Mathf.Clamp01(card * 2f);
            float scale = Mathf.Max(0f, card);
            _card.localScale = new Vector3(scale, scale, 1f);
            Color color = _arrow.color;
            color.a = Mathf.Clamp01(arrow);
            _arrow.color = color;
            _arrow.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-14f, 0f, Mathf.Clamp01(arrow)), 0f);
        }
    }
}
