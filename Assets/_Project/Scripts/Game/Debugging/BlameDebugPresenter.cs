using System;
using System.Text;
using FB.Core.Input;
using FB.Game.Blame;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Debugging
{
    /// <summary>F1 / backquote: shows the last blame chain and the ledger (who pays how much).</summary>
    public sealed class BlameDebugPresenter : IInitializable, IDisposable, ITickable
    {
        private readonly BlameDebugView _view;
        private readonly IInputService _input;
        private readonly IBlameService _blame;
        private readonly IActorRegistry _actors;
        private readonly SignalBus _signalBus;
        private readonly StringBuilder _builder = new StringBuilder(512);
        private bool _visible;

        public BlameDebugPresenter(BlameDebugView view, IInputService input, IBlameService blame,
            IActorRegistry actors, SignalBus signalBus)
        {
            _view = view;
            _input = input;
            _blame = blame;
            _actors = actors;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<BlameEventSignal>(OnBlameEvent);
            _view.SetVisible(false);
            Refresh();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BlameEventSignal>(OnBlameEvent);
        }

        public void Tick()
        {
            if (_input.DebugPressed)
            {
                _visible = !_visible;
                _view.SetVisible(_visible);
            }
        }

        private void OnBlameEvent(BlameEventSignal signal) => Refresh();

        private void Refresh()
        {
            IBlameLedger ledger = _blame.Ledger;
            BlameRecord last = _blame.Last;
            _builder.Clear();
            _builder.Append("<b>BLAME DEBUG</b>  (F1 / `)\n");
            if (last != null)
            {
                _builder.Append("<b>Last:</b> ").Append(last.Text).Append("  <b>$").Append(last.Damage)
                    .Append("</b>  severity ").Append(last.Severity).Append('\n');
                _builder.Append("<b>Chain:</b> ").Append(last.Chain).Append('\n');
            }

            _builder.Append("<b>Fire damage:</b> $").Append(ledger.FireDamage)
                .Append("   <b>Firefighter damage:</b> $").Append(ledger.FirefighterDamage).Append('\n');
            for (int i = 0; i < _actors.Count; i++)
            {
                ActorId actor = (ActorId)i;
                float share = ledger.TotalPoints > 0f ? ledger.PointsOf(actor) / ledger.TotalPoints : 0f;
                _builder.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(_actors.ColorOf(actor))).Append('>')
                    .Append(_actors.NameOf(actor)).Append("</color>: $").Append(Mathf.RoundToInt(ledger.DamageOf(actor)))
                    .Append("  blame ").Append(Mathf.RoundToInt(share * 100f)).Append("%\n");
            }

            _builder.Append("Records: ").Append(ledger.Records.Count);
            _view.SetText(_builder.ToString());
        }
    }
}
