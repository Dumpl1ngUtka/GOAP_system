using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GOAP.Spawner
{
    public class MonoAgentSpawner : MonoBehaviour, IAgentSpawner
    {
        [SerializeField] private GameObject _agentPrefab;
        [SerializeField] private Transform _container;

        private IAgentSpawner _agentSpawner;

        public IReadOnlyList<GameObject> SpawnedAgents => _agentSpawner.SpawnedAgents;

        private void Awake()
        {
            IAgentFactory factory = new GoapAgentFactory(_agentPrefab, _container);
            _agentSpawner = new AgentSpawner(factory);
        }

        public GameObject SpawnAgent(Vector3 position, Quaternion rotation)
        {
            return _agentSpawner.SpawnAgent(position, rotation);
        }

        public GameObject SpawnAgent(Vector3 position)
        {
            return _agentSpawner.SpawnAgent(position);
        }

        public GameObject SpawnAgent()
        {
            return _agentSpawner.SpawnAgent();
        }

        public void DespawnAgent(GameObject agent)
        {
            _agentSpawner.DespawnAgent(agent);
        }

        public void DespawnAllAgents()
        {
            _agentSpawner.DespawnAllAgents();
        }

        public void SpawnAtRandomPoints(int count, Vector3 areaCenter, Vector3 areaSize)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 randomPosition = new Vector3(
                    Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                    areaCenter.y,
                    Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

                SpawnAgent(randomPosition);
            }
        }

        public void TMP_Spawn()
        {
            SpawnAgent();
        }
    }
}