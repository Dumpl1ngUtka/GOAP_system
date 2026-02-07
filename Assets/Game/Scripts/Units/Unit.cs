using AI.Agent;
using Units.Config;
using UnityEngine;

namespace Units
{
    public class Unit : MonoBehaviour
    { 
        [SerializeField] private UnitBaseConfig _unitConfig;
        [SerializeField] private AIAgent _aiAgent;
        
        private Parameters _parameters;
        private IHealth _health;

        public void Init(UnitConfig unitConfig)
        {
            _parameters = new Parameters(unitConfig);
            _health = new AgentHealth(_unitConfig, _parameters);
            _aiAgent.Constructor(GlobalKeys.Team.Alpha, _unitConfig, _health);
        }
    }
}