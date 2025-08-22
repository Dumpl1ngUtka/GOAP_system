using System;
using GOAP;
using GOAP.Agent;
using UnityEngine;
using UnityEngine.Serialization;
using Action = System.Action;

namespace TMP_spawner
{
    public class Spawner : MonoBehaviour
    {
        [FormerlySerializedAs("_enemyAgent")]
        [Header("Enemies")]
        [SerializeField] private GoapAgent _enemyGoapAgent;
        [SerializeField] private Transform _enemySpawnPoint;
        [FormerlySerializedAs("_playerAgent")]
        [Header("Player")]
        [SerializeField] private GoapAgent _playerGoapAgent;
        [SerializeField] private Transform _playerSpawnPoint;

        public bool SpawnEnemyUnit = false;
        public bool SpawnPlayerUnit = false;

        public Action<GoapAgent> Spawned;
        
        private void OnValidate()
        {
            if (SpawnEnemyUnit)
            {
                Spawn(_enemyGoapAgent, _enemySpawnPoint);
                SpawnEnemyUnit = false;
            }

            if (SpawnPlayerUnit)
            {
                Spawn(_playerGoapAgent, _playerSpawnPoint);
                SpawnPlayerUnit = false;
            }
        }

        private void Spawn(GoapAgent goapAgent, Transform spawnPoint)
        {
            var instantiate = Instantiate(goapAgent, spawnPoint);
            Spawned?.Invoke(instantiate);
        }
    }
}
