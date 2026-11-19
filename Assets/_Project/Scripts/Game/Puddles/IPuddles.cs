using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.Puddles
{
    /// <summary>Read access to the water on the floor.</summary>
    public interface IPuddles
    {
        WaterGrid Grid { get; }

        /// <summary>Water depth (m) under a world point.</summary>
        float DepthAt(Vector3 world);

        /// <summary>True if the point stands in a puddle (thicker than half the film).</summary>
        bool IsWet(Vector3 world);

        /// <summary>Cause(Puddle) of the water under a point, or null when dry.</summary>
        Cause CauseAt(Vector3 world);
    }
}
