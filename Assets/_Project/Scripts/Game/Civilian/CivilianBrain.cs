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
    /// The panicking neighbour: owns his state machine (Wander / Panic / Knocked / Shocked), the procedural
    /// animation and the chaos he causes. Hose water and live puddles knock him down (a live puddle keeps him
    /// twitching on the floor while the current flows), with the blame going to whoever caused the water or the power.
    /// </summary>
    public sealed class CivilianBrain : ICivilianStates, IInitializable, ITickable, IDisposable
    {
        private const float ShockCheckInterval = 0.2f;

        private readonly CivilianBody _body;
        private readonly CivilianChaos _chaos;
        private readonly CivilianBurning _burning;
        private readonly CivilianAnimator _animator = new CivilianAnimator();
        private readonly StateMachine<CivilianState> _machine;
        private readonly IElectricity _electricity;
        private readonly IPuddles _puddles;
        private readonly ICauseFactory _causes;
        private readonly SignalBus _signalBus;
        private NavMeshDataInstance _navMesh;
        private float _soak;
        private float _nextShockCheck;

        public CivilianBrain(CivilianBody body, CivilianChaos chaos, CivilianBurning burning, IElectricity electricity, IPuddles puddles,
            ArcSpawner arcs, ICauseFactory causes, SignalBus signalBus)
        {
            _body = body;
            _chaos = chaos;
            _burning = burning;
            _electricity = electricity;
            _puddles = puddles;
            _causes = causes;
            _signalBus = signalBus;
            _machine = new StateMachine<CivilianState>(new CivilianState[]
            {
                new CivilianWanderState(body, this),
                new CivilianPanicState(body, this),
                new CivilianKnockedState(body, this),
                new CivilianShockedState(body, this, electricity, arcs)
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
            _burning.Dispose();
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;
            _machine.Tick(deltaTime);
            _body.TickShove(deltaTime);
            if (_burning.Tick(deltaTime) && !_body.IsKnocked)
            {
                _machine.Enter<CivilianPanicState>();
            }

            _chaos.Tick();
            CheckShock();
            _soak = Mathf.Max(0f, _soak - deltaTime * _body.Config.WaterToKnockDown);
            _animator.Update(_body.View, _body.Speed, _body.IsPanicking, _body.IsKnocked, _body.IsShocked, deltaTime);
        }

        private void OnWaterHit(WaterHitSignal signal)
        {
            if (signal.Target != _body.View.Collider)
            {
                return;
            }

            // The jet shoves him away (also while he lies on the floor: he gets washed along it).
            _body.Shove(-signal.Normal * (signal.Amount * _body.Config.ShovePerWater));
            _burning.Soak();
            if (_body.IsKnocked)
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
            KnockDown<CivilianKnockedState>(CivilianIncident.Soaked, _body.Config.SoakedDamage, cause);
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

            Cause cause = _causes.Create(ActorId.Building, CauseAction.Electrocution, _puddles.CauseAt(feet),
                _electricity.PowerCause);
            KnockDown<CivilianShockedState>(CivilianIncident.Shocked, _body.Config.ShockedDamage, cause);
        }

        private void KnockDown<TState>(CivilianIncident incident, int damage, Cause cause) where TState : CivilianState
        {
            _machine.Enter<TState>();
            _signalBus.Fire(new CivilianIncidentSignal(incident, null, damage, cause));
        }
    }
}
