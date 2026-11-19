namespace FB.Core.Input
{
    /// <summary>Controls mouse capture. In WebGL the lock can only happen after a click.</summary>
    public interface ICursorService
    {
        bool IsLocked { get; }

        /// <summary>When true, a left click captures the cursor.</summary>
        bool LockAllowed { get; set; }

        void Unlock();
    }
}
