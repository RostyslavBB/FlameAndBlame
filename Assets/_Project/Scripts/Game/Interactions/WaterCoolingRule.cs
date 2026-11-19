using UnityEngine;
using FB.Game.Surfaces;

namespace FB.Game.Interactions
{
    /// <summary>Water on a warm (not yet burning) surface takes heat away, delaying ignition.</summary>
    [CreateAssetMenu(menuName = "FB/Rules/Water Cooling", fileName = "R_WaterCooling")]
    public sealed class WaterCoolingRule : InteractionRule
    {
        [SerializeField, Min(0f)] private float _coolingPerWater = 0.8f;

        public override InteractionTrigger Trigger => InteractionTrigger.Water;

        public override bool Matches(in InteractionEvent interaction) =>
            !interaction.Target.IsBurning && interaction.Target.Heat > 0f;

        public override void Apply(in InteractionEvent interaction, ISurfaceActions actions)
        {
            actions.AddHeat(interaction.Target, -interaction.Amount * _coolingPerWater);
        }
    }
}
