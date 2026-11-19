using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>
    /// Minimal tween on UniTask with unscaled time (UI keeps animating during hit-stop and slow-mo).
    /// Always cancellable: callers pass the token of their presenter or entry.
    /// </summary>
    public static class UiTween
    {
        public static async UniTask Run(float duration, Action<float> step, CancellationToken token)
        {
            float time = 0f;
            while (time < duration)
            {
                step(time / duration);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                time += Time.unscaledDeltaTime;
            }

            step(1f);
        }

        public static UniTask Wait(float seconds, CancellationToken token) =>
            UniTask.Delay(TimeSpan.FromSeconds(seconds), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, token);

        public static float EaseOutBack(float t)
        {
            const float overshoot = 1.70158f;
            float x = t - 1f;
            return 1f + (overshoot + 1f) * x * x * x + overshoot * x * x;
        }
    }
}
