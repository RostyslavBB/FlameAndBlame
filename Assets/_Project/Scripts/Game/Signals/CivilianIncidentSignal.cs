using FB.Game.Blame;
using FB.Game.Surfaces;

namespace FB.Game.Signals
{
    public enum CivilianIncident
    {
        /// <summary>The panicking neighbour knocked an item over.</summary>
        KnockedItem,

        /// <summary>The hose knocked the neighbour off his feet.</summary>
        Soaked,

        /// <summary>The neighbour stepped into a live puddle.</summary>
        Shocked
    }

    /// <summary>Something happened with or because of the neighbour (NPC).</summary>
    public readonly struct CivilianIncidentSignal
    {
        public CivilianIncidentSignal(CivilianIncident incident, ISurface item, int damage, Cause cause)
        {
            Incident = incident;
            Item = item;
            Damage = damage;
            Cause = cause;
        }

        public CivilianIncident Incident { get; }

        /// <summary>The knocked item; null for incidents that happen to the neighbour himself.</summary>
        public ISurface Item { get; }

        /// <summary>$ (medical bill for the neighbour).</summary>
        public int Damage { get; }

        public Cause Cause { get; }
    }
}
