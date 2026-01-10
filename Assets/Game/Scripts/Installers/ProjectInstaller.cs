using GOAP.Spawner;
using Items;
using Units;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Spawner")]
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private DroppedItem _itemPrefab;
        
        public override void InstallBindings()
        {
            BindSpawnService();
        }

        private void BindSpawnService()
        {
            ObjectSpawner<Unit> unitSpawner = new();
            ObjectSpawner<DroppedItem> itemsSpawner = new();
            
            Container
                .Bind<SpawnService>()
                .AsSingle()
                .WithArguments(unitSpawner, itemsSpawner, _itemPrefab, _unitPrefab);      
        }
    }
}