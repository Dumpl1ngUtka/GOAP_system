using System.Collections.Generic;
using GOAP.Agent;
using GOAP.Goal;
using GOAP.Goal.GoalList;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using TEST;
using Unit;
using Unit.Mover;
using UnityEngine;

namespace GOAP.Spawner
{
    public class AgentSpawner : IAgentSpawner
    {
        Transform[] _partolPoints;
        private readonly IAgentFactory _agentFactory;
        private readonly List<GameObject> _spawnedAgents = new List<GameObject>();

        public IReadOnlyList<GameObject> SpawnedAgents => _spawnedAgents;

        public AgentSpawner(IAgentFactory agentFactory, Transform[] patrolPoints)
        {
            _agentFactory = agentFactory;
            _partolPoints = patrolPoints;
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
            var rigidBody = agent.GetComponent<Rigidbody>();
            var mover = agent.GetComponent<AgentMover>();
            var knowledge = new GoapKnowledgeBase();
            var planner = new CustomGoapPlanner();
            var health = new AgentHealth(100);
            var enemyInfoHolder = new AgentsInfoHolder<Enemy>();
            var alliesInfoHolder = new AgentsInfoHolder<GoapAgent>();
            var planerContainer = new PlanContainer();
            
            var healthSensor = new HealthSensor(knowledge, health);
            var enemySensor = new OutsideSensor(knowledge, rigidBody, LayerMask.GetMask("Default"), enemyInfoHolder, alliesInfoHolder);

            goapAgent?.Initialize(knowledge, planner, mover, planerContainer, health, enemyInfoHolder,alliesInfoHolder, _partolPoints, healthSensor, enemySensor);
        }
    }
}