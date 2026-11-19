using UnityEngine;
using UnityEngine.AI;

namespace FB.Game.Civilian
{
    /// <summary>
    /// Passive scene part of the neighbour: the agent, the hit collider, the limb pivots animated by
    /// <see cref="CivilianAnimator"/>, normal and dizzy eyes, the rooms he wanders between and the baked nav mesh.
    /// </summary>
    public sealed class CivilianView : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _armLeft;
        [SerializeField] private Transform _armRight;
        [SerializeField] private Transform _legLeft;
        [SerializeField] private Transform _legRight;
        [SerializeField] private Transform _mouth;
        [SerializeField] private GameObject _eyes;
        [SerializeField] private GameObject _dizzyEyes;
        [SerializeField] private Vector3[] _waypoints;
        [SerializeField] private NavMeshData _navMesh;

        public NavMeshAgent Agent => _agent;
        public Collider Collider => _collider;
        public Transform Body => _body;
        public Transform Head => _head;
        public Transform ArmLeft => _armLeft;
        public Transform ArmRight => _armRight;
        public Transform LegLeft => _legLeft;
        public Transform LegRight => _legRight;
        public Transform Mouth => _mouth;
        public Vector3[] Waypoints => _waypoints;
        public NavMeshData NavMesh => _navMesh;

        public void SetDizzy(bool dizzy)
        {
            _eyes.SetActive(!dizzy);
            _dizzyEyes.SetActive(dizzy);
        }
    }
}
