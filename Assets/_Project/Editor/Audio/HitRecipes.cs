using UnityEngine;

namespace FB.Editor.Audio
{
    /// <summary>Short percussive sounds: prop impacts, electric zap, short-circuit sparks, the rubber stamp.</summary>
    internal static class HitRecipes
    {
        /// <summary>Wooden "tok": a dropping sine thump, a click and a resonant knock.</summary>
        public static float[] Impact(int seed, float baseFrequency)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(0.32f);
            Biquad click = Biquad.LowPass(2200f);
            Biquad wood = Biquad.BandPass(baseFrequency * 4f, 5f);
            float phase = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                phase += 2f * Mathf.PI * Mathf.Lerp(baseFrequency, baseFrequency * 0.5f, Mathf.Min(1f, t * 6f)) / SoundSynth.Rate;
                float noise = synth.Noise();
                samples[i] = Mathf.Sin(phase) * Mathf.Exp(-t * 16f)
                    + click.Process(noise) * Mathf.Exp(-t * 70f) * 0.8f
                    + wood.Process(noise) * Mathf.Exp(-t * 22f) * 2.2f;
            }

            return AmbienceRecipes.Finish(samples, 0.9f);
        }

        /// <summary>Stuttering mains buzz with crackle: square + saw around 110 Hz, randomly chopped.</summary>
        public static float[] Zap(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(0.6f);
            Biquad crackleFilter = Biquad.HighPass(2500f);
            Biquad tone = Biquad.LowPass(4000f);
            float phase = 0f;
            float frequency = 110f;
            float gate = 1f;
            int nextGate = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                if (i >= nextGate)
                {
                    nextGate = i + (int)(synth.Range(0.012f, 0.04f) * SoundSynth.Rate);
                    gate = synth.Range(0.25f, 1f);
                    frequency = Mathf.Clamp(frequency + synth.Range(-25f, 25f), 80f, 160f);
                }

                phase = (phase + frequency / SoundSynth.Rate) % 1f;
                float square = phase < 0.5f ? 1f : -1f;
                float saw = phase * 2f - 1f;
                float buzz = tone.Process(square * 0.6f + saw * 0.5f);
                float crackle = crackleFilter.Process(synth.Noise()) * (synth.Chance(60f) ? 4f : 0.6f);
                samples[i] = (buzz + crackle * 0.5f) * gate * Mathf.Exp(-t * 2.5f);
            }

            return AmbienceRecipes.Finish(samples, 0.85f);
        }

        /// <summary>Dense crackle burst with a falling electronic chirp.</summary>
        public static float[] Spark(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(0.45f);
            Biquad high = Biquad.HighPass(3000f);
            float burst = 0f;
            float phase = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                if (synth.Chance(Mathf.Lerp(160f, 20f, t / 0.45f)))
                {
                    burst = synth.Range(0.4f, 1f);
                }

                burst *= 0.994f;
                phase += 2f * Mathf.PI * Mathf.Lerp(2200f, 900f, t / 0.45f) / SoundSynth.Rate;
                samples[i] = high.Process(synth.Noise()) * burst + Mathf.Sin(phase) * 0.15f * Mathf.Exp(-t * 6f);
            }

            return AmbienceRecipes.Finish(samples, 0.85f);
        }

        /// <summary>Heavy "THUNK" of the rubber stamp hitting the bill.</summary>
        public static float[] Stamp(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(0.5f);
            Biquad slam = Biquad.LowPass(900f);
            float phase = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                phase += 2f * Mathf.PI * Mathf.Lerp(62f, 38f, t / 0.5f) / SoundSynth.Rate;
                samples[i] = Mathf.Sin(phase) * Mathf.Exp(-t * 9f) + slam.Process(synth.Noise()) * Mathf.Exp(-t * 28f) * 1.4f;
            }

            return AmbienceRecipes.Finish(samples, 0.95f);
        }
    }
}
