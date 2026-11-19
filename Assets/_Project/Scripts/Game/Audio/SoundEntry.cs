using System;
using UnityEngine;

namespace FB.Game.Audio
{
    /// <summary>One sound of the <see cref="AudioConfig"/>.</summary>
    [Serializable]
    public sealed class SoundEntry
    {
        [SerializeField] private SoundId _id;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField] private Vector2 _pitch = new Vector2(0.95f, 1.05f);
        [Tooltip("Minimum seconds between two plays of this sound (keeps impacts and crackles from stacking).")]
        [SerializeField, Min(0f)] private float _cooldown;
        [SerializeField] private bool _spatial = true;

        public SoundEntry(SoundId id, AudioClip[] clips, float volume, Vector2 pitch, float cooldown, bool spatial)
        {
            _id = id;
            _clips = clips;
            _volume = volume;
            _pitch = pitch;
            _cooldown = cooldown;
            _spatial = spatial;
        }

        public SoundId Id => _id;
        public AudioClip[] Clips => _clips;
        public float Volume => _volume;
        public Vector2 Pitch => _pitch;
        public float Cooldown => _cooldown;
        public bool Spatial => _spatial;
    }
}
