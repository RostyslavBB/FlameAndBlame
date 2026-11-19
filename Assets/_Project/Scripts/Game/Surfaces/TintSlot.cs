using UnityEngine;

namespace FB.Game.Surfaces
{
    /// <summary>One material slot of a surface renderer with its original color (models have several slots).</summary>
    public readonly struct TintSlot
    {
        public TintSlot(MeshRenderer renderer, int materialIndex, Color baseColor)
        {
            Renderer = renderer;
            MaterialIndex = materialIndex;
            BaseColor = baseColor;
        }

        public MeshRenderer Renderer { get; }
        public int MaterialIndex { get; }
        public Color BaseColor { get; }
    }
}
