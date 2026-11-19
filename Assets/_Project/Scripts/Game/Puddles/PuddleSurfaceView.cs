using UnityEngine;

namespace FB.Game.Puddles
{
    /// <summary>
    /// Passive quad over the floor drawn with FB/Puddle. The simulation pushes the water texture (R = depth,
    /// G = electrified) and the active ripples; the shader does the rest.
    /// </summary>
    public sealed class PuddleSurfaceView : MonoBehaviour
    {
        private static readonly int WaterTexId = Shader.PropertyToID("_WaterTex");
        private static readonly int GridRectId = Shader.PropertyToID("_GridRect");
        private static readonly int RipplesId = Shader.PropertyToID("_Ripples");
        private static readonly int TexelSizeId = Shader.PropertyToID("_WaterTex_TexelSize");

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Vector2 _origin;
        [SerializeField] private Vector2 _size;

        private MaterialPropertyBlock _block;

        /// <summary>XZ of the grid's min corner (world).</summary>
        public Vector2 Origin => _origin;

        /// <summary>XZ size of the covered area (m).</summary>
        public Vector2 Size => _size;

        public void Bind(Texture2D water)
        {
            _block ??= new MaterialPropertyBlock();
            _block.SetTexture(WaterTexId, water);
            _block.SetVector(TexelSizeId, new Vector4(1f / water.width, 1f / water.height, water.width, water.height));
            _block.SetVector(GridRectId, new Vector4(_origin.x, _origin.y, _size.x, _size.y));
            _renderer.SetPropertyBlock(_block);
        }

        public void SetRipples(Vector4[] ripples)
        {
            _block.SetVectorArray(RipplesId, ripples);
            _renderer.SetPropertyBlock(_block);
        }
    }
}
