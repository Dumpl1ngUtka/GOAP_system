// --- START OF FILE VisualSensor.cs ---

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
        
        private const float UpdateInterval = 0.2f;
        private float _timeSinceLastUpdate;
        private List<Fact> _cachedFacts = new();

        public VisualSensor(
            Transform selfTransform,
            string selfTeamKey,
            AIConfig aiConfig)
        {
            _selfTransform = selfTransform;
            _selfTeamKey = selfTeamKey;
            _aiConfig = aiConfig;
        }

        public void Start() { }

        public void Update(float deltaTime)
        {
            _timeSinceLastUpdate += deltaTime;
            if (_timeSinceLastUpdate >= UpdateInterval)
            {
                _timeSinceLastUpdate = 0;
                UpdateFacts();
            }
        }

        public void Stop() { }

        private void UpdateFacts()
        {
            List<Fact> newFacts = new();
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
                    

                    // Обрабатываем базовые теги
                    foreach (string objectTag in worldObject.GetTags())
                    {
                        if (_aiConfig.TeamKeys != null && _aiConfig.TeamKeys.Contains(objectTag))
                        {
                            // ИСПОЛЬЗУЕМ GLOBAL KEYS вместо захардкоженных "Ally" / "Enemy"
                            processedTags.Add(objectTag == _selfTeamKey ? GlobalKeys.WorldObject.Ally : GlobalKeys.WorldObject.Enemy);
                        }
                        else
                        {
                            processedTags.Add(objectTag);
                        }
                    }

                    // --- СПЕЦИАЛЬНАЯ ЛОГИКА ДЛЯ ЗДАНИЙ ---
                    // Если это вражеское здание, принудительно делаем его Врагом (чтобы KillEnemyGoal сработал)
                    if (processedTags.Contains(GlobalKeys.WorldObject.EnemyTower) || 
                        processedTags.Contains(GlobalKeys.WorldObject.EnemyThrone))
                    {
                        processedTags.Add(GlobalKeys.WorldObject.Enemy);
                        processedTags.Add(GlobalKeys.WorldObject.Structure);
                    }
                    // Если союзное
                    else if (processedTags.Contains(GlobalKeys.WorldObject.FriendlyTower) || 
                             processedTags.Contains(GlobalKeys.WorldObject.FriendlyThrone))
                    {
                        processedTags.Add(GlobalKeys.WorldObject.Ally);
                        processedTags.Add(GlobalKeys.WorldObject.Structure);
                    }

                    newFacts.Add(new Fact(worldObject, conditionTag, processedTags.ToArray()));
                }
            }
            
            if (!AreFactsEqual(_cachedFacts, newFacts))
            {
                _cachedFacts = newFacts;
                Changed?.Invoke();
            }
        }

        public IEnumerable<Fact> GetFacts() => _cachedFacts;

        private bool AreFactsEqual(List<Fact> list1, List<Fact> list2)
        {
            if (list1.Count != list2.Count) return false;
            if (list1.Count == 0) return true;

            foreach (var f1 in list1)
            {
                bool matchFound = false;
                foreach (var f2 in list2)
                {
                    if (FactsMatch(f1, f2))
                    {
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound) return false;
            }
            return true;
        }

        private bool FactsMatch(Fact f1, Fact f2)
        {
            if (f1.Target != f2.Target) return false;
            if (f1.ConditionTag != f2.ConditionTag) return false;
            if (f1.ObjectTags.Length != f2.ObjectTags.Length) return false;
            
            var set1 = new HashSet<string>(f1.ObjectTags);
            return set1.SetEquals(f2.ObjectTags);
        }
    }
}