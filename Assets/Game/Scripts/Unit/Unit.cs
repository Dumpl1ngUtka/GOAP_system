using System;
using AI.Agent;
using Unit.Config;
using UnityEngine;

namespace Unit
{
    public class Unit : MonoBehaviour
    { 
        [SerializeField] private UnitBaseConfig _unitConfig;
        [SerializeField] private AIAgent _aiAgent;
        
        private Parameters _parameters;
        private IHealth _health;

        private void Init(ParametersConfig parametersConfig)
        {
            _parameters = new Parameters(parametersConfig);
            _health = new AgentHealth(_unitConfig, _parameters);
            _aiAgent.Constructor(_unitConfig, _health);
        }
    }
}