using FB.Core.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>Pooled feed line: colored actor badge, funny text, damage. Animated by the feed presenter.</summary>
    public sealed class FeedEntryView : MonoBehaviour, IPoolItem
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _content;
        [SerializeField] private Image _badge;
        [SerializeField] private TMP_Text _actor;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _damage;

        public void Set(string actor, Color actorColor, string text, string damage)
        {
            _badge.color = actorColor;
            _actor.text = actor;
            _text.text = text;
            _damage.text = damage;
            _damage.enabled = !string.IsNullOrEmpty(damage);
        }

        public void SetAlpha(float alpha) => _group.alpha = alpha;

        public void SetScale(float scale) => _content.localScale = new Vector3(scale, scale, 1f);

        public void SetOffset(Vector2 offset) => _content.anchoredPosition = offset;

        public void OnSpawned()
        {
            SetAlpha(0f);
            SetOffset(Vector2.zero);
            SetScale(1f);
        }

        public void OnDespawned()
        {
            SetAlpha(0f);
        }
    }
}
