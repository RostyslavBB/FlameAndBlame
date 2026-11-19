using UnityEngine;

namespace FB.Editor.Audio
{
    /// <summary>Noise-based sounds: hose hiss and fire crackle loops, ignition whoomph, steam, whoosh.</summary>
    internal static class AmbienceRecipes
    {
        /// <summary>Pressurized water: band-limited hiss with a slow surge and a bubbly flutter.</summary>
        public static float[] HoseLoop(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(2.3f);
            Biquad high = Biquad.HighPass(900f);
            Biquad low = Biquad.LowPass(6500f);
            Biquad body = Biquad.BandPass(1800f, 0.8f);
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                float noise = synth.Noise();
                float hiss = low.Process(high.Process(noise));
                float surge = 0.85f + 0.1f * Mathf.Sin(t * 2f * Mathf.PI * 0.45f) + 0.05f * Mathf.Sin(t * 2f * Mathf.PI * 23f);
                samples[i] = (hiss + body.Process(noise) * 0.6f) * surge;
            }

            samples = SoundSynth.MakeLoop(samples, 0.3f);
            SoundSynth.Normalize(samples, 0.7f);
            return samples;
        }

        /// <summary>Low rumble plus random crackles and the occasional pop.</summary>
        public static float[] FireLoop(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(3.4f);
            Biquad rumble = Biquad.LowPass(320f);
            Biquad crackleFilter = Biquad.HighPass(1400f);
            Biquad popFilter = Biquad.BandPass(500f, 1.2f);
            float crackle = 0f;
            float crackleDecay = 0f;
            float pop = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                if (synth.Chance(20f))
                {
                    crackle = synth.Range(0.25f, 1f);
                    crackleDecay = synth.Range(0.993f, 0.998f);
                }

                if (synth.Chance(2.2f))
                {
                    pop = synth.Range(0.5f, 1f);
                }

                float noise = synth.Noise();
                float bed = rumble.Process(noise) * (0.55f + 0.25f * Mathf.Sin(t * 2f * Mathf.PI * 0.7f));
                float crack = crackleFilter.Process(noise) * crackle;
                float popped = popFilter.Process(noise) * pop * 2.5f;
                crackle *= crackleDecay;
                pop *= 0.9985f;
                samples[i] = bed * 1.6f + crack + popped;
            }

            samples = SoundSynth.MakeLoop(samples, 0.35f);
            SoundSynth.Normalize(samples, 0.75f);
            return samples;
        }

        /// <summary>"Whoomph": filtered noise opening and closing over a dropping sub thump.</summary>
        public static float[] Ignite(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(0.9f);
            Biquad filter = Biquad.LowPass(200f);
            float phase = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i);
                float cutoff = t < 0.12f ? Mathf.Lerp(200f, 2600f, t / 0.12f) : Mathf.Lerp(2600f, 300f, (t - 0.12f) / 0.78f);
                filter.SetLowPass(cutoff, 1.1f);
                float envelope = Mathf.Min(1f, t / 0.03f) * Mathf.Exp(-t * 3.2f);
                phase += 2f * Mathf.PI * Mathf.Lerp(75f, 35f, t / 0.9f) / SoundSynth.Rate;
                samples[i] = filter.Process(synth.Noise()) * envelope + Mathf.Sin(phase) * 0.5f * Mathf.Exp(-t * 5f);
            }

            return Finish(samples, 0.9f);
        }

        /// <summary>"Pshhh": bright noise that fades out.</summary>
        public static float[] Steam(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(1.1f);
            Biquad high = Biquad.HighPass(2400f);
            Biquad low = Biquad.LowPass(9000f);
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i) / 1.1f;
                low.SetLowPass(Mathf.Lerp(9000f, 3500f, t));
                float envelope = Mathf.Min(1f, t * 40f) * Mathf.Pow(1f - t, 1.6f);
                samples[i] = low.Process(high.Process(synth.Noise())) * envelope;
            }

            return Finish(samples, 0.85f);
        }

        /// <summary>Band-pass sweep up and down under a bell envelope: a card flying past.</summary>
        public static float[] Whoosh(int seed)
        {
            SoundSynth synth = new SoundSynth(seed);
            float[] samples = SoundSynth.Buffer(0.55f);
            Biquad filter = Biquad.BandPass(400f, 1.4f);
            for (int i = 0; i < samples.Length; i++)
            {
                float t = SoundSynth.Time(i) / 0.55f;
                filter.SetBandPass(t < 0.45f ? Mathf.Lerp(400f, 2200f, t / 0.45f) : Mathf.Lerp(2200f, 600f, (t - 0.45f) / 0.55f), 1.4f);
                float bell = Mathf.Sin(t * Mathf.PI);
                samples[i] = filter.Process(synth.Noise()) * bell * bell;
            }

            return Finish(samples, 0.8f);
        }

        public static float[] Finish(float[] samples, float peak)
        {
            SoundSynth.FadeEdges(samples, 0.003f, 0.02f);
            SoundSynth.Normalize(samples, peak);
            return samples;
        }
    }
}
