using UnityEngine;

namespace GOAP.Spawner
{
    public abstract class AgentFactory : IAgentFactory
    {
        protected abstract GameObject AgentPrefab { get; }
        protected abstract Transform Container { get; }

        public GameObject CreateAgent(Vector3 position, Quaternion rotation)
        {
            if (AgentPrefab == null)
            {
                Debug.LogError("Agent prefab is not set!");
                return null;
            }

            GameObject agent = Object.Instantiate(AgentPrefab, position, rotation);
        
            if (Container != null)
            {
                agent.transform.SetParent(Container);
            }

            return agent;
        }

        public GameObject CreateAgent(Vector3 position)
        {
            return CreateAgent(position, Quaternion.identity);
        }

        public GameObject CreateAgent()
        {
            return CreateAgent(Vector3.zero, Quaternion.identity);
        }
    }
}