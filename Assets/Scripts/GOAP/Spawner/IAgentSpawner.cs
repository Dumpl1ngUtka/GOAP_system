using System.Collections.Generic;
using UnityEngine;

namespace GOAP.Spawner
{
    public interface IAgentSpawner
    {
        IReadOnlyList<GameObject> SpawnedAgents { get; }
        GameObject SpawnAgent(Vector3 position, Quaternion rotation);
        GameObject SpawnAgent(Vector3 position);
        GameObject SpawnAgent();
        void DespawnAgent(GameObject agent);
        void DespawnAllAgents();
    }
}