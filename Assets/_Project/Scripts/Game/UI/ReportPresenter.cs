using System;
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
    /// "Рахунок за виклик": animated damage counters, the most expensive chains and the verdict stamp.
    /// The whole sequence runs on one cancellation token tied to the scene lifetime.
    /// </summary>
    public sealed class ReportPresenter : IInitializable, IDisposable
    {
        private readonly ReportView _view;
        private readonly IBlameService _blame;
        private readonly ReportVerdict _verdict;
        private readonly ISceneRestarter _restarter;
        private readonly SignalBus _signalBus;
        private readonly UiConfig _config;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public ReportPresenter(ReportView view, IBlameService blame, ReportVerdict verdict, ISceneRestarter restarter,
            SignalBus signalBus, UiConfig config)
        {
            _view = view;
            _blame = blame;
            _verdict = verdict;
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
            _view.SetDamage(Money.Of(0), Money.Of(0), Money.Of(0));
            _view.SetChains(string.Empty);
            _view.SetVerdict(string.Empty);
            _view.SetStamp(0f, 0f, Color.clear, string.Empty);

            try
            {
                await UiTween.Run(_config.ReportFadeSeconds, _view.SetAlpha, token);
                await UiTween.Run(_config.CounterSeconds, t =>
                {
                    float eased = 1f - (1f - t) * (1f - t);
                    _view.SetDamage(Count(ledger.FireDamage, eased), Count(ledger.FirefighterDamage, eased),
                        Count(ledger.TotalDamage, eased));
                }, token);

                _view.SetChains(_verdict.TopChains(ledger, 3));
                await UiTween.Wait(_config.StampDelaySeconds, token);

                VerdictResult result = _verdict.Decide(ledger);
                _view.SetVerdict(result.Text);
                await UiTween.Run(_config.StampSeconds, t =>
                    _view.SetStamp(Mathf.Lerp(3f, 1f, t * t), t, result.Color, result.StampLabel), token);
            }
            catch (OperationCanceledException)
            {
                // Scene restarted while the report was animating.
            }
        }

        private static string Count(int target, float t) => Money.Of(Mathf.RoundToInt(target * t));
    }
}
