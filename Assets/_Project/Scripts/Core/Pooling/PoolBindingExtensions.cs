using UnityEngine;
using Zenject;

namespace FB.Core.Pooling
{
    /// <summary>One-line pool registration for installers.</summary>
    public static class PoolBindingExtensions
    {
        public static void BindComponentPool<T>(this DiContainer container, T prefab, int initialSize)
            where T : Component
        {
            container.BindMemoryPool<T, ComponentPool<T>>()
                .WithInitialSize(initialSize)
                .FromComponentInNewPrefab(prefab)
                .UnderTransformGroup("Pool_" + typeof(T).Name);

            container.Bind<IPool<T>>().To<ComponentPool<T>>().FromResolve();
        }
    }
}
