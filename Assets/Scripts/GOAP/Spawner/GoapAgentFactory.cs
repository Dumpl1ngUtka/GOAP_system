using UnityEngine;

namespace GOAP.Spawner
{
    public class GoapAgentFactory : AgentFactory
    {
        private readonly GameObject _agentPrefab;
        private readonly Transform _container;

        public GoapAgentFactory(GameObject agentPrefab, Transform container = null)
        {
            _agentPrefab = agentPrefab;
            _container = container;
        }

        protected override GameObject AgentPrefab => _agentPrefab;
        protected override Transform Container => _container;
    }
}