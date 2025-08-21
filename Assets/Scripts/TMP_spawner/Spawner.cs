using System;
using GOAP;
using UnityEngine;
using UnityEngine.Serialization;
using Action = System.Action;

namespace TMP_spawner
{
    public class Spawner : MonoBehaviour
    {
        [Header("Enemies")]
        [SerializeField] private Agent _enemyAgent;
        [SerializeField] private Transform _enemySpawnPoint;
        [Header("Player")]
        [SerializeField] private Agent _playerAgent;
        [SerializeField] private Transform _playerSpawnPoint;

        public bool SpawnEnemyUnit = false;
        public bool SpawnPlayerUnit = false;

        public Action<Agent> Spawned;
        
        private void OnValidate()
        {
            if (SpawnEnemyUnit)
            {
                Spawn(_enemyAgent, _enemySpawnPoint);
                SpawnEnemyUnit = false;
            }

            if (SpawnPlayerUnit)
            {
                Spawn(_playerAgent, _playerSpawnPoint);
                SpawnPlayerUnit = false;
            }
        }

        private void Spawn(Agent agent, Transform spawnPoint)
        {
            var instantiate = Instantiate(agent, spawnPoint);
            Spawned?.Invoke(instantiate);
        }
    }
}
