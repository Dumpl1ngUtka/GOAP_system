using System;
using System.Collections.Generic;
using System.Linq;
using AI.Configs;
using AI.Knowledge;
using UnityEngine;

namespace AI.Sensors
{
    public class VisualSensor : ISensor
    {
        public event Action Changed;

        private readonly Transform _selfTransform;
        private readonly string _selfTeamKey;
        private readonly AIConfig _aiConfig;
        
        public VisualSensor(
            Transform selfTransform,
            string selfTeamKey,
            AIConfig aiConfig)
        {
            _selfTransform = selfTransform;
            _selfTeamKey = selfTeamKey;
            _aiConfig = aiConfig;
        }

        public IEnumerable<Fact> GetFacts()
        {
            List<Fact> facts = new();
            Collider[] colliders = Physics.OverlapSphere(_selfTransform.position, _aiConfig.InSightDistance);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject == _selfTransform.gameObject) 
                    continue;

                IWorldObjectForAI worldObject = collider.GetComponent<IWorldObjectForAI>();
                if (worldObject == null) 
                    continue;

                Transform objTransform = worldObject.GetWorldTransform();
                if (objTransform == null) 
                    continue;

                float distance = Vector3.Distance(_selfTransform.position, objTransform.position);

                string conditionTag = null;
                if (distance < _aiConfig.NearbyDistance)
                    conditionTag = GlobalKeys.ConditionTag.Nearby;
                else if (distance <= _aiConfig.AroundDistance)
                    conditionTag = GlobalKeys.ConditionTag.Around;
                else if (distance <= _aiConfig.InSightDistance)
                    conditionTag = GlobalKeys.ConditionTag.InSight;

                if (conditionTag != null)
                {
                    HashSet<string> processedTags = new();
                    foreach (string objectTag in worldObject.GetTags())
                    {
                        if (_aiConfig.TeamKeys.Contains(objectTag))
                            processedTags.Add(objectTag == _selfTeamKey ? "Ally" : "Enemy");
                        else
                            processedTags.Add(objectTag);
                    }
                    facts.Add(new Fact(conditionTag, processedTags.ToArray()));
                }
            }

            return facts;
        }
    }
}