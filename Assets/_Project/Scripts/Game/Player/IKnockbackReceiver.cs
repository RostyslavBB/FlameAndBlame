using UnityEngine;

namespace FB.Game.Player
{
    /// <summary>Anything that can be pushed by hose recoil, electric shocks or explosions.</summary>
    public interface IKnockbackReceiver
    {
        /// <summary>Applies an instant impulse (N*s) in world space.</summary>
        void ApplyImpulse(Vector3 impulse);
    }
}
