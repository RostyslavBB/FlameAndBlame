using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Interactions
{
    /// <summary>Flammable material + burning neighbour: heat accumulates (slower when wet) until ignition.</summary>
    [CreateAssetMenu(menuName = "FB/Rules/Neighbor Ignition", fileName = "R_NeighborIgnition")]
    public sealed class NeighborIgnitionRule : InteractionRule
    {
        [SerializeField] private MaterialKind[] _kinds =
        {
            MaterialKind.Wood, MaterialKind.Fabric, MaterialKind.Electronics, MaterialKind.Plant, MaterialKind.Grease
        };

        [Tooltip("Heat multiplier for wet surfaces.")]
        [SerializeField, Range(0f, 1f)] private float _wetHeatMultiplier = 0.25f;

        public override InteractionTrigger Trigger => InteractionTrigger.NeighborHeat;

        public override bool Matches(in InteractionEvent interaction)
        {
            Surface target = interaction.Target;
            return target.Profile.Flammable && !target.IsBurning && !target.IsRuined && HasKind(target.Kind);
        }

        public override void Apply(in InteractionEvent interaction, ISurfaceActions actions)
        {
            Surface target = interaction.Target;
            float wet = target.Statuses.Has(SurfaceStatus.Wet) ? _wetHeatMultiplier : 1f;
            actions.AddHeat(target, interaction.Amount * target.Profile.Flammability * wet);
            if (target.Heat >= target.Profile.IgnitionHeat)
            {
                actions.IgniteFrom(target, interaction.Source);
            }
        }

        private bool HasKind(MaterialKind kind)
        {
            for (int i = 0; i < _kinds.Length; i++)
            {
                if (_kinds[i] == kind)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
