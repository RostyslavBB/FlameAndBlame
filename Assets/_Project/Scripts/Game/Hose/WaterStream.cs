using FB.Game.Config;
using UnityEngine;
using Zenject;

namespace FB.Game.Hose
{
    /// <summary>
    /// One continuous jet built from the real water particles: they are grouped by age and the stream is drawn
    /// through the average position of each group (FB/WaterStream on a LineRenderer). So the stream bends and lags
    /// when the hose swings, exactly like the particles do, and ends where they hit something (they slow down).
    /// </summary>
    public sealed class WaterStream : IInitializable, ILateTickable
    {
        private const int Bins = 18;
        private const float MinPressure = 0.03f;
        private const float StreamAgeShare = 0.6f;
        private const float HitSpeedShare = 0.5f;

        private readonly HoseView _view;
        private readonly IHoseFacade _hose;
        private readonly HoseConfig _config;
        private readonly Vector3[] _points = new Vector3[Bins + 1];
        private readonly Vector3[] _positionSums = new Vector3[Bins];
        private readonly float[] _speedSums = new float[Bins];
        private readonly int[] _counts = new int[Bins];
        private ParticleSystem.Particle[] _particles;
        private ParticleSystem _jet;
        private LineRenderer _line;

        public WaterStream(HoseView view, IHoseFacade hose, HoseConfig config)
        {
            _view = view;
            _hose = hose;
            _config = config;
        }

        public void Initialize()
        {
            _jet = _view.Jet.Particles;
            _particles = new ParticleSystem.Particle[_config.MaxParticles];
            _line = _view.Stream;
            _line.useWorldSpace = true;
            _line.enabled = false;
        }

        public void LateTick()
        {
            float pressure = _hose.Pressure;
            int count = pressure < MinPressure ? 0 : BuildFromParticles(pressure);
            _line.enabled = count > 1;
            if (count <= 1)
            {
                return;
            }

            _line.positionCount = count;
            _line.SetPositions(_points);
            float width = Mathf.Lerp(0.05f, 0.15f, pressure);
            _line.startWidth = width;
            _line.endWidth = width * 2.8f;
            Color color = new Color(1f, 1f, 1f, Mathf.Clamp01(pressure * 1.6f));
            _line.startColor = color;
            _line.endColor = color;
        }

        private int BuildFromParticles(float pressure)
        {
            System.Array.Clear(_positionSums, 0, Bins);
            System.Array.Clear(_speedSums, 0, Bins);
            System.Array.Clear(_counts, 0, Bins);

            float maxAge = _config.ParticleLifetime * StreamAgeShare;
            int alive = _jet.GetParticles(_particles);
            for (int i = 0; i < alive; i++)
            {
                ParticleSystem.Particle particle = _particles[i];
                float age = particle.startLifetime - particle.remainingLifetime;
                if (age < 0f || age >= maxAge)
                {
                    continue;
                }

                int bin = Mathf.Min(Bins - 1, (int)(age / maxAge * Bins));
                _positionSums[bin] += particle.position;
                _speedSums[bin] += particle.velocity.magnitude;
                _counts[bin]++;
            }

            float jetSpeed = Mathf.Lerp(_config.MinJetSpeed, _config.MaxJetSpeed, pressure);
            int points = 0;
            _points[points++] = _hose.NozzlePosition;
            for (int bin = 0; bin < Bins; bin++)
            {
                if (_counts[bin] == 0)
                {
                    continue;
                }

                _points[points++] = _positionSums[bin] / _counts[bin];
                // Particles that hit something lose speed: the stream ends at the impact.
                if (_speedSums[bin] / _counts[bin] < jetSpeed * HitSpeedShare)
                {
                    break;
                }
            }

            return points;
        }
    }
}
