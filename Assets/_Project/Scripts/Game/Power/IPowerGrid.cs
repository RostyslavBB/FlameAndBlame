using FB.Game.Blame;

namespace FB.Game.Power
{
    /// <summary>Apartment power, switched by the breaker in the hall.</summary>
    public interface IPowerGrid
    {
        bool IsOn { get; }

        /// <summary>Root cause of anything the wiring does: Cause(Building, FaultyWiring).</summary>
        Cause WiringCause { get; }

        void Toggle();
    }
}
