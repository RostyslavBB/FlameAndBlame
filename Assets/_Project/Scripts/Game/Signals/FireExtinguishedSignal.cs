using FB.Game.Blame;
using FB.Game.Surfaces;

namespace FB.Game.Signals
{
    /// <summary>A burning surface was put out. Cause is the water that did it.</summary>
    public readonly struct FireExtinguishedSignal
    {
        public FireExtinguishedSignal(ISurface target, Cause cause)
        {
            Target = target;
            Cause = cause;
        }

        public ISurface Target { get; }
        public Cause Cause { get; }
    }
}
