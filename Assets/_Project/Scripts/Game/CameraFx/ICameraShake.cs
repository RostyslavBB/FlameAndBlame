namespace FB.Game.CameraFx
{
    /// <summary>Trauma-based camera shake shared by all systems (hose, shocks, explosions).</summary>
    public interface ICameraShake
    {
        /// <summary>Adds trauma (0..1 total). Shake strength grows with trauma squared.</summary>
        void AddTrauma(float amount);
    }
}
