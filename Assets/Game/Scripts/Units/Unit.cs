using AI.Agent;
using Config;
using Units.Config;
using Units.UnitClasses;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Units
{
    public class Unit : MonoBehaviour
    { 
        [SerializeField] private AIAgent _aiAgent;
        
        private Parameters _parameters;
        private IHealth _health;

        [Inject]
        public void Construct(
            GameConfig gameConfig,
            UnitClassVariantConfig unitConfig)
        {
            _parameters = new Parameters(unitConfig);
            _health = new AgentHealth(gameConfig.UnitBaseConfig, _parameters);
            _aiAgent.Constructor(GlobalKeys.Team.Alpha, gameConfig.UnitBaseConfig, _health);
        }
    }
}