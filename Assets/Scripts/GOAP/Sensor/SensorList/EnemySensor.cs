using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP.SensorList
{
    public class EnemySensor : MonoBehaviour, ISensor
    {
        [SerializeField] private Agent _agent;
        [SerializeField] private LayerMask _layerMask;
        [Header("Area Settings")]
        [SerializeField] private float _radius;
        private Transform _areaCenter;
        private Belief[] _beliefs;
        
        private void Start()
        {
            _areaCenter = _agent.transform;
        }
        
        public Belief[] GetBeliefs()
        {
            var hits = Physics.OverlapSphere(_areaCenter.position, _radius, _layerMask);

            var list = new List<Belief>();
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Agent otherAgent))
                {
                    if (otherAgent.TeamID != _agent.TeamID) 
                        list.Add(new Belief(BeliefsList.Enemy));
                }
            }

            return list.ToArray();
        }
    }
}