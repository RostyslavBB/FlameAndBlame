using UnityEngine;

namespace FB.Game.Player
{
    /// <summary>
    /// Passive scene-side part of the player: holds the physics body and camera rig.
    /// Movement and look logic live in plain C# systems (<see cref="PlayerMovement"/>, <see cref="PlayerLook"/>).
    /// </summary>
    public sealed class PlayerBody : MonoBehaviour, IKnockbackReceiver
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private Camera _camera;

        public Rigidbody Rigidbody => _rigidbody;
        public CapsuleCollider Collider => _collider;
        public Transform CameraPivot => _cameraPivot;
        public Camera Camera => _camera;

        public void ApplyImpulse(Vector3 impulse)
        {
            _rigidbody.AddForce(impulse, ForceMode.Impulse);
        }
    }
}
