using FB.Game.Blame;
using FB.Game.Surfaces;

namespace FB.Game.Interactions
{
    /// <summary>Input of the rule pipeline: something (water, heat) reached a surface.</summary>
    public readonly struct InteractionEvent
    {
        public InteractionEvent(InteractionTrigger trigger, Surface target, Surface source, float amount, Cause cause)
        {
            Trigger = trigger;
            Target = target;
            Source = source;
            Amount = amount;
            Cause = cause;
        }

        public InteractionTrigger Trigger { get; }
        public Surface Target { get; }

        /// <summary>The burning neighbour for heat events; null for water.</summary>
        public Surface Source { get; }

        /// <summary>Water units or heat units, depending on the trigger.</summary>
        public float Amount { get; }

        public Cause Cause { get; }
    }
}
