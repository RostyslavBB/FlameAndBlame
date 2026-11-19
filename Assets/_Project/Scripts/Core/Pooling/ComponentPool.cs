using UnityEngine;
using Zenject;

namespace FB.Core.Pooling
{
    /// <summary>
    /// Generic component pool on top of Zenject's MonoMemoryPool: activates/deactivates instances
    /// and forwards lifecycle callbacks to <see cref="IPoolItem"/>.
    /// Not sealed on purpose: Zenject needs a concrete pool type per item type.
    /// </summary>
    public class ComponentPool<T> : MonoMemoryPool<T>, IPool<T> where T : Component
    {
        T IPool<T>.Spawn() => Spawn();

        void IPool<T>.Despawn(T item)
        {
            if (item == null)
            {
                return;
            }

            Despawn(item);
        }

        protected override void OnSpawned(T item)
        {
            base.OnSpawned(item);
            if (item is IPoolItem poolItem)
            {
                poolItem.OnSpawned();
            }
        }

        protected override void OnDespawned(T item)
        {
            if (item is IPoolItem poolItem)
            {
                poolItem.OnDespawned();
            }

            base.OnDespawned(item);
        }
    }
}
