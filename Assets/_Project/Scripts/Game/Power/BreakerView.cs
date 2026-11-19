using UnityEngine;

namespace FB.Game.Power
{
    /// <summary>Passive breaker on the hall wall: flips its lever and lamp color.</summary>
    public sealed class BreakerView : MonoBehaviour
    {
        private const float LeverAngle = 35f;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Transform _lever;
        [SerializeField] private MeshRenderer _lamp;
        [SerializeField] private Color _onColor = new Color(0.3f, 1f, 0.35f);
        [SerializeField] private Color _offColor = new Color(1f, 0.25f, 0.2f);

        private MaterialPropertyBlock _block;

        public Vector3 Position => transform.position;

        public void SetOn(bool isOn)
        {
            _lever.localRotation = Quaternion.Euler(isOn ? -LeverAngle : LeverAngle, 0f, 0f);
            _block ??= new MaterialPropertyBlock();
            _block.SetColor(BaseColorId, isOn ? _onColor : _offColor);
            _lamp.SetPropertyBlock(_block);
        }
    }
}
