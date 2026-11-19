using FB.Core.Pooling;
using UnityEngine;

namespace FB.Game.Power
{
    /// <summary>Pooled lightning bolt: a LineRenderer whose middle points jitter every frame.</summary>
    public sealed class ElectricArc : MonoBehaviour, IPoolItem
    {
        [SerializeField] private LineRenderer _line;

        public int PointCount => _line.positionCount;

        public void SetPoint(int index, Vector3 position) => _line.SetPosition(index, position);

        public void OnSpawned()
        {
            _line.enabled = true;
        }

        public void OnDespawned()
        {
            _line.enabled = false;
        }
    }
}
