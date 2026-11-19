using UnityEngine;

namespace FB.Game.Blame
{
    /// <summary>Who the actors are, for texts and UI badges.</summary>
    public interface IActorRegistry
    {
        /// <summary>Number of actors (ActorId values), for per-actor arrays.</summary>
        int Count { get; }

        string NameOf(ActorId actor);
        Color ColorOf(ActorId actor);
    }
}
