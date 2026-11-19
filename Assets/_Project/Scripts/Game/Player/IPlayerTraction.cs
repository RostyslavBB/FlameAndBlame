namespace FB.Game.Player
{
    /// <summary>How well the player grips the floor. Slippery surfaces lower it, 1 = full control.</summary>
    public interface IPlayerTraction
    {
        /// <summary>0..1 multiplier of ground acceleration and braking.</summary>
        float GroundControl { get; set; }
    }
}
