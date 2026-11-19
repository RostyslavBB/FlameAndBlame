using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>
    /// A retriggerable "punch" for ticking presenters: <see cref="Trigger"/> restarts it, <see cref="Advance"/>
    /// returns a decaying 0..1 wobble. No allocations, no tasks: cheap enough for every HUD frame.
    /// </summary>
    public sealed class UiPulse
    {
        private readonly float _duration;
        private float _time;

        public UiPulse(float duration)
        {
            _duration = duration;
            _time = duration;
        }

        public bool Active => _time < _duration;

        public void Trigger() => _time = 0f;

        /// <summary>Steps by unscaled delta time; returns the wobble (0 when idle).</summary>
        public float Advance(float deltaTime)
        {
            if (_time >= _duration)
            {
                return 0f;
            }

            _time = Mathf.Min(_duration, _time + deltaTime);
            return UiTween.Punch(_time / _duration, 2f);
        }

        /// <summary>Linear fade from 1 (just triggered) to 0, for flashes.</summary>
        public float Fade => 1f - Mathf.Clamp01(_time / _duration);
    }
}
