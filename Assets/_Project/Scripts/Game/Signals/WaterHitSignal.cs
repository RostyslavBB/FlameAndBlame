using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.Signals
{
    /// <summary>Water from the hose hit a collider this frame (aggregated per collider).</summary>
    public readonly struct WaterHitSignal
    {
        public WaterHitSignal(Vector3 point, Vector3 normal, float amount, Collider target, Cause cause)
        {
            Point = point;
            Normal = normal;
            Amount = amount;
            Target = target;
            Cause = cause;
        }

        public Vector3 Point { get; }
        public Vector3 Normal { get; }

        /// <summary>Liters (abstract units) delivered by this hit.</summary>
        public float Amount { get; }

        /// <summary>What got wet. May belong to a prop, the floor or a wall.</summary>
        public Collider Target { get; }

        public Cause Cause { get; }
    }
}
