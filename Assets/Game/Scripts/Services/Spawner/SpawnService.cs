using System.Collections.Generic;
using AI.Agent;
using AI.Global;
using Items;
using Units;
using Units.Config;
using Units.UnitClasses;
using UnityEngine;

namespace Services.Spawner
{
    public class SpawnService
    {
        private readonly Unit _unitPrefab;
        private readonly DroppedItem _droppedItemPrefab;
        private readonly ISpawner<Unit> _unitSpawner;
        private readonly ISpawner<DroppedItem> _itemSpawner;
        
        // НОВОЕ: Словарь командиров для распределения агентов по фракциям
        private readonly CommanderRegistry _commanderRegistry;

        public SpawnService(
            Unit unitPrefab, 
            DroppedItem itemPrefab, 
            ISpawner<Unit> unitSpawner, 
            ISpawner<DroppedItem> itemSpawner,
            CommanderRegistry commanderRegistry) // Инжектим командиров
        {
            _unitPrefab = unitPrefab;
            _droppedItemPrefab = itemPrefab;
            _unitSpawner = unitSpawner;   
            _itemSpawner = itemSpawner;
            _commanderRegistry = commanderRegistry;
        }

        public void SpawnAgent(UnitClassVariantConfig config, Vector3 position, Quaternion rotation, string team)
        {
            Unit unit = _unitSpawner.Spawn(_unitPrefab, position, rotation, new []{config});
            
            if (unit.gameObject.TryGetComponent(out AIAgent agent))
            {
                agent.Constructor(team, unit);
                
                
                Debug.Log($"SpawnService: Agent {unit.gameObject.name} spawned");
                if (_commanderRegistry.TryGetCommander(team, out CommanderAgent commander))
                {
                    commander.RegisterAgent(agent);
                }
                else
                {
                    Debug.LogWarning($"SpawnService: No Commander found for team '{team}'. Agent will act on instincts only.");
                }
            }
            else
            {
                Debug.LogError("Unit has no AIAgent component");
            }
        }
        
        public void SpawnItem(ItemVariantConfig itemVariantConfig, Vector3 position, Quaternion rotation)
        {
            _itemSpawner.Spawn(_droppedItemPrefab, position, rotation, new []{itemVariantConfig});
        }
    }
}