using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Services.Spawner
{
    public class ObjectSpawner<T> : ISpawner<T> where T : Component
    {
        private readonly IInstantiator _instantiator;

        public ObjectSpawner(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public virtual T Spawn(T prefab, Vector3 position, Quaternion rotation, IEnumerable<object> extraArgs = null)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null.");
                return null;
            }

            if (extraArgs != null)
                return _instantiator.InstantiatePrefabForComponent<T>(
                    prefab,
                    position,
                    rotation,
                    null,
                    extraArgs);
            
            return _instantiator.InstantiatePrefabForComponent<T>(
                prefab,
                position,
                rotation,
                null);
        }
    }
}