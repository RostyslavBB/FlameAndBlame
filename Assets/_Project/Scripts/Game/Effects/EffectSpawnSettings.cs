namespace FB.Game.Effects
{
    /// <summary>Limits of a one-shot effect spawner, read from configs in the installer.</summary>
    public readonly struct EffectSpawnSettings
    {
        public EffectSpawnSettings(int maxActive, float lifetime, float cooldown)
        {
            MaxActive = maxActive;
            Lifetime = lifetime;
            Cooldown = cooldown;
        }

        public int MaxActive { get; }
        public float Lifetime { get; }

        /// <summary>Minimum seconds between two spawns.</summary>
        public float Cooldown { get; }
    }
}
