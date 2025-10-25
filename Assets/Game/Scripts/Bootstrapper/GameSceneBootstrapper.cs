using GOAP.Agent;
using GOAP.Spawner;
using Items;
using UnityEngine;

namespace Bootstrapper
{
    public class GameSceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private GoapAgent _agentPrefab; 
        [SerializeField] private DroppedItem _itemPrefab; 
        [SerializeField] private GameObject _serviceContainer;
            
        private void Awake()
        {
            InitSpawnService();
        }

        private void InitSpawnService()
        {
            var spawnService = Instantiate(new GameObject(), _serviceContainer.transform);
            spawnService.name = "SpawnService";
            var component = spawnService.AddComponent(typeof(SpawnService)) as SpawnService;
            
            var agentSpawner = new ObjectSpawner<GoapAgent>();
            var itemSpawner = new ObjectSpawner<DroppedItem>();
            
            component?.Init(_agentPrefab, _itemPrefab, agentSpawner, itemSpawner);
        }
    }
}