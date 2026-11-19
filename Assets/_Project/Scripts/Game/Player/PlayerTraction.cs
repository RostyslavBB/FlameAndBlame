using UnityEngine;

namespace FB.Game.Player
{
    public sealed class PlayerTraction : IPlayerTraction
    {
        private float _groundControl = 1f;

        public float GroundControl
        {
            get => _groundControl;
            set => _groundControl = Mathf.Clamp01(value);
        }
    }
}
