using System.Collections.Generic;
using UnityEngine;

namespace FB.Game.Interactions
{
    /// <summary>Ordered list of interaction rules. Every matching rule is applied, top to bottom.</summary>
    [CreateAssetMenu(menuName = "FB/Interaction Rules Config", fileName = "InteractionRulesConfig")]
    public sealed class InteractionRulesConfig : ScriptableObject
    {
        [SerializeField] private List<InteractionRule> _rules = new List<InteractionRule>();

        public IReadOnlyList<InteractionRule> Rules => _rules;

#if UNITY_EDITOR
        public void EditorSetRules(IEnumerable<InteractionRule> rules)
        {
            _rules = new List<InteractionRule>(rules);
        }
#endif
    }
}
