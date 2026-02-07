using System;
using System.Collections.Generic;
using System.Linq;
using AI.Base;
using AI.Knowledge;
using UnityEngine;

namespace AI.Sensors
{
    public class SensorHolder : IComponentAI
    {
        public event Action FactsChanged;
        
        private readonly List<ISensor> _sensors;
        private bool _isRunning;
        
        public SensorHolder(params ISensor[] sensors)
        {
            _sensors = sensors.ToList();
            foreach (var sensor in _sensors)
            {
                sensor.Changed += OnSensorChanged;
            }
        }
        
        public void Start()
        {
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Update(float deltaTime)
        {
            if (!_isRunning) return;
            
            foreach (ISensor sensor in _sensors)
            {
                sensor.Update(deltaTime);
            }
        }

        public void AddSensor(ISensor sensor)
        {
            _sensors.Add(sensor);
            sensor.Changed += OnSensorChanged;
        }

        public void RemoveSensor(ISensor sensor)
        {
            sensor.Changed -= OnSensorChanged;
            _sensors.Remove(sensor);
        }

        public IEnumerable<Fact> GetFactsFromAllSensors()
        {
            List<Fact> facts = new();
            
            foreach (ISensor sensor in _sensors) 
                facts.AddRange(sensor.GetFacts());

            return facts;
        }

        private void OnSensorChanged()
        {
            FactsChanged?.Invoke();
        }
    }
}