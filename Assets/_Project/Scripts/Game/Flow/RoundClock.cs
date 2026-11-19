using UnityEngine;

namespace FB.Game.Flow
{
    /// <summary>Countdown driven by <see cref="PlayingState"/>.</summary>
    public sealed class RoundClock : IRoundClock
    {
        public float Duration { get; private set; }
        public float Remaining { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsOver => Remaining <= 0f;

        public void Start(float duration)
        {
            Duration = duration;
            Remaining = duration;
            IsRunning = true;
        }

        public void Advance(float deltaTime)
        {
            if (IsRunning)
            {
                Remaining = Mathf.Max(0f, Remaining - deltaTime);
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
