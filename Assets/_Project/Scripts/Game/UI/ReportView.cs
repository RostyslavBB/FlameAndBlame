using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FB.Game.UI
{
    /// <summary>Passive "Рахунок за виклик" panel. The button only forwards the click.</summary>
    public sealed class ReportView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TMP_Text _fireDamage;
        [SerializeField] private TMP_Text _firefighterDamage;
        [SerializeField] private TMP_Text _total;
        [SerializeField] private TMP_Text _chains;
        [SerializeField] private TMP_Text _verdict;
        [SerializeField] private RectTransform _stamp;
        [SerializeField] private Image _stampImage;
        [SerializeField] private TMP_Text _stampLabel;
        [SerializeField] private Button _restart;

        public event Action RestartClicked;

        private void Awake() => _restart.onClick.AddListener(OnRestartClicked);

        private void OnDestroy() => _restart.onClick.RemoveListener(OnRestartClicked);

        public void SetAlpha(float alpha)
        {
            _group.alpha = alpha;
            _group.interactable = alpha > 0.99f;
            _group.blocksRaycasts = alpha > 0f;
            gameObject.SetActive(alpha > 0f);
        }

        public void SetDamage(string fire, string firefighter, string total)
        {
            _fireDamage.text = fire;
            _firefighterDamage.text = firefighter;
            _total.text = total;
        }

        public void SetChains(string text) => _chains.text = text;

        public void SetVerdict(string text) => _verdict.text = text;

        public void SetStamp(float scale, float alpha, Color color, string label)
        {
            _stamp.localScale = new Vector3(scale, scale, 1f);
            color.a = alpha;
            _stampImage.color = color;
            _stampLabel.color = color;
            _stampLabel.text = label;
        }

        private void OnRestartClicked() => RestartClicked?.Invoke();
    }
}
