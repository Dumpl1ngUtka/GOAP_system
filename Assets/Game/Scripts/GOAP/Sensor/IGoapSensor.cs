using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;
using Unit;

namespace GOAP.Sensor
{
    public interface IGoapSensor
    {
        IEnumerable<Fact> GetFacts();
    }
}