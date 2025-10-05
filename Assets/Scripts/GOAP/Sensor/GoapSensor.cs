using System.Collections.Generic;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Sensor
{
    public abstract class GoapSensor : IGoapSensor
    {
        protected GoapSensor(IGoapKnowledge knowledge){}
        
        public abstract IEnumerable<Fact> GetFacts();
    }
}