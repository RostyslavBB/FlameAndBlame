using FB.Game.Player;
using UnityEngine;
using Zenject;

namespace FB.Game.Puddles
{
    /// <summary>Standing in water makes the floor slippery, more so the deeper it is.</summary>
    public sealed class PuddleTraction : ITickable
    {
        private readonly IPuddles _puddles;
        private readonly PlayerBody _player;
        private readonly IPlayerTraction _traction;
        private readonly PuddleConfig _config;

        public PuddleTraction(IPuddles puddles, PlayerBody player, IPlayerTraction traction, PuddleConfig config)
        {
            _puddles = puddles;
            _player = player;
            _traction = traction;
            _config = config;
        }

        public void Tick()
        {
            float wetness = Mathf.Clamp01(_puddles.DepthAt(_player.Rigidbody.position) / _config.FullSlipDepth);
            _traction.GroundControl = Mathf.Lerp(1f, _config.SlipGroundControl, wetness);
        }
    }
}
