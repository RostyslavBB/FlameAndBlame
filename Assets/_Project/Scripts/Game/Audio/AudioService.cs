using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace FB.Game.Audio
{
    /// <summary>
    /// Plays sounds on a fixed pool of pre-built voices (the oldest one is stolen when all are busy), with random
    /// clip variants, random pitch and a per-sound cooldown so dozens of simultaneous hits stay one clean sound.
    /// </summary>
    public sealed class AudioService : IAudioService, IInitializable
    {
        private readonly AudioRig _rig;
        private readonly AudioConfig _config;
        private readonly Dictionary<SoundId, float> _nextAllowed = new Dictionary<SoundId, float>();
        private int _nextVoice;

        public AudioService(AudioRig rig, AudioConfig config)
        {
            _rig = rig;
            _config = config;
        }

        public void Initialize()
        {
            foreach (AudioSource voice in _rig.Voices)
            {
                voice.maxDistance = _config.MaxDistance;
            }

            Prepare(_rig.HoseLoop, SoundId.HoseLoop);
            Prepare(_rig.FireLoop, SoundId.FireLoop);
        }

        public void Play(SoundId id, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (!TryStart(id, out SoundEntry entry))
            {
                return;
            }

            AudioSource voice = NextVoice();
            voice.transform.position = position;
            voice.spatialBlend = entry.Spatial ? 1f : 0f;
            // Play() rather than PlayOneShot: isPlaying then tells which voices are free, and Stop() can steal one.
            voice.clip = PickClip(entry);
            voice.pitch = pitch * Random.Range(entry.Pitch.x, entry.Pitch.y);
            voice.volume = Mathf.Clamp01(volume) * entry.Volume * _config.MasterVolume;
            voice.Play();
        }

        public void PlayUi(SoundId id, float volume = 1f, float pitch = 1f)
        {
            if (TryStart(id, out SoundEntry entry))
            {
                Fire(_rig.Ui, entry, volume, pitch);
            }
        }

        public void SetLoop(SoundId id, float volume, float pitch, Vector3 position)
        {
            AudioSource source = id == SoundId.FireLoop ? _rig.FireLoop : _rig.HoseLoop;
            if (source.clip == null || !_config.TryGet(id, out SoundEntry entry))
            {
                return;
            }

            source.transform.position = position;
            source.volume = Mathf.Clamp01(volume) * entry.Volume * _config.MasterVolume;
            source.pitch = pitch;
            if (volume > 0.001f && !source.isPlaying)
            {
                source.Play();
            }
            else if (volume <= 0.001f && source.isPlaying)
            {
                source.Pause();
            }
        }

        private bool TryStart(SoundId id, out SoundEntry entry)
        {
            if (!_config.TryGet(id, out entry))
            {
                return false;
            }

            float now = Time.unscaledTime;
            if (_nextAllowed.TryGetValue(id, out float allowed) && now < allowed)
            {
                return false;
            }

            _nextAllowed[id] = now + entry.Cooldown;
            return true;
        }

        private void Fire(AudioSource source, SoundEntry entry, float volume, float pitch)
        {
            source.pitch = pitch * Random.Range(entry.Pitch.x, entry.Pitch.y);
            source.PlayOneShot(PickClip(entry), Mathf.Clamp01(volume) * entry.Volume * _config.MasterVolume);
        }

        private static AudioClip PickClip(SoundEntry entry) => entry.Clips[Random.Range(0, entry.Clips.Length)];

        private AudioSource NextVoice()
        {
            AudioSource[] voices = _rig.Voices;
            for (int i = 0; i < voices.Length; i++)
            {
                int index = (_nextVoice + i) % voices.Length;
                if (!voices[index].isPlaying)
                {
                    _nextVoice = (index + 1) % voices.Length;
                    return voices[index];
                }
            }

            AudioSource stolen = voices[_nextVoice];
            stolen.Stop();
            _nextVoice = (_nextVoice + 1) % voices.Length;
            return stolen;
        }

        private void Prepare(AudioSource source, SoundId id)
        {
            if (_config.TryGet(id, out SoundEntry entry))
            {
                source.clip = entry.Clips[0];
                source.loop = true;
                source.volume = 0f;
                source.maxDistance = _config.MaxDistance;
            }
        }
    }
}
