using FB.Game.Blame;

namespace FB.Game.Surfaces
{
    /// <summary>
    /// The only way to change surface state. Every method takes a <see cref="Cause"/> (or derives one) and
    /// announces the change through signals, so blame and visuals never miss an effect.
    /// </summary>
    public interface ISurfaceActions
    {
        void AddWater(Surface target, float amount, Cause cause);

        /// <summary>Removes the Wet status once the surface dried.</summary>
        void Dry(Surface target);

        /// <summary>Adds (or removes, if negative) heat. Never below zero.</summary>
        void AddHeat(Surface target, float amount);

        /// <summary>Sets the surface on fire with an explicit cause (e.g. the round-start ignition).</summary>
        void Ignite(Surface target, Cause cause);

        /// <summary>Fire spreads from a burning source; the new cause gets the source's fire as parent.</summary>
        void IgniteFrom(Surface target, Surface source);

        void Extinguish(Surface target, Cause cause);

        /// <summary>Ruins the item; the ruin cause is derived from <paramref name="parent"/>.</summary>
        void Ruin(Surface target, RuinReason reason, Cause parent);
    }
}
