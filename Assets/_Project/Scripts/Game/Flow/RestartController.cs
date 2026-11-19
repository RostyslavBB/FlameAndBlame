using FB.Core.Input;
using Zenject;

namespace FB.Game.Flow
{
    /// <summary>R restarts the round at any moment.</summary>
    public sealed class RestartController : ITickable
    {
        private readonly IInputService _input;
        private readonly ISceneRestarter _restarter;

        public RestartController(IInputService input, ISceneRestarter restarter)
        {
            _input = input;
            _restarter = restarter;
        }

        public void Tick()
        {
            if (_input.RestartPressed)
            {
                _restarter.Restart();
            }
        }
    }
}
