using FB.Game.Blame;
using FB.Game.Surfaces;

namespace FB.Game.Signals
{
    /// <summary>An item lost its value. Damage is in $; Cause leads back to whoever is to blame.</summary>
    public readonly struct ItemRuinedSignal
    {
        public ItemRuinedSignal(ISurface entity, int damage, RuinReason reason, Cause cause)
        {
            Entity = entity;
            Damage = damage;
            Reason = reason;
            Cause = cause;
        }

        public ISurface Entity { get; }
        public int Damage { get; }
        public RuinReason Reason { get; }
        public Cause Cause { get; }
    }
}
