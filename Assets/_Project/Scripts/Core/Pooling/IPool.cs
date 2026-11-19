namespace FB.Core.Pooling
{
    /// <summary>Spawns and recycles instances of <typeparamref name="T"/> without allocating during gameplay.</summary>
    public interface IPool<T>
    {
        /// <summary>Number of instances currently in use.</summary>
        int NumActive { get; }

        /// <summary>Takes an instance from the pool (creates one only if the pool is empty).</summary>
        T Spawn();

        /// <summary>Returns an instance to the pool. Null is ignored.</summary>
        void Despawn(T item);
    }
}
