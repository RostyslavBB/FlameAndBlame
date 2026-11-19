using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace FB.Game.Flow
{
    public sealed class SceneRestarter : ISceneRestarter
    {
        private readonly ZenjectSceneLoader _sceneLoader;
        private bool _isLoading;

        public SceneRestarter(ZenjectSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Restart()
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;
            Time.timeScale = 1f;
            _sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
