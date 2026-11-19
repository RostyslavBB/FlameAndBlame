using System;
using FB.Core.StateMachine;
using FB.Game.Blame;
using FB.Game.Power;
using FB.Game.Puddles;
using FB.Game.Signals;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FB.Game.Civilian
{
    /// <summary>
    /// The panicking neighbour: owns his state machine (Wander / Panic / Knocked), the procedural animation
    /// and the chaos he causes. Hose water and live puddles knock him down, with the blame going to whoever
    /// caused the water or the power.
    /// </summary>
    public sealed class CivilianBrain : ICivilianStates, IInitializable, ITickable, IDisposable
    {
        private const float ShockCheckInterval = 0.2f;

        private readonly CivilianBody _body;
        private readonly CivilianChaos _chaos;
        private readonly CivilianAnimator _animator = new CivilianAnimator();
        private readonly StateMachine<CivilianState> _machine;
        private readonly IElectricity _electricity;
        private readonly IPuddles _puddles;
        private readonly ArcSpawner _arcs;
        private readonly ICauseFactory _causes;
        private readonly SignalBus _signalBus;
        private NavMeshDataInstance _navMesh;
        private float _soak;
        private float _nextShockCheck;

        public CivilianBrain(CivilianBody body, CivilianChaos chaos, IElectricity electricity, IPuddles puddles,
            ArcSpawner arcs, ICauseFactory causes, SignalBus signalBus)
        {
            _body = body;
            _chaos = chaos;
            _electricity = electricity;
            _puddles = puddles;
            _arcs = arcs;
            _causes = causes;
            _signalBus = signalBus;
            _machine = new StateMachine<CivilianState>(new CivilianState[]
            {
                new CivilianWanderState(body, this),
                new CivilianPanicState(body, this),
                new CivilianKnockedState(body, this)
            });
        }

        public void Enter<TState>() where TState : CivilianState => _machine.Enter<TState>();

        public void Initialize()
        {
            if (_body.View.NavMesh != null)
            {
                _navMesh = NavMesh.AddNavMeshData(_body.View.NavMesh);
            }

            _body.Activate();
            _signalBus.Subscribe<WaterHitSignal>(OnWaterHit);
            _machine.Enter<CivilianWanderState>();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<WaterHitSignal>(OnWaterHit);
            _navMesh.Remove();
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;
            _machine.Tick(deltaTime);
            _chaos.Tick();
            CheckShock();
            _soak = Mathf.Max(0f, _soak - deltaTime * _body.Config.WaterToKnockDown);
            _animator.Update(_body.View, _body.Speed, _body.IsPanicking, _body.IsKnocked, deltaTime);
        }

        private void OnWaterHit(WaterHitSignal signal)
        {
            if (signal.Target != _body.View.Collider || _body.IsKnocked)
            {
                return;
            }

            _soak += signal.Amount;
            if (_soak < _body.Config.WaterToKnockDown)
            {
                return;
            }

            _soak = 0f;
            Cause cause = _causes.Create(ActorId.Player, CauseAction.Soak, signal.Cause);
            KnockDown(CivilianIncident.Soaked, _body.Config.SoakedDamage, cause);
        }

        private void CheckShock()
        {
            float now = Time.time;
            if (now < _nextShockCheck || _body.IsKnocked)
            {
                return;
            }

            _nextShockCheck = now + ShockCheckInterval;
            Vector3 feet = _body.Position;
            if (!_electricity.IsElectrifiedAt(feet))
            {
                return;
            }

            _arcs.Spawn(feet + Vector3.up * 0.02f, feet + Vector3.up * 1.2f);
            Cause cause = _causes.Create(ActorId.Building, CauseAction.Electrocution, _puddles.CauseAt(feet),
                _electricity.PowerCause);
            KnockDown(CivilianIncident.Shocked, _body.Config.ShockedDamage, cause);
        }

        private void KnockDown(CivilianIncident incident, int damage, Cause cause)
        {
            _machine.Enter<CivilianKnockedState>();
            _signalBus.Fire(new CivilianIncidentSignal(incident, null, damage, cause));
        }
    }
}
