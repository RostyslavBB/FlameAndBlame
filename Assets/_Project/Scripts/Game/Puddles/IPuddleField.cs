using System.Collections.Generic;
using UnityEngine;

namespace FB.Game.Puddles
{
    /// <summary>Read access to the puddles on the floor.</summary>
    public interface IPuddleField
    {
        /// <summary>Active puddles; valid until the next tick.</summary>
        IReadOnlyList<Puddle> Active { get; }

        /// <summary>The puddle under a point (XZ), or null.</summary>
        Puddle FindAt(Vector3 point);
    }
}
