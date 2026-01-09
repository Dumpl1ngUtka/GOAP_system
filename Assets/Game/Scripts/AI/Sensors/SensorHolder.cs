using System;
using System.Collections.Generic;
using System.Linq;
using AI.Base;
using AI.Knowledge;

namespace AI.Sensors
{
    public class SensorHolder : IComponentAI
    {
        public event Action FactsChanged;
        
        private readonly List<ISensor> _sensors;
        
        public SensorHolder(params ISensor[] sensors)
        {
            _sensors = sensors.ToList();
        }
        
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void AddSensor(ISensor sensor)
        {
            _sensors.Add(sensor);
        }

        public void RemoveSensor(ISensor sensor)
        {
            _sensors.Remove(sensor);
        }

        public IEnumerable<Fact> GetFactsFromAllSensors()
        {
            List<Fact> facts = new();
            
            foreach (ISensor sensor in _sensors) 
                facts.AddRange(sensor.GetFacts());

            FactsChanged?.Invoke();
            return facts;
        }
    }
}