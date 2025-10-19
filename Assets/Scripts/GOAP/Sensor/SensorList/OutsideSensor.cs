using System.Collections.Generic;
using GOAP.Agent;
using GOAP.Goal.GoalList;
using GOAP.KnowledgeBase;
using TEST;
using UnityEngine;

namespace GOAP.Sensor
{
    public class OutsideSensor : GoapSensor
    {
        private float _nearRadius = 3f;
        private float _aroundRadius = 10f;
        
        private Rigidbody _rigidbody;
        private LayerMask _layerMask;
        private Collider _selfCollider;
        private EnemyInfoHolder _enemyInfoHolder;
        
        public OutsideSensor(IGoapKnowledge knowledge, Rigidbody rigidbody, LayerMask layerMask, EnemyInfoHolder infoHolder) : base(knowledge)
        {
            _rigidbody = rigidbody;
            _layerMask = layerMask;
            _enemyInfoHolder = infoHolder;
            _selfCollider = rigidbody.GetComponent<Collider>();
        }

        public override IEnumerable<Fact> GetFacts()
        {
            var facts = new List<Fact>();
            _enemyInfoHolder.Clear();
            var colliders = Physics.OverlapSphere(_rigidbody.position, _aroundRadius);
            
            foreach (var collider in colliders)
            {
                if (collider == _selfCollider)
                    continue;
                
                IObjectForFact factObj = null;
                if (collider.TryGetComponent<GoapAgent>(out var agent)) 
                    factObj = agent;

                if (collider.TryGetComponent<Enemy>(out var enemy))
                {
                    factObj = enemy;
                    _enemyInfoHolder.AddEnemy(enemy);
                }

                if (factObj == null)
                    continue;
                
                var factTag = Vector3.Distance(_rigidbody.position, collider.transform.position) <= _nearRadius?
                    FactTag.Nearby : FactTag.Around;
                    
                facts.Add(new Fact(factTag, factObj));
            }
            return facts;
        }
        
    }
}