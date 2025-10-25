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
        private GoapAgent _agent;
        private DroppedItem _droppedItem;
        private ISpawner<GoapAgent> _agentSpawner;
        private ISpawner<DroppedItem> _itemSpawner;
        
        public void Init(GoapAgent agentPrefab, DroppedItem itemPrefab, ISpawner<GoapAgent> agentSpawner, ISpawner<DroppedItem> itemSpawner)
        {
            _agent = agentPrefab;
            _droppedItem = itemPrefab;
            _agentSpawner = agentSpawner;   
            _itemSpawner = itemSpawner;
        }

        public void SpawnAgent(int teamID, Vector3 position, Quaternion rotation)
        {
            var agent = _agentSpawner.Spawn(_agent, position, rotation);
                        
            var goapAgent = agent.GetComponent<GoapAgent>();
            var mover = agent.GetComponent<AgentMover>();
            var knowledge = new GoapKnowledgeBase();
            var planner = new CustomGoapPlanner();
            var health = new AgentHealth(100);
            var enemyInfoHolder = new AgentsInfoHolder<GoapAgent>();
            var alliesInfoHolder = new AgentsInfoHolder<GoapAgent>();
            var planerContainer = new PlanContainer();
            
            var healthSensor = new HealthSensor(knowledge, health);
            var enemySensor = new OutsideSensor(knowledge, goapAgent, LayerMask.GetMask("Default"), enemyInfoHolder, alliesInfoHolder);

            goapAgent?.Initialize(teamID, knowledge, planner, mover, planerContainer, health, enemyInfoHolder,alliesInfoHolder, healthSensor, enemySensor);
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