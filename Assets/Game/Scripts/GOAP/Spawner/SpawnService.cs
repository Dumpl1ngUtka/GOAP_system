using GOAP.Agent;
using GOAP.Goal;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using Items;
using Unit;
using Unit.Mover;
using UnityEngine;

namespace GOAP.Spawner
{
    public class SpawnService : MonoBehaviour
    {
        private Unit.Unit _unit;
        private DroppedItem _droppedItem;
        private ISpawner<GoapAgent> _agentSpawner;
        private ISpawner<DroppedItem> _itemSpawner;
        
        public void Init(GoapAgent agentPrefab, DroppedItem itemPrefab, ISpawner<GoapAgent> agentSpawner, ISpawner<DroppedItem> itemSpawner)
        {
            //_unit = agentPrefab;
            _droppedItem = itemPrefab;
            _agentSpawner = agentSpawner;   
            _itemSpawner = itemSpawner;
        }

        public void SpawnAgent(int teamID, Vector3 position, Quaternion rotation)
        {
            //var agent = _agentSpawner.Spawn(_unit, position, rotation);
                        
            
        }
        
        public void SpawnItem(Item item, Vector3 position, Quaternion rotation)
        {
            var droppedItem = _itemSpawner.Spawn(_droppedItem, position, rotation);
            droppedItem.Init(item);
        }

        public void TMP_SpawnAgent(int teamID)
        {
            SpawnAgent(teamID, Vector3.zero, Quaternion.identity);
        }
    }
}