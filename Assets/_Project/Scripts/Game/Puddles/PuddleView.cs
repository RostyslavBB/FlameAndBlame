using FB.Core.Pooling;
using UnityEngine;

namespace FB.Game.Puddles
{
    /// <summary>Pooled flat disc on the floor. Passive: size and color are pushed by PuddleService.</summary>
    public sealed class PuddleView : MonoBehaviour, IPoolItem
    {
        private const float Thickness = 0.01f;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private MeshRenderer _renderer;

        private MaterialPropertyBlock _block;

        public void Show(Vector3 center, float radius, Color color)
        {
            transform.position = new Vector3(center.x, center.y + Thickness * 0.5f, center.z);
            transform.localScale = new Vector3(radius * 2f, Thickness, radius * 2f);
            _block ??= new MaterialPropertyBlock();
            _block.SetColor(BaseColorId, color);
            _renderer.SetPropertyBlock(_block);
        }

        public void OnSpawned()
        {
        }

        public void OnDespawned()
        {
            transform.localScale = Vector3.zero;
        }
    }
}
