using UnityEngine;

namespace FB.Editor.Audio
{
    /// <summary>
    /// RBJ biquad filter (low-pass, high-pass, band-pass). Coefficients can be changed every sample for sweeps;
    /// the state is kept, so sweeps stay smooth.
    /// </summary>
    internal struct Biquad
    {
        private float _b0;
        private float _b1;
        private float _b2;
        private float _a1;
        private float _a2;
        private float _x1;
        private float _x2;
        private float _y1;
        private float _y2;

        public static Biquad LowPass(float frequency, float q = 0.707f)
        {
            Biquad filter = default;
            filter.SetLowPass(frequency, q);
            return filter;
        }

        public static Biquad HighPass(float frequency, float q = 0.707f)
        {
            Biquad filter = default;
            filter.SetHighPass(frequency, q);
            return filter;
        }

        public static Biquad BandPass(float frequency, float q)
        {
            Biquad filter = default;
            filter.SetBandPass(frequency, q);
            return filter;
        }

        public void SetLowPass(float frequency, float q = 0.707f)
        {
            Prepare(frequency, q, out float cos, out float alpha);
            Set((1f - cos) * 0.5f, 1f - cos, (1f - cos) * 0.5f, 1f + alpha, -2f * cos, 1f - alpha);
        }

        public void SetHighPass(float frequency, float q = 0.707f)
        {
            Prepare(frequency, q, out float cos, out float alpha);
            Set((1f + cos) * 0.5f, -(1f + cos), (1f + cos) * 0.5f, 1f + alpha, -2f * cos, 1f - alpha);
        }

        public void SetBandPass(float frequency, float q)
        {
            Prepare(frequency, q, out float cos, out float alpha);
            Set(alpha, 0f, -alpha, 1f + alpha, -2f * cos, 1f - alpha);
        }

        public float Process(float x)
        {
            float y = _b0 * x + _b1 * _x1 + _b2 * _x2 - _a1 * _y1 - _a2 * _y2;
            _x2 = _x1;
            _x1 = x;
            _y2 = _y1;
            _y1 = y;
            return y;
        }

        private static void Prepare(float frequency, float q, out float cos, out float alpha)
        {
            float omega = 2f * Mathf.PI * Mathf.Clamp(frequency, 10f, SoundSynth.Rate * 0.45f) / SoundSynth.Rate;
            cos = Mathf.Cos(omega);
            alpha = Mathf.Sin(omega) / (2f * q);
        }

        private void Set(float b0, float b1, float b2, float a0, float a1, float a2)
        {
            _b0 = b0 / a0;
            _b1 = b1 / a0;
            _b2 = b2 / a0;
            _a1 = a1 / a0;
            _a2 = a2 / a0;
        }
    }
}
