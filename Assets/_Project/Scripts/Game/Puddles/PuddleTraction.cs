using FB.Game.Player;
using Zenject;

namespace FB.Game.Puddles
{
    /// <summary>Standing in a puddle makes the floor slippery (lower ground control for the player).</summary>
    public sealed class PuddleTraction : ITickable
    {
        private readonly IPuddleField _puddles;
        private readonly PlayerBody _player;
        private readonly IPlayerTraction _traction;
        private readonly PuddleConfig _config;

        public PuddleTraction(IPuddleField puddles, PlayerBody player, IPlayerTraction traction, PuddleConfig config)
        {
            _puddles = puddles;
            _player = player;
            _traction = traction;
            _config = config;
        }

        public void Tick()
        {
            bool inPuddle = _puddles.FindAt(_player.Rigidbody.position) != null;
            _traction.GroundControl = inPuddle ? _config.SlipGroundControl : 1f;
        }
    }
}
