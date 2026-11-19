using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>Passive container of the blame feed (right side of the screen, newest on top).</summary>
    public sealed class FeedView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private CanvasGroup _group;

        public RectTransform Container => _container;

        public void SetVisible(bool visible) => _group.alpha = visible ? 1f : 0f;
    }
}
