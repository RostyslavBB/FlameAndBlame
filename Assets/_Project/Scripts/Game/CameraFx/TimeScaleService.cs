using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Game.Flow;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.CameraFx
{
    /// <summary>
    /// Hit-stop then slow-mo on severe blame events. A newer event cancels the running pulse; disposing the scene
    /// (restart) or reaching the report always restores time scale 1.
    /// </summary>
    public sealed class TimeScaleService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly JuiceConfig _config;
        private CancellationTokenSource _pulse;

        public TimeScaleService(SignalBus signalBus, JuiceConfig config)
        {
            _signalBus = signalBus;
            _config = config;
        }

        public void Initialize()
        {
            Time.timeScale = 1f;
            _signalBus.Subscribe<BlameEventSignal>(OnBlameEvent);
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BlameEventSignal>(OnBlameEvent);
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
            CancelPulse();
            Time.timeScale = 1f;
        }

        private void OnBlameEvent(BlameEventSignal signal)
        {
            if (signal.Severity < _config.MinSeverity)
            {
                return;
            }

            CancelPulse();
            _pulse = new CancellationTokenSource();
            Pulse(_pulse.Token).Forget();
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            if (signal.Phase == GamePhase.Report)
            {
                CancelPulse();
                Time.timeScale = 1f;
            }
        }

        private async UniTaskVoid Pulse(CancellationToken token)
        {
            try
            {
                Time.timeScale = _config.HitStopTimeScale;
                await Wait(_config.HitStopSeconds, token);
                Time.timeScale = _config.SlowMoTimeScale;
                await Wait(_config.SlowMoSeconds, token);
                Time.timeScale = 1f;
            }
            catch (OperationCanceledException)
            {
                // A newer pulse or disposal took over the time scale.
            }
        }

        private static UniTask Wait(float seconds, CancellationToken token) =>
            UniTask.Delay(TimeSpan.FromSeconds(seconds), DelayType.Realtime, PlayerLoopTiming.Update, token);

        private void CancelPulse()
        {
            if (_pulse == null)
            {
                return;
            }

            _pulse.Cancel();
            _pulse.Dispose();
            _pulse = null;
        }
    }
}
