using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Game.Smoke
{
    /// <summary>Passive smoke visuals: the ceiling smoke layers, the drifting ceiling puffs and the smoky post-processing volume.</summary>
    public sealed class SmokeView : MonoBehaviour
    {
        private static readonly int DensityId = Shader.PropertyToID("_Density");

        [SerializeField] private MeshRenderer _layer;
        [SerializeField] private Volume _volume;
        [SerializeField] private ParticleSystem _puffs;

        private MaterialPropertyBlock _block;

        public void Show(float amount, float layerY, float puffsPerSecond)
        {
            _layer.enabled = amount > 0.01f;
            Vector3 position = _layer.transform.position;
            _layer.transform.position = new Vector3(position.x, layerY, position.z);
            _block ??= new MaterialPropertyBlock();
            _block.SetFloat(DensityId, amount);
            _layer.SetPropertyBlock(_block);
            _volume.weight = amount;
            if (_puffs != null)
            {
                ParticleSystem.EmissionModule emission = _puffs.emission;
                emission.rateOverTimeMultiplier = puffsPerSecond;
            }
        }
    }
}
