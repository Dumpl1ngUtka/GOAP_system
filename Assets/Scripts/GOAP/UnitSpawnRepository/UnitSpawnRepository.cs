using System;
using TMP_spawner;
using UnityEngine;

namespace GOAP.UnitSpawnRepository
{
    public class UnitSpawnRepository : MonoBehaviour
    {
        [SerializeField] private Spawner _spawner;
        private AgentDataBase _dataBase;

        private void Start()
        {
            _spawner.Spawned += OnSpawn;
        }

        private void OnSpawn(Agent agent)
        {
            //_dataBase.Add;
        }
    }
}