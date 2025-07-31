using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class Agent : MonoBehaviour
    {
        private const float SensorUpdateDelay = 1f;
        
        private BeliefsHolder _beliefsHolder;
        private List<ISensor> _sensors = new List<ISensor>();

        private void Start()
        {
            _beliefsHolder = new BeliefsHolder();

            StartCoroutine(UpdateBeliefs());
        }

        public bool TryGetBelief(string beliefName, out Belief belief)
        {
            belief = _beliefsHolder.GetBelief(beliefName);
            return belief.Name != "";
        }

        private IEnumerator UpdateBeliefs()
        {
            yield return new WaitForSeconds(SensorUpdateDelay);
            _beliefsHolder.SetBeliefs(GetBeliefsFromSensors());
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
    }
}
