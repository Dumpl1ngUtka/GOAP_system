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
        
        public HealthSensor(IHealth health)
        {
            _health = health;
        }

        IEnumerable<Fact> ISensor.GetFacts()
        {
            List<Fact> facts = new();
            float health = _health.CurrentHealth * 1f / _health.MaxHealth;
            
            if (health < HealthThreshold) 
                facts.Add(new Fact(GlobalKeys.ConditionTag.IsLow, GlobalKeys.Resources.Health));
            
            return facts;        
        }
    }
}