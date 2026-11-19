using UnityEngine;

namespace FB.Game.Civilian
{
    /// <summary>Strolls between rooms, idles a bit, panics as soon as a fire is close.</summary>
    public sealed class CivilianWanderState : CivilianState
    {
        private const float SenseInterval = 0.25f;

        private float _idle;
        private float _sense;

        public CivilianWanderState(CivilianBody body, ICivilianStates states) : base(body, states)
        {
        }

        public override void Enter()
        {
            _idle = 0f;
            Body.MoveTo(Body.RandomWaypoint(), Body.Config.WalkSpeed);
        }

        public override void Tick(float deltaTime)
        {
            _sense -= deltaTime;
            if (_sense <= 0f)
            {
                _sense = SenseInterval;
                if (Body.TryFindFire(Body.Config.PanicRadius, out _))
                {
                    States.Enter<CivilianPanicState>();
                    return;
                }
            }

            if (!Body.HasArrived)
            {
                return;
            }

            if (_idle <= 0f)
            {
                Vector2 range = Body.Config.IdleSeconds;
                _idle = Random.Range(range.x, range.y);
            }

            _idle -= deltaTime;
            if (_idle <= 0f)
            {
                Body.MoveTo(Body.RandomWaypoint(), Body.Config.WalkSpeed);
            }
        }
    }
}
