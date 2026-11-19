namespace FB.Game.Fire
{
    /// <summary>Scheduled fire outbreaks of the round.</summary>
    public interface IFireOutbreaks
    {
        /// <summary>True while some room has not caught fire yet: the round is not over even if all fires are out.</summary>
        bool HasPending { get; }
    }
}
