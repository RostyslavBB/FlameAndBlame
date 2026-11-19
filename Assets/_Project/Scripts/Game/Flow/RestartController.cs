using FB.Core.Input;
using UnityEngine.SceneManagement;
using Zenject;

namespace FB.Game.Flow
{
    /// <summary>R reloads the scene. The SceneContext is destroyed with it, so scene-level subscriptions are disposed.</summary>
    public sealed class RestartController : ITickable
    {
        private readonly IInputService _input;
        private readonly ZenjectSceneLoader _sceneLoader;
        private bool _isLoading;

        public RestartController(IInputService input, ZenjectSceneLoader sceneLoader)
        {
            _input = input;
            _sceneLoader = sceneLoader;
        }

        public void Tick()
        {
            if (_isLoading || !_input.RestartPressed)
            {
                return;
            }

            _isLoading = true;
            _sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
