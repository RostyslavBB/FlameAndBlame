using System;
using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>
    /// Passive bridge for Unity's particle collision callback (it only exists on MonoBehaviours).
    /// The logic lives in <see cref="WaterJetCollisionHandler"/>.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class WaterJet : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;

        public event Action<GameObject> Collided;

        public ParticleSystem Particles => _particles;

        private void OnParticleCollision(GameObject other)
        {
            Collided?.Invoke(other);
        }
    }
}
