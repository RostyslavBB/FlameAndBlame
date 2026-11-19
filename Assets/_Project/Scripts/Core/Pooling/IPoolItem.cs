namespace FB.Core.Pooling
{
    /// <summary>Optional callbacks for pooled components that must reset their state.</summary>
    public interface IPoolItem
    {
        /// <summary>Called right after the item is taken from the pool.</summary>
        void OnSpawned();

        /// <summary>Called right before the item goes back to the pool.</summary>
        void OnDespawned();
    }
}
