using FB.Game.Level;
using FB.Game.Power;
using Zenject;

namespace FB.Game.Environment
{
    /// <summary>The breaker is real: switching the power off darkens lamps and screens all over the apartment.</summary>
    public sealed class RoomLightsController : IInitializable, ITickable
    {
        private readonly IPowerGrid _grid;
        private readonly RoomLightsView _view;
        private bool _powered;

        public RoomLightsController(IPowerGrid grid, LevelCatalog level)
        {
            _grid = grid;
            _view = level.Lights;
        }

        public void Initialize()
        {
            _powered = _grid.IsOn;
            _view.SetPowered(_powered);
        }

        public void Tick()
        {
            if (_grid.IsOn == _powered)
            {
                return;
            }

            _powered = _grid.IsOn;
            _view.SetPowered(_powered);
        }
    }
}
