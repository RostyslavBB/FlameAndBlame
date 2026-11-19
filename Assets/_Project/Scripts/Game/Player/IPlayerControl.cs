namespace FB.Game.Player
{
    /// <summary>Whether the player may move, look and spray (off during briefing and report).</summary>
    public interface IPlayerControl
    {
        bool Enabled { get; set; }
    }
}
