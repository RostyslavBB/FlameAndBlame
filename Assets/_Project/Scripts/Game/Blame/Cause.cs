using System;
using System.Collections.Generic;

namespace FB.Game.Blame
{
    /// <summary>
    /// Immutable node of the blame graph: "who did what, because of which earlier causes".
    /// Every gameplay effect carries one, so any damage can be traced back to its root actors.
    /// </summary>
    public sealed class Cause
    {
        private static readonly IReadOnlyList<Cause> NoParents = Array.Empty<Cause>();

        public Cause(int id, CauseAction action, ActorId instigator, IReadOnlyList<Cause> parents)
        {
            Id = id;
            Action = action;
            Instigator = instigator;
            Parents = parents ?? NoParents;
        }

        public int Id { get; }
        public CauseAction Action { get; }
        public ActorId Instigator { get; }
        public IReadOnlyList<Cause> Parents { get; }

        public override string ToString() => $"#{Id} {Instigator}:{Action}";
    }
}
