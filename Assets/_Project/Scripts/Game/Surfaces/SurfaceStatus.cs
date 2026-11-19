using System;

namespace FB.Game.Surfaces
{
    [Flags]
    public enum SurfaceStatus
    {
        None = 0,
        Wet = 1 << 0,
        Burning = 1 << 1,
        Ruined = 1 << 2,
        Electrified = 1 << 3
    }
}
