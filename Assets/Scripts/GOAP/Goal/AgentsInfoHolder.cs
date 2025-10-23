using System.Collections.Generic;
using GOAP.Agent;
using UnityEngine;

namespace GOAP.Goal
{
    public class AgentsInfoHolder<T> where T : IGoapAgent
    {
        private List<T> _agents = new List<T>();
        
        public void AddAgent(T agent) => _agents.Add(agent);

        public List<T> GetAllAgents() => _agents;

        //TODO change
        public T GetNearestAgent(Vector2 position)
        {
            var minDistance = float.MaxValue;
            var nearestAgent = default(T);
            foreach (var agent in _agents)
            {
                if (agent == null)
                    continue;
                
                var distance = Vector3.Distance(agent.GetTransform.position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAgent = agent;
                }
            }
            return _agents.Count == 0 ? default(T) : _agents[Random.Range(0, _agents.Count)];
            
            return nearestAgent;
        }

        public void Clear() => _agents.Clear();
        
        public int GetAgentsCount() => _agents.Count;
    }
}