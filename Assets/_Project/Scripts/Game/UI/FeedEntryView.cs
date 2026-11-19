using FB.Core.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>
    /// Pooled feed line: colored actor badge and name, funny text, damage, and a white flash overlay.
    /// Animated by the feed presenter.
    /// </summary>
    public sealed class FeedEntryView : MonoBehaviour, IPoolItem
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _content;
        [SerializeField] private Image _badge;
        [SerializeField] private Image _flash;
        [SerializeField] private TMP_Text _actor;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _damage;

        public void Set(string actor, Color actorColor, string text)
        {
            _badge.color = actorColor;
            _actor.text = actor;
            _actor.color = Color.Lerp(actorColor, Color.white, 0.35f);
            _text.text = text;
        }

        public void SetDamage(string damage)
        {
            _damage.text = damage;
            _damage.enabled = !string.IsNullOrEmpty(damage);
        }

        public void SetDamageScale(float scale) => _damage.rectTransform.localScale = new Vector3(scale, scale, 1f);

        public void SetAlpha(float alpha) => _group.alpha = alpha;

        public void SetScale(float scale) => _content.localScale = new Vector3(scale, scale, 1f);

        public void SetOffset(Vector2 offset) => _content.anchoredPosition = offset;

        public void SetTilt(float degrees) => _content.localRotation = Quaternion.Euler(0f, 0f, degrees);

        public void SetFlash(float alpha)
        {
            Color color = _flash.color;
            color.a = alpha;
            _flash.color = color;
        }

        public void OnSpawned()
        {
            SetAlpha(0f);
            SetOffset(Vector2.zero);
            SetScale(1f);
            SetTilt(0f);
            SetFlash(0f);
            SetDamageScale(1f);
        }

        public void OnDespawned()
        {
            SetAlpha(0f);
        }
    }
}
