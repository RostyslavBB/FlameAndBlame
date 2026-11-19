using System.Collections.Generic;

namespace FB.Game.Blame
{
    /// <summary>Accumulated blame of the round: $ and points per actor, fire vs firefighter damage, records.</summary>
    public interface IBlameLedger
    {
        int TotalDamage { get; }

        /// <summary>$ attributed to the building and the fire itself.</summary>
        int FireDamage { get; }

        /// <summary>$ attributed to the player (water, electricity, chaos).</summary>
        int FirefighterDamage { get; }

        float TotalPoints { get; }
        float DamageOf(ActorId actor);
        float PointsOf(ActorId actor);
        IReadOnlyList<BlameRecord> Records { get; }
    }
}
