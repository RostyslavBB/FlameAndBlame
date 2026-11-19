using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Core.Pooling;
using FB.Game.Audio;
using FB.Game.Flow;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>
    /// Blame feed: notable BlameEventSignals become pooled lines on top (small ones skipped, spaced by a cooldown);
    /// at most N lines; top-severity events shake.
    /// Each line owns a CancellationTokenSource, cancelled when it is pushed out or the scene is disposed.
    /// </summary>
    public sealed class FeedPresenter : IInitializable, IDisposable
    {
        private readonly FeedView _view;
        private readonly IPool<FeedEntryView> _pool;
        private readonly SignalBus _signalBus;
        private readonly UiConfig _config;
        private readonly IAudioService _audio;
        private const float FlyInPixels = 460f;
        private const float CountSeconds = 0.45f;
        private const int TopSeverity = 3;

        private readonly List<Line> _lines = new List<Line>();
        private float _nextLineTime;

        public FeedPresenter(FeedView view, IPool<FeedEntryView> pool, SignalBus signalBus, UiConfig config,
            IAudioService audio)
        {
            _view = view;
            _pool = pool;
            _signalBus = signalBus;
            _config = config;
            _audio = audio;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<BlameEventSignal>(OnBlameEvent);
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BlameEventSignal>(OnBlameEvent);
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
            for (int i = _lines.Count - 1; i >= 0; i--)
            {
                _lines[i].Cancellation.Cancel();
                _lines[i].Cancellation.Dispose();
            }

            _lines.Clear();
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            _view.SetVisible(signal.Phase != GamePhase.Report);
        }

        private void OnBlameEvent(BlameEventSignal signal)
        {
            if (_config.IsHiddenInFeed(signal.Kind) || !ShouldShow(signal.Severity))
            {
                return;
            }

            if (_lines.Count >= _config.FeedMaxEntries)
            {
                Remove(_lines[0]);
            }

            FeedEntryView entry = _pool.Spawn();
            entry.transform.SetParent(_view.Container, false);
            entry.transform.SetAsFirstSibling();
            entry.Set(signal.ActorName, signal.ActorColor, signal.Text);
            entry.SetDamage(null);

            Line line = new Line(entry, new CancellationTokenSource());
            _lines.Add(line);
            _audio.PlayUi(SoundId.FeedLine, 0.8f, 0.9f + signal.Severity * 0.08f);
            Animate(line, signal.Severity, signal.Damage).Forget();
        }

        /// <summary>
        /// Keeps the feed readable: small events are skipped and lines are spaced out; only top-severity events
        /// may break the cooldown. Filtering is UI-only: the ledger and the bill still count everything.
        /// </summary>
        private bool ShouldShow(int severity)
        {
            if (severity < _config.FeedMinSeverity)
            {
                return false;
            }

            float now = Time.unscaledTime;
            if (now < _nextLineTime && severity < TopSeverity)
            {
                return false;
            }

            _nextLineTime = now + _config.FeedCooldownSeconds;
            return true;
        }

        private async UniTaskVoid Animate(Line line, int severity, int damage)
        {
            FeedEntryView entry = line.Entry;
            CancellationToken token = line.Cancellation.Token;
            try
            {
                // Fly in from the right with a little tilt, flashing white as it lands.
                await UiTween.Run(_config.FeedPopSeconds, t =>
                {
                    float eased = UiTween.EaseOutBack(t);
                    entry.SetAlpha(Mathf.Clamp01(t * 2.5f));
                    entry.SetOffset(new Vector2(Mathf.LerpUnclamped(FlyInPixels, 0f, eased), 0f));
                    entry.SetTilt(Mathf.LerpUnclamped(-6f, 0f, eased));
                    entry.SetFlash(0.8f * (1f - t));
                }, token);

                if (damage > 0)
                {
                    await CountDamage(entry, damage, token);
                }

                if (severity >= _config.FeedShakeMinSeverity)
                {
                    float pixels = _config.FeedShakePixels * (severity - _config.FeedShakeMinSeverity + 1);
                    await UiTween.Run(_config.FeedShakeSeconds, t =>
                        entry.SetOffset(new Vector2(Mathf.Sin(t * 60f) * pixels * (1f - t), 0f)), token);
                }

                await UiTween.Wait(_config.FeedEntrySeconds, token);
                await UiTween.Run(_config.FeedFadeSeconds, t =>
                {
                    entry.SetAlpha(1f - t);
                    entry.SetOffset(new Vector2(UiTween.EaseInCubic(t) * FlyInPixels, 0f));
                }, token);
                Remove(line);
            }
            catch (OperationCanceledException)
            {
                // Pushed out by a newer line or the scene is being disposed: nothing to clean up here.
            }
        }

        private static async UniTask CountDamage(FeedEntryView entry, int damage, CancellationToken token)
        {
            await UiTween.Run(CountSeconds, t =>
                entry.SetDamage("+" + Money.Of(Mathf.RoundToInt(damage * UiTween.EaseOutCubic(t)))), token);
            await UiTween.Run(0.3f, t => entry.SetDamageScale(1f + UiTween.Punch(t) * 0.45f), token);
        }

        private void Remove(Line line)
        {
            if (!_lines.Remove(line))
            {
                return;
            }

            line.Cancellation.Cancel();
            line.Cancellation.Dispose();
            _pool.Despawn(line.Entry);
        }

        private sealed class Line
        {
            public Line(FeedEntryView entry, CancellationTokenSource cancellation)
            {
                Entry = entry;
                Cancellation = cancellation;
            }

            public FeedEntryView Entry { get; }
            public CancellationTokenSource Cancellation { get; }
        }
    }
}
