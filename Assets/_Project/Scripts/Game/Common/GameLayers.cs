namespace FB.Game.Common
{
    /// <summary>Physics layers created by Build All (Tools/Flame&amp;Blame).</summary>
    public static class GameLayers
    {
        public const int Player = 8;
        public const string PlayerName = "Player";
        public const int Civilian = 9;
        public const string CivilianName = "Civilian";

        /// <summary>Everything except the player: water must not hit the one holding the hose.</summary>
        public const int AllButPlayerMask = ~(1 << Player);

        /// <summary>Level geometry and props only: the hose rope, puddles and chaos ignore the characters.</summary>
        public const int EnvironmentMask = ~((1 << Player) | (1 << Civilian));
    }
}
