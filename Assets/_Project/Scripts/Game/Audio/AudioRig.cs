using UnityEngine;

namespace FB.Game.Audio
{
    /// <summary>Passive holder of the pre-built audio sources (built by Build All, never instantiated at runtime).</summary>
    public sealed class AudioRig : MonoBehaviour
    {
        [SerializeField] private AudioSource[] _voices;
        [SerializeField] private AudioSource _ui;
        [SerializeField] private AudioSource _hoseLoop;
        [SerializeField] private AudioSource _fireLoop;

        public AudioSource[] Voices => _voices;
        public AudioSource Ui => _ui;
        public AudioSource HoseLoop => _hoseLoop;
        public AudioSource FireLoop => _fireLoop;
    }
}
