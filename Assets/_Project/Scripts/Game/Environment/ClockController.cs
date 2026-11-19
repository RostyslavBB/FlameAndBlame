using System;
using FB.Game.Level;
using UnityEngine;
using Zenject;

namespace FB.Game.Environment
{
    /// <summary>The kitchen clock shows the real local time (a tiny detail that makes the flat feel lived in).</summary>
    public sealed class ClockController : ITickable
    {
        private const float UpdateInterval = 1f;

        private readonly ClockView _view;
        private float _nextUpdate;

        public ClockController(LevelCatalog level)
        {
            _view = level.Clock;
        }

        public void Tick()
        {
            if (Time.unscaledTime < _nextUpdate)
            {
                return;
            }

            _nextUpdate = Time.unscaledTime + UpdateInterval;
            DateTime now = DateTime.Now;
            float minutes = now.Minute + now.Second / 60f;
            _view.SetTime(now.Hour + minutes / 60f, minutes);
        }
    }
}
