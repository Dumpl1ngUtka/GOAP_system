using GOAP.Agent;
using GOAP.Goal;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using Items;
using Units.Config;
using UnityEngine;

namespace GOAP.Spawner
{
    public class SpawnService : MonoBehaviour
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

        public void SpawnAgent(UnitConfig config, Vector3 position, Quaternion rotation)
        {
            Units.Unit unit = _unitSpawner.Spawn(_unitPrefab, position, rotation);
            unit.Init(config);
        }
        
        public void SpawnItem(Item item, Vector3 position, Quaternion rotation)
        {
            DroppedItem droppedItem = _itemSpawner.Spawn(_droppedItemPrefab, position, rotation);
            droppedItem.Init(item);
        }
    }
}