using System.Collections.Generic;
using AI.GlobalRegistry;
using AI.Knowledge;
using UnityEngine;

namespace Environment
{
    public class OreNode : MonoBehaviour, IWorldObjectForAI
    {
        [SerializeField] private int _resourceAmount = 100;
        
        private void Start()
        {
            AIGlobalRegistry.ReportOreSpawned(this);
        }

        public void Mine(int amount)
        {
            _resourceAmount -= amount;
            if (_resourceAmount <= 0)
            {
                Deplete();
            }
        }

        private void Deplete()
        {
            AIGlobalRegistry.ReportOreDepleted(this);
            Destroy(gameObject); 
        }

        public IEnumerable<string> GetTags()
        {
            return new[] { GlobalKeys.WorldObject.OreNode, GlobalKeys.WorldObject.Resource }; 
        }

        public Transform GetWorldTransform()
        {
            return transform;
        }
    }
}