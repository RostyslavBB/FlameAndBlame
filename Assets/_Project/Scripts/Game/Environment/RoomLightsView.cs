using UnityEngine;

namespace FB.Game.Environment
{
    /// <summary>
    /// Passive list of what the apartment power feeds: ceiling lamp lights plus glowing material slots
    /// (lamp shades, TV and laptop screens). Switching only changes lights and slot colors.
    /// </summary>
    public sealed class RoomLightsView : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Light[] _lights;
        [SerializeField] private MeshRenderer[] _glowRenderers;
        [SerializeField] private int[] _glowSlots;
        [SerializeField] private Color[] _glowColors;
        [SerializeField] private Color _offColor = new Color(0.12f, 0.12f, 0.13f);

        private MaterialPropertyBlock _block;

        public void SetPowered(bool powered)
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].enabled = powered;
            }

            _block ??= new MaterialPropertyBlock();
            for (int i = 0; i < _glowRenderers.Length; i++)
            {
                _glowRenderers[i].GetPropertyBlock(_block, _glowSlots[i]);
                _block.SetColor(BaseColorId, powered ? _glowColors[i] : _offColor);
                _glowRenderers[i].SetPropertyBlock(_block, _glowSlots[i]);
            }
        }
    }
}
