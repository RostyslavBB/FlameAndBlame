using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Interactions
{
    /// <summary>Burning + Water: water removes heat; below a share of the ignition heat the fire goes out.</summary>
    [CreateAssetMenu(menuName = "FB/Rules/Extinguish", fileName = "R_Extinguish")]
    public sealed class ExtinguishRule : InteractionRule
    {
        [SerializeField, Min(0f)] private float _coolingPerWater = 0.6f;
        [SerializeField, Range(0f, 1f)] private float _extinguishBelowIgnitionShare = 0.5f;

        public override InteractionTrigger Trigger => InteractionTrigger.Water;

        public override bool Matches(in InteractionEvent interaction) => interaction.Target.IsBurning;

        public override void Apply(in InteractionEvent interaction, ISurfaceActions actions)
        {
            Surface target = interaction.Target;
            actions.AddHeat(target, -interaction.Amount * _coolingPerWater);
            if (target.Heat <= target.Profile.IgnitionHeat * _extinguishBelowIgnitionShare)
            {
                actions.Extinguish(target, interaction.Cause);
            }
        }
    }
}
