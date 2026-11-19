namespace FB.Game.Common
{
    /// <summary>Physics layers created by Build All (Tools/Flame&amp;Blame).</summary>
    public static class GameLayers
    {
        public const int Player = 8;
        public const string PlayerName = "Player";

        /// <summary>Everything except the player: water must not hit the one holding the hose.</summary>
        public const int AllButPlayerMask = ~(1 << Player);
    }
}
