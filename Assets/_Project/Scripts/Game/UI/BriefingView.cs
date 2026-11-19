using TMPro;
using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>Passive briefing card shown before the round.</summary>
    public sealed class BriefingView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;

        public void SetText(string title, string body)
        {
            _title.text = title;
            _body.text = body;
        }

        public void SetAlpha(float alpha)
        {
            _group.alpha = alpha;
            gameObject.SetActive(alpha > 0f);
        }
    }
}
