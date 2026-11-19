using UnityEngine;

namespace FB.Game.Environment
{
    /// <summary>Passive wall clock: two hand pivots rotating around the clock face normal (local Z).</summary>
    public sealed class ClockView : MonoBehaviour
    {
        [SerializeField] private Transform _hourHand;
        [SerializeField] private Transform _minuteHand;

        public void SetTime(float hours, float minutes)
        {
            _hourHand.localRotation = Quaternion.Euler(0f, 0f, -hours % 12f * 30f);
            _minuteHand.localRotation = Quaternion.Euler(0f, 0f, -minutes * 6f);
        }
    }
}
