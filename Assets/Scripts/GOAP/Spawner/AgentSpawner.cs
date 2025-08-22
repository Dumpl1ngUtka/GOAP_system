using System.Collections.Generic;
using GOAP.Agent;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using UnityEngine;

namespace GOAP.Spawner
{
    public class AgentSpawner : IAgentSpawner
    {
        private readonly IAgentFactory _agentFactory;
        private readonly List<GameObject> _spawnedAgents = new List<GameObject>();

        public IReadOnlyList<GameObject> SpawnedAgents => _spawnedAgents;

        public AgentSpawner(IAgentFactory agentFactory)
        {
            _agentFactory = agentFactory;
        }

        public GameObject SpawnAgent(Vector3 position, Quaternion rotation)
        {
            GameObject agent = _agentFactory.CreateAgent(position, rotation);
        
            if (agent != null)
            {
                _spawnedAgents.Add(agent);
                InitializeAgentComponents(agent);
            }

            return agent;
        }

        public GameObject SpawnAgent(Vector3 position)
        {
            return SpawnAgent(position, Quaternion.identity);
        }

        public GameObject SpawnAgent()
        {
            return SpawnAgent(Vector3.zero);
        }

        public void DespawnAgent(GameObject agent)
        {
            if (agent != null && _spawnedAgents.Contains(agent))
            {
                _spawnedAgents.Remove(agent);
                Object.Destroy(agent);
            }
        }

        public void DespawnAllAgents()
        {
            foreach (var agent in _spawnedAgents)
            {
                Object.Destroy(agent);
            }
            _spawnedAgents.Clear();
        }

        private void InitializeAgentComponents(GameObject agent)
        {
            var goapAgent = agent.GetComponent<GoapAgent>();
            var knowledge = agent.GetComponent<IGoapKnowledge>() ?? new GoapKnowledgeBase();
            var sensor = agent.GetComponent<IGoapSensor>();
            var planner = new GoapPlanner();

            goapAgent?.Initialize(knowledge, planner, sensor);
        }
    }
}