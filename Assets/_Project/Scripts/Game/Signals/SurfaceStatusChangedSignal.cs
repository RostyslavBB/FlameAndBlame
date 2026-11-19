using FB.Game.Surfaces;

namespace FB.Game.Signals
{
    /// <summary>A status flag was added to or removed from a surface (edges only, not every tick).</summary>
    public readonly struct SurfaceStatusChangedSignal
    {
        public SurfaceStatusChangedSignal(ISurface surface, SurfaceStatus status, bool added)
        {
            Surface = surface;
            Status = status;
            Added = added;
        }

        public ISurface Surface { get; }
        public SurfaceStatus Status { get; }
        public bool Added { get; }
    }
}
