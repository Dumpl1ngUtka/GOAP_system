using System.Collections.Generic;
using GOAP.Agent;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Sensor
{
    public class EnemySensor : GoapSensor
    {
        private float _radius = 15f;
        
        private Rigidbody _rigidbody;
        private LayerMask _layerMask;
        
        public EnemySensor(IGoapKnowledge knowledge, Rigidbody rigidbody, LayerMask layerMask) : base(knowledge)
        {
            _rigidbody = rigidbody;
            _layerMask = layerMask;
        }

        public override IEnumerable<Fact> GetFacts()
        {
            var facts = new List<Fact>();
            Collider[] colliders = {};
            var agentsCount = Physics.OverlapSphereNonAlloc(_rigidbody.position, _radius, colliders, _layerMask);

            if (agentsCount == 0)
                return facts;
            
            var enemyCount = 0;
            foreach (var collider in colliders)
                if (collider.TryGetComponent(out GoapAgent agent)) // && agent.Team != this.Team
                    enemyCount++;

            if (enemyCount > 0)
                facts.Add(new Fact(FactTag.IsEnemyAround));
            
            return facts;
        }
    }
}