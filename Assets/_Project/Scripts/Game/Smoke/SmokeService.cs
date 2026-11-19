using FB.Game.Fire;
using FB.Game.Level;
using UnityEngine;
using Zenject;

namespace FB.Game.Smoke
{
    /// <summary>
    /// Every burning object adds smoke; it gathers under the ceiling and the layer slowly comes down, the air gets
    /// hazy (fog) and the picture duller (post-processing volume). When fires are out the smoke clears.
    /// </summary>
    public sealed class SmokeService : ISmoke, IInitializable, ITickable, System.IDisposable
    {
        private readonly IFireStatus _fire;
        private readonly SmokeConfig _config;
        private readonly SmokeView _view;
        private float _visual;

        public SmokeService(IFireStatus fire, SmokeConfig config, LevelCatalog level)
        {
            _fire = fire;
            _config = config;
            _view = level.Smoke;
        }

        public float Amount { get; private set; }

        public void Initialize()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = _config.FogColor;
            Apply();
        }

        public void Dispose()
        {
            RenderSettings.fogDensity = 0f;
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;
            int burning = _fire.BurningCount;
            float clear = burning > 0 ? _config.ClearPerSecond : _config.ClearWhenOutPerSecond;
            Amount = Mathf.Clamp01(Amount + (burning * _config.PerFirePerSecond - clear) * deltaTime);
            _visual = Mathf.Lerp(_visual, Amount, 1f - Mathf.Exp(-_config.VisualLerpSpeed * deltaTime));
            Apply();
        }

        private void Apply()
        {
            float layerY = Mathf.Lerp(_config.LayerTopY, _config.LayerBottomY, _visual);
            _view.Show(_visual, layerY);
            RenderSettings.fogDensity = _config.MaxFogDensity * _visual * _visual;
        }
    }
}
