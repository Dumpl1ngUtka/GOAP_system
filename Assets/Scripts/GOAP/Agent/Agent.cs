using System;
using System.Collections;
using System.Collections.Generic;
using GOAP.SensorList;
using UnityEngine;

namespace GOAP
{
    public class Agent : MonoBehaviour
    {
        public int TeamID;
        private const float SensorUpdateDelay = 1f;

        [SerializeField] private Unit.Unit _unit;
        private float _updateTimer = 0f;
        private BeliefsHolder _beliefsHolder;
        private List<ISensor> _sensors = new List<ISensor>();
        
        private void Start()
        {
            _beliefsHolder = new BeliefsHolder();

            _sensors = new List<ISensor>()
            {
                new StatsSensor(_unit.Stats),
                GetComponent<EnemySensor>(),
            };
        }

        private void Update()
        {
            _updateTimer += Time.deltaTime;
            if (_updateTimer >= SensorUpdateDelay)
            {
                UpdateBeliefs();
                _updateTimer = 0f;
            }
        }

        public bool TryGetBelief(BeliefsList beliefName, out Belief belief)
        {
            belief = _beliefsHolder.GetBelief(beliefName);
            return belief.Key != BeliefsList.None;
        }

        private void UpdateBeliefs()
        {
            _beliefsHolder.SetBeliefs(GetBeliefsFromSensors());
            PrintBeliefs();
        }
        
        private List<Belief> GetBeliefsFromSensors()
        {
            var beliefs = new List<Belief>();
            foreach (var sensor in _sensors)
            {
                var sensorBeliefs = sensor.GetBeliefs();
                beliefs.AddRange(sensorBeliefs);
            }
            return beliefs;
        }

        private void PrintBeliefs()
        {
            Debug.Log("---------------------------------------");
            foreach (var belief in _beliefsHolder.GetAllBeliefs()) 
                Debug.Log(belief.Key + " : " + belief.Value);
        }
    }
}
