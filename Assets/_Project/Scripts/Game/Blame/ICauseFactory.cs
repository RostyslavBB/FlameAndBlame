namespace FB.Game.Blame
{
    /// <summary>The only way to create <see cref="Cause"/> instances (unique ids, consistent parents).</summary>
    public interface ICauseFactory
    {
        /// <summary>A root cause with no parents (e.g. the player starts spraying).</summary>
        Cause Create(ActorId instigator, CauseAction action);

        /// <summary>A cause triggered by one earlier cause (e.g. fire spreading from a burning object).</summary>
        Cause Create(ActorId instigator, CauseAction action, Cause parent);

        /// <summary>A cause with two contributing causes (e.g. wet puddle + live power source).</summary>
        Cause Create(ActorId instigator, CauseAction action, Cause first, Cause second);
    }
}
