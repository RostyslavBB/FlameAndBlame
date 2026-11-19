namespace FB.Game.Surfaces
{
    /// <summary>A surface close enough to receive heat, with a 0..1 falloff by distance.</summary>
    public readonly struct SurfaceNeighbor
    {
        public SurfaceNeighbor(Surface surface, float falloff)
        {
            Surface = surface;
            Falloff = falloff;
        }

        public Surface Surface { get; }
        public float Falloff { get; }
    }
}
