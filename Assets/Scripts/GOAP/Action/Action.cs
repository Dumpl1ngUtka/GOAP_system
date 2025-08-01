using System;
using UnityEngine;

namespace GOAP
{
    public abstract class Action : ScriptableObject
    {
        [SerializeField] private float _cost;
        [SerializeField] private Condition[] _conditions;
        [SerializeField] private Effect[] _effects;
        
        public float Cost => _cost;
        public Condition[] Conditions => _conditions;
        public Effect[] Effects => _effects;
        
        public Action IsComplete { get; set; }

        public virtual Action GetInstance()
        {
            var instance = CreateInstance<Action>();
            instance._cost = _cost;
            instance._conditions = _conditions;
            instance._effects = _effects;
            return instance;
        }
        
        public abstract void Do(Agent agent);
    }
}