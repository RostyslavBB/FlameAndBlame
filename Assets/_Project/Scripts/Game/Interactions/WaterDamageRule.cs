using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Interactions
{
    /// <summary>
    /// Water-sensitive material + enough water = ruined. One asset per case:
    /// Electronics (short circuit), Plant with lots of water (drowned).
    /// </summary>
    [CreateAssetMenu(menuName = "FB/Rules/Water Damage", fileName = "R_WaterDamage")]
    public sealed class WaterDamageRule : InteractionRule
    {
        [SerializeField] private MaterialKind _kind = MaterialKind.Electronics;
        [Tooltip("Accumulated water needed to ruin the item.")]
        [SerializeField, Min(0f)] private float _waterThreshold = 0.8f;
        [SerializeField] private RuinReason _reason = RuinReason.ShortCircuit;

        public override InteractionTrigger Trigger => InteractionTrigger.Water;

        public override bool Matches(in InteractionEvent interaction)
        {
            Surface target = interaction.Target;
            return target.Kind == _kind && target.Profile.WaterSensitive && !target.IsRuined &&
                   target.Water >= _waterThreshold;
        }

        public override void Apply(in InteractionEvent interaction, ISurfaceActions actions)
        {
            actions.Ruin(interaction.Target, _reason, interaction.Cause);
        }

#if UNITY_EDITOR
        public void EditorSetup(MaterialKind kind, float waterThreshold, RuinReason reason)
        {
            _kind = kind;
            _waterThreshold = waterThreshold;
            _reason = reason;
        }
#endif
    }
}
