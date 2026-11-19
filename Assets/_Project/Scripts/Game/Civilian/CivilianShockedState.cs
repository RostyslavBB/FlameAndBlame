using FB.Game.Power;
using UnityEngine;

namespace FB.Game.Civilian
{
    /// <summary>
    /// Lies on the floor of a live puddle, twitching and screaming while arcs keep striking him. Only once the
    /// floor under him is no longer electrified (power cut or puddle dried) does he jump up and panic again.
    /// </summary>
    public sealed class CivilianShockedState : CivilianState
    {
        private const float CheckInterval = 0.2f;

        private readonly IElectricity _electricity;
        private readonly ArcSpawner _arcs;
        private float _nextArc;
        private float _nextCheck;
        private float _recover;

        public CivilianShockedState(CivilianBody body, ICivilianStates states, IElectricity electricity,
            ArcSpawner arcs) : base(body, states)
        {
            _electricity = electricity;
            _arcs = arcs;
        }

        public override void Enter()
        {
            Body.IsKnocked = true;
            Body.IsPanicking = true;
            Body.IsShocked = true;
            Body.Stop();
            Body.PrepareToFall();
            _nextArc = 0f;
            _nextCheck = CheckInterval;
            _recover = Body.Config.ShockRecoverSeconds;
        }

        public override void Exit()
        {
            Body.IsKnocked = false;
            Body.IsPanicking = false;
            Body.IsShocked = false;
        }

        public override void Tick(float deltaTime)
        {
            _nextCheck -= deltaTime;
            if (_nextCheck <= 0f)
            {
                _nextCheck = CheckInterval;
                Body.IsShocked = _electricity.IsElectrifiedAt(Body.Position);
            }

            if (Body.IsShocked)
            {
                _recover = Body.Config.ShockRecoverSeconds;
                StrikeArcs(deltaTime);
                return;
            }

            _recover -= deltaTime;
            if (_recover <= 0f)
            {
                States.Enter<CivilianPanicState>();
            }
        }

        private void StrikeArcs(float deltaTime)
        {
            _nextArc -= deltaTime;
            if (_nextArc > 0f)
            {
                return;
            }

            _nextArc = Body.Config.ShockArcInterval;
            Transform root = Body.View.transform;
            Vector2 spread = Random.insideUnitCircle * 0.7f;
            Vector3 floor = root.position + new Vector3(spread.x, 0.02f, spread.y);
            // He lies on his back, so the body stretches behind the root.
            Vector3 body = root.position - root.forward * Random.Range(0.2f, 1.3f) + Vector3.up * 0.2f;
            _arcs.Spawn(floor, body);
        }
    }
}
