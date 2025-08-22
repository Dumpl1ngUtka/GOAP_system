using GOAP.KnowledgeBase;
using Unit;
using UnityEngine;

namespace GOAP.Sensor
{
    public class BasicSensor : MonoBehaviour, IGoapSensor
    {
        private IGoapKnowledge _knowledge;
        private IHealth _health;
    
        public void Initialize(IGoapKnowledge knowledge, IHealth health)
        {
            _knowledge = knowledge;
            _health = health;
        }

        public void UpdateKnowledge()
        {
            if (_knowledge == null || _health == null) return;

            _knowledge.SetFact("isHealthLow", _health.IsHealthLow);
        
            GameObject nearestEnemy = FindNearestEnemy();
            _knowledge.SetObject("nearestEnemy", nearestEnemy);
            _knowledge.SetFact("seeEnemy", nearestEnemy != null);
        }

        private GameObject FindNearestEnemy()
        {
            return null;
        }
    }
}