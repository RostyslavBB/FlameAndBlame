using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>Single access point to the hose for other systems (HUD, camera FX, blame).</summary>
    public interface IHoseFacade
    {
        /// <summary>Current pressure, 0..1.</summary>
        float Pressure { get; }

        bool IsSpraying { get; }
        Vector3 NozzlePosition { get; }
        Vector3 SprayDirection { get; }

        /// <summary>Cause of the water currently leaving the nozzle (one per trigger press).</summary>
        Cause CurrentCause { get; }
    }
}
