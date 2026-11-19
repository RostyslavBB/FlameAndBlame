using UnityEngine;

namespace FB.Game.Audio
{
    /// <summary>Fire-and-forget sounds from a fixed pool of voices, plus two continuous loops (hose, fire).</summary>
    public interface IAudioService
    {
        /// <summary>Plays a 3D sound at a world position (2D if the sound is configured as non-spatial).</summary>
        void Play(SoundId id, Vector3 position, float volume = 1f, float pitch = 1f);

        /// <summary>Plays a flat UI sound.</summary>
        void PlayUi(SoundId id, float volume = 1f, float pitch = 1f);

        /// <summary>Drives a looping sound; volume 0 pauses it.</summary>
        void SetLoop(SoundId id, float volume, float pitch, Vector3 position);
    }
}
