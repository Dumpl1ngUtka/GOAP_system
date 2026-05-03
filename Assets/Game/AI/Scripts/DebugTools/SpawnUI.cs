using System;
using Services.Spawner;
using TMPro;
using Units.Config;
using Units.UnitClasses;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AI.DebugTools
{
    public class SpawnUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _spawnButton;
        [SerializeField] private Button _enemySpawnButton;
        [SerializeField] private  Button _nextConfigButton;
        [SerializeField] private  Button _prevConfigButton;
        [Header("Spawn Settings")]
        [SerializeField] private Transform _spawnPosition;
        [SerializeField] private Transform _enemySpawnPosition;
        [SerializeField] private UnitClassVariantConfig[] _configs;

        private int _currentConfigIndex = 0; 
            
        private SpawnService _spawnService;
        
        [Inject]
        public void Constructor(SpawnService spawnService)
        {
            _spawnService = spawnService;
        }

        private void OnEnable()
        {
            _spawnButton.onClick.AddListener(Spawn);
            _enemySpawnButton.onClick.AddListener(SpawnEnemy);
            _nextConfigButton.onClick.AddListener(IncreaseIndex);
            _prevConfigButton.onClick.AddListener(DecreaseIndex);
            
            _text.text = _configs[_currentConfigIndex].Name;
        }

        private void OnDisable()
        {
            _spawnButton.onClick.RemoveAllListeners();
            _enemySpawnButton.onClick.RemoveAllListeners();
            _nextConfigButton.onClick.RemoveAllListeners();
            _prevConfigButton.onClick.RemoveAllListeners();
        }

        private void IncreaseIndex()
        {
            _currentConfigIndex++;
            if (_currentConfigIndex >= _configs.Length)
                _currentConfigIndex = 0;

            _text.text = _configs[_currentConfigIndex].Name;
        }

        private void DecreaseIndex()
        {
            _currentConfigIndex--;
            if (_currentConfigIndex < 0)
                _currentConfigIndex = _configs.Length - 1;
            
            _text.text = _configs[_currentConfigIndex].Name;
        }

        private void Spawn()
        {
            _spawnService.SpawnAgent(_configs[_currentConfigIndex], Vector3.zero, Quaternion.identity, GlobalKeys.Team.TeamA);
        }
        
        private void SpawnEnemy()
        {
            _spawnService.SpawnAgent(_configs[_currentConfigIndex], Vector3.zero, Quaternion.identity, GlobalKeys.Team.TeamB);
        }
    }
}