namespace FB.Game.Smoke
{
    /// <summary>How smoky the apartment is.</summary>
    public interface ISmoke
    {
        /// <summary>0 (clear) .. 1 (full of smoke).</summary>
        float Amount { get; }
    }
}
