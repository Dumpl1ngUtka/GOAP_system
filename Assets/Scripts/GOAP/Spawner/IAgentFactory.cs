using UnityEngine;

namespace GOAP.Spawner
{
    public interface IAgentFactory
    {
        GameObject CreateAgent(Vector3 position, Quaternion rotation);
        GameObject CreateAgent(Vector3 position);
        GameObject CreateAgent();
    }
}