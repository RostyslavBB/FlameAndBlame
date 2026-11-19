using System;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Blame
{
    /// <summary>
    /// Observer of all costly effects. For each one: register the cause in the graph, split damage and points
    /// between root actors, narrate a feed line and publish a <see cref="BlameEventSignal"/>.
    /// </summary>
    public sealed class BlameService : IBlameService, IInitializable, IDisposable
    {
        private const float PointsPerSeverity = 50f;
        private const string PlayerVictim = "Ти";

        private readonly SignalBus _signalBus;
        private readonly CauseGraph _graph;
        private readonly BlameLedger _ledger;
        private readonly ChainNarrator _narrator;
        private readonly CauseFormatter _formatter;
        private readonly IActorRegistry _actors;
        private readonly float[] _shares;

        public BlameService(SignalBus signalBus, CauseGraph graph, BlameLedger ledger, ChainNarrator narrator,
            CauseFormatter formatter, IActorRegistry actors)
        {
            _signalBus = signalBus;
            _graph = graph;
            _ledger = ledger;
            _narrator = narrator;
            _formatter = formatter;
            _actors = actors;
            _shares = new float[actors.Count];
        }

        public IBlameLedger Ledger => _ledger;
        public BlameRecord Last { get; private set; }

        public void Initialize()
        {
            _signalBus.Subscribe<ItemRuinedSignal>(OnItemRuined);
            _signalBus.Subscribe<FireSpreadSignal>(OnFireSpread);
            _signalBus.Subscribe<ElectrocutionSignal>(OnElectrocution);
            _signalBus.Subscribe<CivilianIncidentSignal>(OnCivilianIncident);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<ItemRuinedSignal>(OnItemRuined);
            _signalBus.Unsubscribe<FireSpreadSignal>(OnFireSpread);
            _signalBus.Unsubscribe<ElectrocutionSignal>(OnElectrocution);
            _signalBus.Unsubscribe<CivilianIncidentSignal>(OnCivilianIncident);
        }

        private void OnItemRuined(ItemRuinedSignal signal)
        {
            NarrativeEvent kind = signal.Reason switch
            {
                RuinReason.ShortCircuit => NarrativeEvent.ShortCircuit,
                RuinReason.Drowned => NarrativeEvent.Drowned,
                _ => NarrativeEvent.Burned
            };
            Record(kind, signal.Cause, signal.Damage, signal.Entity.DisplayName, null, null);
        }

        private void OnFireSpread(FireSpreadSignal signal)
        {
            NarrativeEvent kind = signal.Source == null ? NarrativeEvent.FireStart : NarrativeEvent.FireSpread;
            Record(kind, signal.Cause, 0, signal.Target.DisplayName, signal.Source?.DisplayName, null);
        }

        private void OnElectrocution(ElectrocutionSignal signal)
        {
            NarrativeEvent kind = signal.IsPlayer ? NarrativeEvent.ShockPlayer : NarrativeEvent.ShockObject;
            string victim = signal.IsPlayer ? PlayerVictim : signal.Surface.DisplayName;
            Record(kind, signal.Cause, signal.Damage, victim, null, victim);
        }

        private void OnCivilianIncident(CivilianIncidentSignal signal)
        {
            string neighbour = _actors.NameOf(ActorId.Civilian);
            switch (signal.Incident)
            {
                case CivilianIncident.KnockedItem:
                    Record(NarrativeEvent.CivilianKnock, signal.Cause, signal.Damage, signal.Item.DisplayName, null,
                        null);
                    break;
                case CivilianIncident.Soaked:
                    Record(NarrativeEvent.CivilianSoaked, signal.Cause, signal.Damage, neighbour, null, neighbour);
                    break;
                case CivilianIncident.Ignited:
                    Record(NarrativeEvent.CivilianIgnited, signal.Cause, signal.Damage, neighbour, null, neighbour);
                    break;
                default:
                    Record(NarrativeEvent.CivilianShocked, signal.Cause, signal.Damage, neighbour, null, neighbour);
                    break;
            }
        }

        private void Record(NarrativeEvent kind, Cause cause, int damage, string item, string source, string victim)
        {
            if (cause == null)
            {
                return;
            }

            _graph.Register(cause);
            _graph.CollectRootShares(cause, _shares);
            ActorId main = MainActor();
            int severity = SeverityOf(kind, damage);
            float points = damage > 0 ? damage : severity * PointsPerSeverity;

            string text = _narrator.Narrate(kind, main, item, source);
            BlameRecord record = new BlameRecord(kind, text, _formatter.Format(cause, victim), damage, severity,
                main, cause, victim ?? (item != null ? "«" + item + "»" : null));
            _ledger.Add(_shares, damage, points, record);
            Last = record;

            Debug.Log($"[Blame] {text} | ${damage} | {record.Chain} | {SharesText()}");
            _signalBus.Fire(new BlameEventSignal(record, _actors.ColorOf(main), _actors.NameOf(main)));
        }

        private ActorId MainActor()
        {
            int best = 0;
            for (int i = 1; i < _shares.Length; i++)
            {
                if (_shares[i] > _shares[best])
                {
                    best = i;
                }
            }

            return (ActorId)best;
        }

        private static int SeverityOf(NarrativeEvent kind, int damage)
        {
            int byDamage = damage >= 700 ? 3 : damage >= 200 ? 2 : 1;
            switch (kind)
            {
                case NarrativeEvent.ShockPlayer: return 3;
                case NarrativeEvent.CivilianSoaked:
                case NarrativeEvent.CivilianShocked:
                case NarrativeEvent.CivilianIgnited: return Math.Max(2, byDamage);
                case NarrativeEvent.FireStart:
                case NarrativeEvent.ShortCircuit:
                case NarrativeEvent.Drowned: return Math.Max(2, byDamage);
                case NarrativeEvent.FireSpread:
                case NarrativeEvent.ShockObject: return 1;
                default: return byDamage;
            }
        }

        private string SharesText()
        {
            string text = string.Empty;
            for (int i = 0; i < _shares.Length; i++)
            {
                if (_shares[i] > 0f)
                {
                    text += $"{_actors.NameOf((ActorId)i)} {_shares[i]:P0} ";
                }
            }

            return text;
        }
    }
}
