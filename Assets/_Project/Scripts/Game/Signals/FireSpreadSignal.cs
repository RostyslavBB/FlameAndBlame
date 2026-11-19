using FB.Game.Blame;
using FB.Game.Surfaces;

namespace FB.Game.Signals
{
    /// <summary>A surface caught fire. Source is null for the initial ignition.</summary>
    public readonly struct FireSpreadSignal
    {
        public FireSpreadSignal(ISurface target, ISurface source, Cause cause)
        {
            Target = target;
            Source = source;
            Cause = cause;
        }

        public ISurface Target { get; }
        public ISurface Source { get; }
        public Cause Cause { get; }
    }
}
