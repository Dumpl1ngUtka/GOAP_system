using System;
using System.Collections.Generic;
using System.Linq;
using AI.Base;
using AI.Sensors;

namespace AI.Knowledge
{
    public class AIKnowledge : IKnowledge, IComponentAI
    {
        public event Action Changed;
        
        private List<Fact> _facts = new();

        private readonly SensorHolder _sensorHolder;
        
        public AIKnowledge(SensorHolder sensorHolder)
        {
            _sensorHolder = sensorHolder;
        }
        
        public void Start()
        {
            _sensorHolder.FactsChanged += OnSensorsChanged;
        }

        public void Stop()
        {
            _sensorHolder.FactsChanged -= OnSensorsChanged;
        }


        public IEnumerable<Fact> GetAllFacts()
        {
            return _facts;
        }

        bool IKnowledge.ContainsFact(string conditionTag, params string[] objectTags) => 
            ContainsFact(conditionTag, objectTags);

        public bool ContainsFact(string conditionTag, IEnumerable<string> objectTags)
        {
            bool has = false;
            IEnumerable<string> enumerable = objectTags as string[] ?? objectTags.ToArray();
            foreach (Fact fact in _facts)
            {
                if (fact.ConditionTag != conditionTag)
                    continue;

                bool factHasAllTags = true;
                foreach (string objectTag in enumerable)
                {
                    if (!fact.ObjectTags.Contains(objectTag))
                    {
                        factHasAllTags = false; 
                        break;
                    }
                }

                if (factHasAllTags)
                {
                    has = true;
                    break;
                }
            }
            return has;
        }

        IEnumerable<Fact> IKnowledge.GetAllFactsByTag(string conditionTag, params string[] objectTags) => 
            GetAllFactsByTag(conditionTag, objectTags);

        public IEnumerable<Fact> GetAllFactsByTag(string conditionTag, IEnumerable<string> objectTags)
        {
            List<Fact> facts = new();
            IEnumerable<string> enumerable = objectTags as string[] ?? objectTags.ToArray();
            foreach (Fact fact in _facts)
            {
                if (fact.ConditionTag != conditionTag)
                    continue;

                bool factHasAllTags = true;
                foreach (string objectTag in enumerable)
                {
                    if (!fact.ObjectTags.Contains(objectTag))
                    {
                        factHasAllTags = false; 
                        break;
                    }
                }

                if (factHasAllTags) 
                    facts.Add(fact);
            }
            return facts;
        }
        
        public void RemoveAllFacts()
        {
            _facts.Clear();
            Changed?.Invoke();
        }

        private void OnSensorsChanged()
        {
            _facts = _sensorHolder.GetFactsFromAllSensors().ToList();
            Changed?.Invoke();
        }
    }
}