using System;
using System.Collections.Generic;
using AI.Knowledge;
using Units;

namespace AI.Sensors
{
    public class HealthSensor : ISensor
    {
        public event Action Changed;
        
        private const float HealthThreshold = 0.5f;
        
        private readonly IHealth _health;
        private float _timeSinceLastUpdate;
        private List<Fact> _cachedFacts = new();
        
        public HealthSensor(IHealth health)
        {
            _health = health;
        }

        public void Start()
        {
            _health.Changed += UpdateFacts;
        }

        public void Update(float deltaTime)
        {
        }

        public void Stop()
        {
            _health.Changed -= UpdateFacts;
        }

        private void UpdateFacts()
        {
            List<Fact> newFacts = new();
            float health = _health.CurrentHealth * 1f / _health.MaxHealth;
            
            if (health < HealthThreshold) 
                newFacts.Add(new Fact(GlobalKeys.ConditionTag.IsLow, GlobalKeys.Resources.Health));
            
            if (!AreFactsEqual(_cachedFacts, newFacts))
            {
                _cachedFacts = newFacts;
                Changed?.Invoke();
            }
        }

        IEnumerable<Fact> ISensor.GetFacts()
        {
            return _cachedFacts;
        }

        private bool AreFactsEqual(List<Fact> list1, List<Fact> list2)
        {
            if (list1.Count != list2.Count) return false;
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].ConditionTag != list2[i].ConditionTag) return false;
            }
            return true;
        }
    }
}