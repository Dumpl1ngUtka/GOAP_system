using System;
using AI.Agent;
using Config;
using GOAP.Agent;
using Services.Spawner;
using Units.Config;
using Units.UnitClasses;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Units
{
    public class Unit : MonoBehaviour
    { 
        public UnitClassVariantConfig Config { get; private set; }
        public UnitInventory Inventory { get; private set; }
        public Parameters Parameters { get; private set; }
        public IHealth Health { get; private set; }
        public AgentUI UI;

        [Inject]
        public void Construct(
            GameConfig gameConfig,
            UnitClassVariantConfig unitConfig,
            SpawnService spawnService)
        {
            Config = unitConfig;
            Parameters = new Parameters(unitConfig);
            Health = new AgentHealth(gameConfig.UnitBaseConfig, Parameters);
            Inventory = new UnitInventory(spawnService, transform);
        }

        private void OnEnable()
        {
            Health.Changed += HandleChange; 
            Health.Died += Die; 
        }


        private void OnDisable()
        {
            Health.Changed -= HandleChange; 
            Health.Died -= Die; 
        }
        
        private void Die()
        {
            Destroy(gameObject);
        }

        private void HandleChange()
        {
            UI.HealthChanged((ushort)Health.CurrentHealth, (ushort)Health.MaxHealth);
        }
    }
}