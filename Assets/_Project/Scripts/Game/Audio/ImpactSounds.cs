using UnityEngine;

namespace FB.Game.Audio
{
    /// <summary>Maps prop hits to sound: louder with speed, deeper with mass; gentle bumps stay silent.</summary>
    public sealed class ImpactSounds : IImpactListener
    {
        private const float MinSpeed = 1.2f;
        private const float LoudSpeed = 6f;

        private readonly IAudioService _audio;

        public ImpactSounds(IAudioService audio)
        {
            _audio = audio;
        }

        public void OnImpact(Vector3 point, float speed, float mass)
        {
            if (speed < MinSpeed)
            {
                return;
            }

            float volume = Mathf.Lerp(0.25f, 1f, Mathf.InverseLerp(MinSpeed, LoudSpeed, speed));
            float pitch = Mathf.Lerp(1.4f, 0.65f, Mathf.InverseLerp(0.2f, 12f, mass));
            _audio.Play(SoundId.Impact, point, volume, pitch);
        }
    }
}
