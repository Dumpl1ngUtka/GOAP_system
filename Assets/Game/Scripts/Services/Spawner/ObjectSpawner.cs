using UnityEngine;

namespace Services.Spawner
{
    public class ObjectSpawner<T> : ISpawner<T> where T : Component
    {
        public virtual T Spawn(T prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null.");
                return null;
            }

            return Object.Instantiate(prefab, position, rotation);
        }
    }
}