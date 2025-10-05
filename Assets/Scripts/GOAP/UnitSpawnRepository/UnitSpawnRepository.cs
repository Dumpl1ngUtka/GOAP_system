using System;
using GOAP.Agent;
using UnityEngine;

namespace GOAP.UnitSpawnRepository
{
    public class UnitSpawnRepository : MonoBehaviour
    {
        //[SerializeField] private TMP_spawner.Spawner _spawner;
        private AgentDataBase _dataBase;

        private void Start()
        {
            //_spawner.Spawned += OnSpawn;
        }

        private void OnSpawn(GoapAgent goapAgent)
        {
            //_dataBase.Add;
        }
    }
}