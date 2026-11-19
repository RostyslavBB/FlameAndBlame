using FB.Game.Puddles;
using FB.Game.Smoke;
using FB.Game.Environment;
using FB.Game.Power;
using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>
    /// Scene objects of the generated level that gameplay needs (surfaces, breaker, puddle water surface, smoke, lights, clock), filled by the level builder,
    /// so nothing is searched for at runtime.
    /// </summary>
    public sealed class LevelCatalog : MonoBehaviour
    {
        [SerializeField] private SurfaceEntity[] _surfaces;
        [SerializeField] private BreakerView _breaker;
        [SerializeField] private PuddleSurfaceView _puddles;
        [SerializeField] private SmokeView _smoke;
        [SerializeField] private RoomLightsView _lights;
        [SerializeField] private ClockView _clock;

        public SurfaceEntity[] Surfaces => _surfaces;
        public BreakerView Breaker => _breaker;
        public PuddleSurfaceView Puddles => _puddles;
        public SmokeView Smoke => _smoke;
        public RoomLightsView Lights => _lights;
        public ClockView Clock => _clock;
    }
}
