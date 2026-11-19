using FB.Game.Blame;
using Zenject;

namespace FB.Game.Power
{
    public sealed class PowerGrid : IPowerGrid, IInitializable
    {
        private readonly ICauseFactory _causes;

        public PowerGrid(ICauseFactory causes)
        {
            _causes = causes;
        }

        public bool IsOn { get; private set; } = true;
        public Cause WiringCause { get; private set; }

        public void Initialize()
        {
            WiringCause = _causes.Create(ActorId.Building, CauseAction.FaultyWiring);
        }

        public void Toggle()
        {
            IsOn = !IsOn;
        }
    }
}
