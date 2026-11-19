using FB.Game.Blame;
using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Signals
{
    /// <summary>
    /// The player or an object got shocked by an electrified puddle.
    /// Cause has two parents: the puddle (water) and the live power source.
    /// </summary>
    public readonly struct ElectrocutionSignal
    {
        public ElectrocutionSignal(bool isPlayer, ISurface surface, Vector3 point, int damage, Cause cause)
        {
            IsPlayer = isPlayer;
            Surface = surface;
            Point = point;
            Damage = damage;
            Cause = cause;
        }

        public bool IsPlayer { get; }

        /// <summary>The shocked object; null when the victim is the player.</summary>
        public ISurface Surface { get; }

        public Vector3 Point { get; }

        /// <summary>$ (medical bill for the player).</summary>
        public int Damage { get; }

        public Cause Cause { get; }
    }
}
