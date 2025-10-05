using System.Collections.Generic;
using GOAP.KnowledgeBase;
using Unit;

namespace GOAP.Sensor
{
    public class HealthSensor : GoapSensor
    {
        private const float HealthThreshold = 0.5f; 
        
        private IHealth _health;
        private IGoapKnowledge _knowledge;
        
        public HealthSensor(IGoapKnowledge knowledge, IHealth health) : base(knowledge)
        {
            _knowledge = knowledge;
            _health = health;
        }
        
        public override IEnumerable<Fact> GetFacts()
        {
            var facts = new List<Fact>();
            var health = _health.CurrentHealth * 1f / _health.CurrentHealth;
            
            if (health < HealthThreshold) 
                facts.Add(new Fact(FactTag.IsLowHealth));
            
            return facts;
        }
    }
}