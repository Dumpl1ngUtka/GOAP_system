using Items;
using Units.Config;
using Units.UnitClasses;
using UnityEngine;

namespace Services.Spawner
{
    public class SpawnService
    {
        private readonly Units.Unit _unitPrefab;
        private readonly DroppedItem _droppedItemPrefab;
        private readonly ISpawner<Units.Unit> _unitSpawner;
        private readonly ISpawner<DroppedItem> _itemSpawner;

        public SpawnService(
            Units.Unit unitPrefab, 
            DroppedItem itemPrefab, 
            ISpawner<Units.Unit> unitSpawner, 
            ISpawner<DroppedItem> itemSpawner)
        {
            _unitPrefab = unitPrefab;
            _droppedItemPrefab = itemPrefab;
            _unitSpawner = unitSpawner;   
            _itemSpawner = itemSpawner;
        }

        public void SpawnAgent(UnitClassVariantConfig config, Vector3 position, Quaternion rotation)
        {
            _unitSpawner.Spawn(_unitPrefab, position, rotation, new []{config});
        }
        
        public void SpawnItem(ItemVariantConfig itemVariantConfig, Vector3 position, Quaternion rotation)
        {
            _itemSpawner.Spawn(_droppedItemPrefab, position, rotation, new []{itemVariantConfig});
        }
    }
}