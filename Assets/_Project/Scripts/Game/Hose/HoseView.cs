using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>Passive references to the hose visuals on the player prefab. No logic.</summary>
    public sealed class HoseView : MonoBehaviour
    {
        [SerializeField] private Transform _nozzleTip;
        [SerializeField] private Transform _hoseAttach;
        [SerializeField] private WaterJet _jet;
        [SerializeField] private ParticleSystem _mist;
        [SerializeField] private LineRenderer _line;
        [SerializeField] private LineRenderer _stream;
        [SerializeField] private CapsuleCollider _ropeProbe;

        /// <summary>Where the water leaves the nozzle.</summary>
        public Transform NozzleTip => _nozzleTip;

        /// <summary>Back of the nozzle, where the hose is connected.</summary>
        public Transform HoseAttach => _hoseAttach;

        public WaterJet Jet => _jet;

        /// <summary>Soft spray cloud around the jet (fire-hose look).</summary>
        public ParticleSystem Mist => _mist;
        public LineRenderer Line => _line;

        /// <summary>Continuous water jet (FB/WaterStream).</summary>
        public LineRenderer Stream => _stream;

        /// <summary>Trigger capsule used only as a shape for rope segment penetration queries.</summary>
        public CapsuleCollider RopeProbe => _ropeProbe;
    }
}
