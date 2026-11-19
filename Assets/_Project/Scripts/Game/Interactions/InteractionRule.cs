using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Interactions
{
    /// <summary>
    /// Base for rules stored as assets, so designers reorder and tune them in InteractionRulesConfig.
    /// Rules are stateless: they only read the event and act through ISurfaceActions.
    /// </summary>
    public abstract class InteractionRule : ScriptableObject, IInteractionRule
    {
        public abstract InteractionTrigger Trigger { get; }
        public abstract bool Matches(in InteractionEvent interaction);
        public abstract void Apply(in InteractionEvent interaction, ISurfaceActions actions);
    }
}
