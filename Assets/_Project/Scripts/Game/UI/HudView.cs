using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>Passive HUD: round timer, hose pressure bar, crosshair, interaction hint.</summary>
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TMP_Text _timer;
        [SerializeField] private Image _pressureFill;
        [SerializeField] private Image _crosshair;
        [SerializeField] private TMP_Text _hint;

        public void SetVisible(bool visible) => _group.alpha = visible ? 1f : 0f;

        public void SetTime(string text) => _timer.text = text;

        public void SetPressure(float fill, Color color)
        {
            _pressureFill.fillAmount = fill;
            _pressureFill.color = color;
            _crosshair.color = color;
        }

        public void SetHint(string text)
        {
            _hint.text = text;
            _hint.enabled = !string.IsNullOrEmpty(text);
        }
    }
}
