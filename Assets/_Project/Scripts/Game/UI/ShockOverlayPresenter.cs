using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>
    /// When the player gets shocked: lightning flickers over the screen, the image jumps and distorts,
    /// then everything decays. A new shock restarts the effect; the scene lifetime cancels it.
    /// </summary>
    public sealed class ShockOverlayPresenter : IInitializable, IDisposable
    {
        private const float Duration = 0.75f;
        private const float FlickerRate = 28f;
        private const float JitterPixels = 18f;

        private readonly ShockOverlayView _view;
        private readonly SignalBus _signalBus;
        private CancellationTokenSource _effect;

        public ShockOverlayPresenter(ShockOverlayView view, SignalBus signalBus)
        {
            _view = view;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _view.Hide();
            _signalBus.Subscribe<ElectrocutionSignal>(OnElectrocution);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<ElectrocutionSignal>(OnElectrocution);
            Cancel();
        }

        private void OnElectrocution(ElectrocutionSignal signal)
        {
            if (!signal.IsPlayer)
            {
                return;
            }

            Cancel();
            _effect = new CancellationTokenSource();
            Play(_effect.Token).Forget();
        }

        private async UniTaskVoid Play(CancellationToken token)
        {
            try
            {
                int lastFrame = -1;
                bool flipX = false;
                bool flipY = false;
                await UiTween.Run(Duration, t =>
                {
                    float decay = (1f - t) * (1f - t);
                    int frame = (int)(t * Duration * FlickerRate);
                    if (frame != lastFrame)
                    {
                        lastFrame = frame;
                        flipX = UnityEngine.Random.value > 0.5f;
                        flipY = UnityEngine.Random.value > 0.5f;
                    }

                    bool on = frame % 3 != 2;
                    Vector2 jitter = UnityEngine.Random.insideUnitCircle * (JitterPixels * decay);
                    _view.Show(on ? decay : decay * 0.25f, decay * 0.45f, decay, jitter, flipX, flipY);
                }, token);
                _view.Hide();
            }
            catch (OperationCanceledException)
            {
                // A newer shock restarted the effect or the scene is being disposed.
            }
        }

        private void Cancel()
        {
            if (_effect == null)
            {
                return;
            }

            _effect.Cancel();
            _effect.Dispose();
            _effect = null;
        }
    }
}
