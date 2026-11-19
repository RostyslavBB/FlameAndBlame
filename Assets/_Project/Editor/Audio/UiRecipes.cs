using UnityEngine;

namespace FB.Editor.Audio
{
    /// <summary>UI blips: cash register "ka-ching", clock tick and pop.</summary>
    internal static class UiRecipes
    {
        /// <summary>Cash register "ka-ching": a click, then a bright bell chord.</summary>
        public static float[] Cash()
        {
            SoundSynth synth = new SoundSynth(7);
            float[] samples = SoundSynth.Buffer(0.6f);
            Biquad click = Biquad.BandPass(3200f, 2f);
            float[] partials = { 2093f, 2637f, 3136f, 4227f };
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                float ka = click.Process(synth.Noise()) * (t < 0.025f ? 1f : 0f);
                float ching = 0f;
                if (t > 0.055f)
                {
                    float local = t - 0.055f;
                    for (int p = 0; p < partials.Length; p++)
                    {
                        ching += Mathf.Sin(2f * Mathf.PI * partials[p] * local) * Mathf.Exp(-local * (5f + p * 2f)) / (p + 1f);
                    }
                }

                samples[i] = ka * 1.5f + ching;
            }

            return AmbienceRecipes.Finish(samples, 0.8f);
        }

        /// <summary>Clock tick: short high sine with an overtone.</summary>
        public static float[] Tick()
        {
            float[] samples = SoundSynth.Buffer(0.09f);
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                samples[i] = (Mathf.Sin(2f * Mathf.PI * 1250f * t) + 0.3f * Mathf.Sin(2f * Mathf.PI * 2500f * t)) * Mathf.Exp(-t * 55f);
            }

            return AmbienceRecipes.Finish(samples, 0.8f);
        }

        /// <summary>Bubbly "bloop" for things popping in.</summary>
        public static float[] Pop()
        {
            float[] samples = SoundSynth.Buffer(0.13f);
            float phase = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i) / 0.13f;
                phase += 2f * Mathf.PI * Mathf.Lerp(330f, 950f, t) / SoundSynth.Rate;
                samples[i] = Mathf.Sin(phase) * Mathf.Sin(t * Mathf.PI) * (1f - t * 0.5f);
            }

            return AmbienceRecipes.Finish(samples, 0.8f);
        }
    }
}
