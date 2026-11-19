using FB.Game.Power;
using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>
    /// Scene objects of the generated level that gameplay needs (surfaces, breaker), filled by the level builder,
    /// so nothing is searched for at runtime.
    /// </summary>
    public sealed class LevelCatalog : MonoBehaviour
    {
        [SerializeField] private SurfaceEntity[] _surfaces;
        [SerializeField] private BreakerView _breaker;

        public SurfaceEntity[] Surfaces => _surfaces;
        public BreakerView Breaker => _breaker;
    }
}
