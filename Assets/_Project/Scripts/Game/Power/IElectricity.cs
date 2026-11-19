using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.Power
{
    /// <summary>Which water on the floor is live, and which power source is to blame for it.</summary>
    public interface IElectricity
    {
        bool IsLive { get; }

        /// <summary>The wiring, or a short-circuited appliance (then it leads back to whoever watered it).</summary>
        Cause PowerCause { get; }

        bool IsElectrifiedAt(Vector3 world);
    }
}
