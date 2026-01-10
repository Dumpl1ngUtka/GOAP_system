using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;

namespace GOAP.Sensor
{
    public interface IGoapSensor
    {
        IEnumerable<Fact> GetFacts();
    }
}