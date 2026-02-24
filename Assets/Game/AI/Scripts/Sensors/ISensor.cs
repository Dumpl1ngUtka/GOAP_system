using System;
using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Sensors
{
    public interface ISensor
    {
        event Action Changed;
        IEnumerable<Fact> GetFacts();
        void Start();
        void Update(float deltaTime);
        void Stop();
    }
}