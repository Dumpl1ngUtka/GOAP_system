using System.Collections.Generic;
using GOAP.Agent;
using GOAP.Goal;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Sensor
{
    public class OutsideSensor : GoapSensor
    {
        private float _nearRadius = 3f;
        private float _aroundRadius = 30f;
        
        private GoapAgent _agent;
        private Rigidbody _rigidbody;
        private LayerMask _layerMask;
        private Collider _selfCollider;
        private AgentsInfoHolder<GoapAgent> _enemyInfoHolder;
        private AgentsInfoHolder<GoapAgent> _alliesInfoHolder;
        
        public OutsideSensor(
            IGoapKnowledge knowledge, 
            GoapAgent agent, 
            LayerMask layerMask, 
            AgentsInfoHolder<GoapAgent> enemyInfoHolder,
            AgentsInfoHolder<GoapAgent> alliesInfoHolder) : base(knowledge)
        {
            _agent = agent;
            _rigidbody = agent.GetComponent<Rigidbody>();
            _layerMask = layerMask;
            _enemyInfoHolder = enemyInfoHolder;
            _alliesInfoHolder = alliesInfoHolder;
            _selfCollider = agent.GetComponent<Collider>();
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
                
                var additionTags = new List<ObjectForFactTag>();
                IObjectForFact factObj = null;
                
                if (collider.TryGetComponent<GoapAgent>(out var agent))
                {
                    if (_agent.TeamId == agent.TeamId)
                    {
                        _alliesInfoHolder.AddAgent(agent);
                        additionTags.Add(ObjectForFactTag.Ally);
                    }
                    else
                    {
                        _enemyInfoHolder.AddAgent(agent);
                        additionTags.Add(ObjectForFactTag.Enemy);
                    }
                    factObj = agent;
                } 

                if (factObj == null)
                    continue;

                var distanceTag = GetDistanceTag(_rigidbody.position, collider.transform.position);
                facts.Add(new Fact(distanceTag, factObj).WithAdditionObjectForFactTags(additionTags));
            }
            return facts;
        }

        private FactTag GetDistanceTag(Vector3 pos1, Vector3 pos2) => 
            Vector3.Distance(pos1, pos2) <= _nearRadius? FactTag.Nearby : FactTag.Around;
    }
}