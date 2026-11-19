using UnityEngine;
using Zenject;

namespace FB.Game.Audio
{
    /// <summary>
    /// Passive bridge for Unity's collision callback on loose props (it only exists on MonoBehaviours).
    /// The sound logic lives in <see cref="ImpactSounds"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ImpactSensor : MonoBehaviour
    {
        [SerializeField] private Rigidbody _body;

        private IImpactListener _listener;

        [Inject]
        public void Construct(IImpactListener listener) => _listener = listener;

        private void OnCollisionEnter(Collision collision)
        {
            if (_listener == null || collision.contactCount == 0)
            {
                return;
            }

            _listener.OnImpact(collision.GetContact(0).point, collision.relativeVelocity.magnitude, _body.mass);
        }
    }
}
