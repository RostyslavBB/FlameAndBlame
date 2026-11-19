using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>
    /// Minimal tween on UniTask with unscaled time (UI keeps animating during hit-stop and slow-mo), plus the
    /// easing curves the presenters use. Always cancellable: callers pass the token of their presenter or entry.
    /// </summary>
    public static class UiTween
    {
        private const float MaxStep = 0.1f;

        /// <summary>
        /// Unscaled delta time capped per frame: the first frame after a load can report seconds, which would skip
        /// an animation entirely.
        /// </summary>
        public static float DeltaTime => Mathf.Min(Time.unscaledDeltaTime, MaxStep);

        public static async UniTask Run(float duration, Action<float> step, CancellationToken token)
        {
            float time = 0f;
            while (time < duration)
            {
                step(time / duration);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                time += DeltaTime;
            }

            step(1f);
        }

        /// <summary>Calls <paramref name="step"/> with the elapsed time every frame until cancelled (idle loops).</summary>
        public static async UniTask Loop(Action<float> step, CancellationToken token)
        {
            float time = 0f;
            while (true)
            {
                step(time);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                time += DeltaTime;
            }
        }

        public static UniTask Wait(float seconds, CancellationToken token) =>
            UniTask.Delay(TimeSpan.FromSeconds(seconds), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, token);

        public static float EaseOutBack(float t)
        {
            const float overshoot = 1.70158f;
            float x = t - 1f;
            return 1f + (overshoot + 1f) * x * x * x + overshoot * x * x;
        }

        public static float EaseOutCubic(float t)
        {
            float x = 1f - t;
            return 1f - x * x * x;
        }

        public static float EaseInCubic(float t) => t * t * t;

        /// <summary>Springy settle: overshoots a few times and rests at 1.</summary>
        public static float EaseOutElastic(float t)
        {
            if (t <= 0f || t >= 1f)
            {
                return Mathf.Clamp01(t);
            }

            const float period = 2f * Mathf.PI / 3f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * period) + 1f;
        }

        /// <summary>0 -> 1 -> 0 decaying wobble, for punches and shakes (t in 0..1).</summary>
        public static float Punch(float t, float wobbles = 3f) =>
            Mathf.Sin(t * Mathf.PI * wobbles) * (1f - t) * (1f - t);
    }
}
