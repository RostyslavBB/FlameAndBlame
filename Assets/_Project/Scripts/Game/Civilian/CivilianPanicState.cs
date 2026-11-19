namespace FB.Game.Civilian
{
    /// <summary>
    /// Runs screaming from room to room, away from the nearest fire, knocking things over on the way.
    /// Calms down only after a while without fire nearby.
    /// </summary>
    public sealed class CivilianPanicState : CivilianState
    {
        private const float RepathSeconds = 2.5f;

        private float _calmDown;
        private float _repath;

        public CivilianPanicState(CivilianBody body, ICivilianStates states) : base(body, states)
        {
        }

        public override void Enter()
        {
            Body.IsPanicking = true;
            _calmDown = Body.Config.PanicSeconds;
            Flee();
        }

        public override void Exit()
        {
            Body.IsPanicking = false;
        }

        public override void Tick(float deltaTime)
        {
            _repath -= deltaTime;
            if (_repath <= 0f || Body.HasArrived)
            {
                Flee();
            }

            _calmDown -= deltaTime;
            if (_calmDown > 0f)
            {
                return;
            }

            if (Body.IsBurning || Body.TryFindFire(Body.Config.PanicRadius, out _))
            {
                _calmDown = Body.Config.PanicSeconds;
            }
            else
            {
                States.Enter<CivilianWanderState>();
            }
        }

        private void Flee()
        {
            _repath = RepathSeconds;
            bool fire = Body.TryFindFire(Body.Config.PanicRadius * 3f, out UnityEngine.Vector3 danger);
            Body.MoveTo(fire ? Body.WaypointAwayFrom(danger) : Body.RandomWaypoint(), Body.Config.PanicSpeed);
        }
    }
}
