using System;
using UnityEngine;

namespace FB.Game.Audio
{
    /// <summary>Clips and mixing per sound: variants, volume, random pitch, anti-spam cooldown, 2D or 3D.</summary>
    [CreateAssetMenu(menuName = "FB/Audio Config", fileName = "AudioConfig")]
    public sealed class AudioConfig : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float _masterVolume = 0.8f;
        [SerializeField, Min(1f)] private float _maxDistance = 14f;
        [SerializeField] private SoundEntry[] _sounds = Array.Empty<SoundEntry>();

        public float MasterVolume => _masterVolume;
        public float MaxDistance => _maxDistance;

        public bool TryGet(SoundId id, out SoundEntry entry)
        {
            for (int i = 0; i < _sounds.Length; i++)
            {
                if (_sounds[i].Id == id)
                {
                    entry = _sounds[i];
                    return entry.Clips != null && entry.Clips.Length > 0;
                }
            }

            entry = null;
            return false;
        }

#if UNITY_EDITOR
        public void EditorSetSounds(SoundEntry[] sounds) => _sounds = sounds;
#endif
    }
}
