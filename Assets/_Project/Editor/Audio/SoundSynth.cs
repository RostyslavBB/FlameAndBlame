using UnityEngine;

namespace FB.Editor.Audio
{
    /// <summary>Tiny offline synthesizer: seeded noise, buffers and finishing passes (normalize, fades, seamless loops).</summary>
    internal sealed class SoundSynth
    {
        public const int Rate = 22050;

        private readonly System.Random _random;

        public SoundSynth(int seed)
        {
            _random = new System.Random(seed);
        }

        public float Noise() => (float)(_random.NextDouble() * 2.0 - 1.0);

        public float Range(float min, float max) => min + (float)_random.NextDouble() * (max - min);

        public bool Chance(float perSecond) => _random.NextDouble() < perSecond / Rate;

        public static float[] Buffer(float seconds) => new float[Mathf.CeilToInt(seconds * Rate)];

        public static float Time(int sample) => sample / (float)Rate;

        public static void Normalize(float[] samples, float peak)
        {
            float max = 0.0001f;
            for (int i = 0; i < samples.Length; i++)
            {
                max = Mathf.Max(max, Mathf.Abs(samples[i]));
            }

            float gain = peak / max;
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= gain;
            }
        }

        /// <summary>Short fades at both ends so one-shots never click.</summary>
        public static void FadeEdges(float[] samples, float fadeIn, float fadeOut)
        {
            int inCount = Mathf.Min(samples.Length, Mathf.CeilToInt(fadeIn * Rate));
            int outCount = Mathf.Min(samples.Length, Mathf.CeilToInt(fadeOut * Rate));
            for (int i = 0; i < inCount; i++)
            {
                samples[i] *= i / (float)inCount;
            }

            for (int i = 0; i < outCount; i++)
            {
                samples[samples.Length - 1 - i] *= i / (float)outCount;
            }
        }

        /// <summary>Crossfades the tail into the head and cuts the tail off: the result loops without a seam.</summary>
        public static float[] MakeLoop(float[] samples, float crossfade)
        {
            int fade = Mathf.Min(samples.Length / 3, Mathf.CeilToInt(crossfade * Rate));
            int length = samples.Length - fade;
            float[] loop = new float[length];
            System.Array.Copy(samples, loop, length);
            for (int i = 0; i < fade; i++)
            {
                float t = i / (float)fade;
                loop[i] = samples[i] * t + samples[length + i] * (1f - t);
            }

            return loop;
        }
    }
}
