using FB.Game.Surfaces;

namespace FB.Game.Interactions
{
    /// <summary>Strategy: one material/status interaction ("burning + water = extinguish").</summary>
    public interface IInteractionRule
    {
        /// <summary>The kind of event this rule listens to.</summary>
        InteractionTrigger Trigger { get; }

        /// <summary>True if the rule applies to this event.</summary>
        bool Matches(in InteractionEvent interaction);

        /// <summary>Applies the effect. All changes go through <paramref name="actions"/>.</summary>
        void Apply(in InteractionEvent interaction, ISurfaceActions actions);
    }
}
