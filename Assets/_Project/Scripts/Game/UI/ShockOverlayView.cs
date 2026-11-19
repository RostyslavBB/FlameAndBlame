using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>
    /// Passive electric-shock effect: full-screen lightning overlay + a post-processing volume
    /// (chromatic aberration, lens distortion, cold flash). The presenter drives strength and jitter.
    /// </summary>
    public sealed class ShockOverlayView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RawImage _bolts;
        [SerializeField] private Image _flash;
        [SerializeField] private Volume _volume;

        public void Show(float bolts, float flash, float distortion, Vector2 jitter, bool flipX, bool flipY)
        {
            _group.alpha = Mathf.Max(bolts, flash);
            _bolts.color = new Color(1f, 1f, 1f, bolts);
            _bolts.uvRect = new Rect(flipX ? 1f : 0f, flipY ? 1f : 0f, flipX ? -1f : 1f, flipY ? -1f : 1f);
            _bolts.rectTransform.anchoredPosition = jitter;
            Color color = _flash.color;
            color.a = flash;
            _flash.color = color;
            _volume.weight = distortion;
        }

        public void Hide() => Show(0f, 0f, 0f, Vector2.zero, false, false);
    }
}
