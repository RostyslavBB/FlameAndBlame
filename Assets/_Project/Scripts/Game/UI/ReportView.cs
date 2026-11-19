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
        [SerializeField] private RectTransform _card;
        [SerializeField] private CanvasGroup[] _rows;
        [SerializeField] private TMP_Text _fireDamage;
        [SerializeField] private TMP_Text _firefighterDamage;
        [SerializeField] private TMP_Text _total;
        [SerializeField] private TMP_Text _chains;
        [SerializeField] private ChainLinkView[] _links;
        [SerializeField] private TMP_Text _verdict;
        [SerializeField] private RectTransform _stamp;
        [SerializeField] private Image _stampImage;
        [SerializeField] private TMP_Text _stampLabel;
        [SerializeField] private Button _restart;

        private Vector2[] _rowHomes;

        public event Action RestartClicked;

        public int RowCount => _rows.Length;

        private void Awake() => _restart.onClick.AddListener(OnRestartClicked);

        private void OnDestroy() => _restart.onClick.RemoveListener(OnRestartClicked);

        public void SetAlpha(float alpha)
        {
            _group.alpha = alpha;
            _group.interactable = alpha > 0.99f;
            _group.blocksRaycasts = alpha > 0f;
            gameObject.SetActive(alpha > 0f);
        }

        public void SetCard(Vector2 offset, float rotation)
        {
            _card.anchoredPosition = offset;
            _card.localRotation = Quaternion.Euler(0f, 0f, rotation);
        }

        /// <summary>Row reveal: 0 = hidden and shifted left, 1 = in place.</summary>
        public void SetRow(int index, float reveal)
        {
            EnsureRowHomes();
            _rows[index].alpha = Mathf.Clamp01(reveal);
            ((RectTransform)_rows[index].transform).anchoredPosition = _rowHomes[index] + new Vector2((reveal - 1f) * 80f, 0f);
        }

        public void SetDamage(string fire, string firefighter, string total)
        {
            _fireDamage.text = fire;
            _firefighterDamage.text = firefighter;
            _total.text = total;
        }

        public void SetTotalScale(float scale) => _total.rectTransform.localScale = new Vector3(scale, scale, 1f);

        public void SetChains(string text) => _chains.text = text;

        public int ChainsLength => _chains.text.Length;

        public void SetChainsVisible(int characters) => _chains.maxVisibleCharacters = characters;

        public int LinkCount => _links.Length;

        public void SetLink(int index, ChainLink link, bool last)
        {
            _links[index].SetVisible(true);
            _links[index].Set(link, last);
            _links[index].SetReveal(0f, 0f);
        }

        public void HideLinks()
        {
            for (int i = 0; i < _links.Length; i++)
            {
                _links[i].SetVisible(false);
            }
        }

        public void SetLinkReveal(int index, float card, float arrow) => _links[index].SetReveal(card, arrow);

        public void SetVerdict(string text) => _verdict.text = text;

        public void SetVerdictScale(float scale) => _verdict.rectTransform.localScale = new Vector3(scale, scale, 1f);

        public void SetStamp(float scale, float alpha, Color color, string label)
        {
            _stamp.localScale = new Vector3(scale, scale, 1f);
            color.a = alpha;
            _stampImage.color = color;
            _stampLabel.color = color;
            _stampLabel.text = label;
        }

        public void SetRestart(float scale, bool visible)
        {
            Transform button = _restart.transform;
            button.localScale = new Vector3(scale, scale, 1f);
            _restart.gameObject.SetActive(visible);
        }

        /// <summary>
        /// The panel starts inactive, so Awake has not run when the presenter prepares it: the row rest positions are
        /// captured on first use instead.
        /// </summary>
        private void EnsureRowHomes()
        {
            if (_rowHomes != null)
            {
                return;
            }

            _rowHomes = new Vector2[_rows.Length];
            for (int i = 0; i < _rows.Length; i++)
            {
                _rowHomes[i] = ((RectTransform)_rows[i].transform).anchoredPosition;
            }
        }

        private void OnRestartClicked() => RestartClicked?.Invoke();
    }
}
