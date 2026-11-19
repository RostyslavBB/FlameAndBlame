using System.Collections.Generic;
using FB.Game.Surfaces;

namespace FB.Game.Interactions
{
    /// <summary>Runs every matching rule for an interaction event.</summary>
    public sealed class InteractionRuleRunner
    {
        private readonly IReadOnlyList<InteractionRule> _rules;
        private readonly ISurfaceActions _actions;

        public InteractionRuleRunner(InteractionRulesConfig config, ISurfaceActions actions)
        {
            _rules = config.Rules;
            _actions = actions;
        }

        public void Run(in InteractionEvent interaction)
        {
            for (int i = 0; i < _rules.Count; i++)
            {
                IInteractionRule rule = _rules[i];
                if (rule != null && rule.Trigger == interaction.Trigger && rule.Matches(interaction))
                {
                    rule.Apply(interaction, _actions);
                }
            }
        }
    }
}
