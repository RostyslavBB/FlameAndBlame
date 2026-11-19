namespace FB.Game.Blame
{
    /// <summary>Turns gameplay effects into blame: who pays how much, plus narrated feed lines.</summary>
    public interface IBlameService
    {
        IBlameLedger Ledger { get; }

        /// <summary>The most recent record, or null.</summary>
        BlameRecord Last { get; }
    }
}
