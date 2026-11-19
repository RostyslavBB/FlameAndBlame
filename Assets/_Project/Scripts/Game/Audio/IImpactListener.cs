using UnityEngine;

namespace FB.Game.Audio
{
    /// <summary>Receives physical hits of loose props (forwarded by <see cref="ImpactSensor"/>).</summary>
    public interface IImpactListener
    {
        void OnImpact(Vector3 point, float speed, float mass);
    }
}
