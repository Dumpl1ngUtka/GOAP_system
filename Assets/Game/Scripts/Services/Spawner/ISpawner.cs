using UnityEngine;

namespace Services.Spawner
{
    public interface ISpawner<T> where T : Component
    {
        T Spawn(T prefab, Vector3 position, Quaternion rotation);
    }
}