namespace FB.Game.Civilian
{
    /// <summary>Lies on the floor with spiral eyes, then jumps up and panics again.</summary>
    public sealed class CivilianKnockedState : CivilianState
    {
        private float _timer;

        public CivilianKnockedState(CivilianBody body, ICivilianStates states) : base(body, states)
        {
        }

        public override void Enter()
        {
            _timer = Body.Config.KnockedSeconds;
            Body.IsKnocked = true;
            Body.Stop();
            Body.View.SetDizzy(true);
        }

        public override void Exit()
        {
            Body.IsKnocked = false;
            Body.View.SetDizzy(false);
        }

        public override void Tick(float deltaTime)
        {
            _timer -= deltaTime;
            if (_timer <= 0f)
            {
                States.Enter<CivilianPanicState>();
            }
        }
    }
}
