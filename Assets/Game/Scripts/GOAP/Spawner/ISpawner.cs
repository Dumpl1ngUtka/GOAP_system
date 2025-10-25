using GOAP.Agent;
using UnityEngine;

namespace GOAP.Spawner
{
    public interface ISpawner<T> where T : Component
    {
        T Spawn(T prefab, Vector3 position, Quaternion rotation);
    }
}