using FB.Core.Input;
using FB.Game.Level;
using FB.Game.Player;
using Zenject;

namespace FB.Game.Power
{
    /// <summary>E near the breaker toggles the apartment power.</summary>
    public sealed class BreakerController : IBreakerInteraction, IInitializable, ITickable
    {
        private readonly IInputService _input;
        private readonly IPowerGrid _grid;
        private readonly BreakerView _view;
        private readonly PlayerBody _player;
        private readonly PowerConfig _config;

        public BreakerController(IInputService input, IPowerGrid grid, LevelCatalog level, PlayerBody player,
            PowerConfig config)
        {
            _input = input;
            _grid = grid;
            _view = level.Breaker;
            _player = player;
            _config = config;
        }

        /// <summary>True when the player is close enough to use the breaker (for the HUD hint).</summary>
        public bool InReach =>
            (_player.CameraPivot.position - _view.Position).sqrMagnitude <= _config.BreakerReach * _config.BreakerReach;

        public void Initialize()
        {
            _view.SetOn(_grid.IsOn);
        }

        public void Tick()
        {
            if (_input.InteractPressed && InReach)
            {
                _grid.Toggle();
                _view.SetOn(_grid.IsOn);
            }
        }
    }
}
