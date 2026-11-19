using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Core.Pooling;
using FB.Game.Flow;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>
    /// Blame feed: every BlameEventSignal becomes a pooled line on top; at most N lines; big events shake.
    /// Each line owns a CancellationTokenSource, cancelled when it is pushed out or the scene is disposed.
    /// </summary>
    public sealed class FeedPresenter : IInitializable, IDisposable
    {
        private readonly FeedView _view;
        private readonly IPool<FeedEntryView> _pool;
        private readonly SignalBus _signalBus;
        private readonly UiConfig _config;
        private readonly List<Line> _lines = new List<Line>();

        public FeedPresenter(FeedView view, IPool<FeedEntryView> pool, SignalBus signalBus, UiConfig config)
        {
            _view = view;
            _pool = pool;
            _signalBus = signalBus;
            _config = config;
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
            if (_lines.Count >= _config.FeedMaxEntries)
            {
                Remove(_lines[0]);
            }

            FeedEntryView entry = _pool.Spawn();
            entry.transform.SetParent(_view.Container, false);
            entry.transform.SetAsFirstSibling();
            entry.Set(signal.ActorName, signal.ActorColor, signal.Text,
                signal.Damage > 0 ? "+" + Money.Of(signal.Damage) : null);

            Line line = new Line(entry, new CancellationTokenSource());
            _lines.Add(line);
            Animate(line, signal.Severity).Forget();
        }

        private async UniTaskVoid Animate(Line line, int severity)
        {
            FeedEntryView entry = line.Entry;
            CancellationToken token = line.Cancellation.Token;
            try
            {
                await UiTween.Run(_config.FeedPopSeconds, t =>
                {
                    entry.SetAlpha(t);
                    entry.SetScale(Mathf.LerpUnclamped(0.8f, 1f, UiTween.EaseOutBack(t)));
                }, token);

                if (severity >= _config.FeedShakeMinSeverity)
                {
                    float pixels = _config.FeedShakePixels * (severity - _config.FeedShakeMinSeverity + 1);
                    await UiTween.Run(_config.FeedShakeSeconds, t =>
                        entry.SetOffset(new Vector2(Mathf.Sin(t * 60f) * pixels * (1f - t), 0f)), token);
                }

                await UiTween.Wait(_config.FeedEntrySeconds, token);
                await UiTween.Run(_config.FeedFadeSeconds, t => entry.SetAlpha(1f - t), token);
                Remove(line);
            }
            catch (OperationCanceledException)
            {
                // Pushed out by a newer line or the scene is being disposed: nothing to clean up here.
            }
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
