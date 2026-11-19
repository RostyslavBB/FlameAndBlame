using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FB.Game.Blame;
using FB.Game.Flow;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.UI
{
    /// <summary>
    /// "Рахунок за виклик" as a little show: the card drops in, rows slide in one by one while their counters
    /// roll, the blame chain pops in card by card, the top events type themselves out, the verdict springs up, the stamp slams (shaking the card) and the
    /// restart button pops and keeps breathing. Everything runs on one token tied to the scene lifetime.
    /// </summary>
    public sealed class ReportPresenter : IInitializable, IDisposable
    {
        private const float CardDrop = 1100f;
        private const float RowRevealSeconds = 0.28f;
        private const float ShakeSeconds = 0.4f;
        private const float LinkPopSeconds = 0.24f;

        private readonly ReportView _view;
        private readonly IBlameService _blame;
        private readonly ReportVerdict _verdict;
        private readonly BlameChainBuilder _chain;
        private readonly List<ChainLink> _links = new List<ChainLink>(8);
        private readonly ISceneRestarter _restarter;
        private readonly SignalBus _signalBus;
        private readonly UiConfig _config;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private string _fire;
        private string _firefighter;
        private string _total;

        public ReportPresenter(ReportView view, IBlameService blame, ReportVerdict verdict, BlameChainBuilder chain,
            ISceneRestarter restarter, SignalBus signalBus, UiConfig config)
        {
            _view = view;
            _blame = blame;
            _verdict = verdict;
            _chain = chain;
            _restarter = restarter;
            _signalBus = signalBus;
            _config = config;
        }

        public void Initialize()
        {
            _view.SetAlpha(0f);
            _view.RestartClicked += _restarter.Restart;
            _signalBus.Subscribe<GamePhaseChangedSignal>(OnPhaseChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GamePhaseChangedSignal>(OnPhaseChanged);
            _view.RestartClicked -= _restarter.Restart;
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private void OnPhaseChanged(GamePhaseChangedSignal signal)
        {
            if (signal.Phase == GamePhase.Report)
            {
                Play(_lifetime.Token).Forget();
            }
        }

        private async UniTaskVoid Play(CancellationToken token)
        {
            IBlameLedger ledger = _blame.Ledger;
            Reset();
            try
            {
                await UiTween.Run(_config.ReportFadeSeconds * 2f, t =>
                {
                    _view.SetAlpha(Mathf.Clamp01(t * 2f));
                    float eased = UiTween.EaseOutBack(t);
                    _view.SetCard(new Vector2(0f, Mathf.LerpUnclamped(CardDrop, 0f, eased)), Mathf.LerpUnclamped(7f, 0f, eased));
                }, token);

                float countSeconds = _config.CounterSeconds * 0.5f;
                await RevealRow(0, token);
                await UiTween.Run(countSeconds, t => ShowDamage(Count(ledger.FireDamage, t), _firefighter, _total), token);
                await RevealRow(1, token);
                await UiTween.Run(countSeconds, t => ShowDamage(_fire, Count(ledger.FirefighterDamage, t), _total), token);
                await RevealRow(2, token);
                await UiTween.Run(countSeconds, t => ShowDamage(_fire, _firefighter, Count(ledger.TotalDamage, t)), token);
                await UiTween.Run(0.35f, t => _view.SetTotalScale(1f + UiTween.Punch(t) * 0.35f), token);

                _view.SetChains(_verdict.TopChains(ledger, 3));
                int length = _view.ChainsLength;
                await RevealRow(3, token);
                await RevealChain(ledger, token);
                await UiTween.Run(length / (_config.BriefingCharsPerSecond * 2.5f),
                    t => _view.SetChainsVisible(Mathf.CeilToInt(length * t)), token);
                await UiTween.Wait(_config.StampDelaySeconds, token);

                VerdictResult result = _verdict.Decide(ledger);
                _view.SetVerdict(result.Text);
                await UiTween.Run(0.5f, t => _view.SetVerdictScale(UiTween.EaseOutElastic(t)), token);
                await UiTween.Run(_config.StampSeconds, t =>
                    _view.SetStamp(Mathf.Lerp(3f, 1f, UiTween.EaseInCubic(t)), t, result.Color, result.StampLabel), token);
                await Shake(token);
                await PopRestart(token);
            }
            catch (OperationCanceledException)
            {
                // Scene restarted while the report was animating.
            }
        }

        private void Reset()
        {
            _fire = _firefighter = _total = Money.Of(0);
            ShowDamage(_fire, _firefighter, _total);
            for (int i = 0; i < _view.RowCount; i++)
            {
                _view.SetRow(i, 0f);
            }

            _view.SetChains(string.Empty);
            _view.HideLinks();
            _view.SetVerdict(string.Empty);
            _view.SetVerdictScale(0f);
            _view.SetStamp(0f, 0f, Color.clear, string.Empty);
            _view.SetRestart(0f, false);
        }

        private UniTask RevealRow(int index, CancellationToken token) =>
            index < _view.RowCount
                ? UiTween.Run(RowRevealSeconds, t => _view.SetRow(index, UiTween.EaseOutBack(t)), token)
                : UniTask.CompletedTask;

        /// <summary>The most expensive event as cards with arrows, popping in one after another.</summary>
        private async UniTask RevealChain(IBlameLedger ledger, CancellationToken token)
        {
            BlameRecord top = _verdict.MostExpensive(ledger);
            if (top == null)
            {
                return;
            }

            _chain.Build(top, _view.LinkCount, _links);
            for (int i = 0; i < _links.Count; i++)
            {
                _view.SetLink(i, _links[i], i == _links.Count - 1);
            }

            for (int i = 0; i < _links.Count; i++)
            {
                int index = i;
                await UiTween.Run(LinkPopSeconds, t =>
                    _view.SetLinkReveal(index, UiTween.EaseOutBack(t), Mathf.InverseLerp(0.5f, 1f, t)), token);
            }
        }

        private UniTask Shake(CancellationToken token)
        {
            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            return UiTween.Run(ShakeSeconds, t =>
            {
                float punch = UiTween.Punch(t, 5f);
                _view.SetCard(direction * (punch * 22f), punch * 1.5f);
            }, token);
        }

        private async UniTask PopRestart(CancellationToken token)
        {
            _view.SetRestart(0f, true);
            await UiTween.Run(0.35f, t => _view.SetRestart(UiTween.EaseOutBack(t), true), token);
            await UiTween.Loop(time => _view.SetRestart(1f + Mathf.Sin(time * 4f) * 0.04f, true), token);
        }

        private void ShowDamage(string fire, string firefighter, string total)
        {
            _fire = fire;
            _firefighter = firefighter;
            _total = total;
            _view.SetDamage(fire, firefighter, total);
        }

        private static string Count(int target, float t) =>
            Money.Of(Mathf.RoundToInt(target * UiTween.EaseOutCubic(t)));
    }
}
