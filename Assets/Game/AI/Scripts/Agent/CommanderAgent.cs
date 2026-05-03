using System.Collections.Generic;
using System.Linq;
using AI.Global;
using AI.GlobalRegistry;
using AI.Knowledge;
using AI.Sensors;
using AI.Squads;
using AI.Squads.Orders;
using Environment;
using Units.UnitClasses;
using UnityEngine;
using Zenject;

namespace AI.Agent
{
    public class CommanderAgent : MonoBehaviour
    {
        [Header("Commander Settings")] public string TeamKey = "TeamA";

        // Подчиненные юниты (это и есть тот самый слой, который позже можно заменить на Отряды/Squads)
        private List<AIAgent> _subordinateAgents = new List<AIAgent>();

        // Глобальная база знаний командира
        private IKnowledge _globalKnowledge;
        private SensorHolder _globalSensors;
        [SerializeField] private GameObject _squadPrefab; // Пустой ГО со скриптом Squad

        // Свободные юниты, которые пока не в отрядах
        private List<AIAgent> _unassignedAgents = new List<AIAgent>();

        // Активные отряды
        private List<Squad> _activeSquads = new List<Squad>();

        // Трекинг наших зданий
        private int _aliveTowersCount = 3;

        private float _evaluationTimer = 0f;
        private const float EVALUATION_INTERVAL = 2f; // Оцениваем карту раз в 2 секунды

        private CommanderRegistry _commanderRegistry;

        [Inject]
        public void Construct(CommanderRegistry commanderRegistry)
        {
            _commanderRegistry = commanderRegistry;
        }
        
        private void Awake()
        {
            // Сенсоры командира "видят" глобальную карту (ресурсы, захваченные точки, базы врагов)
            _globalSensors = new SensorHolder(
                // Пример: GlobalResourceSensor, GlobalThreatSensor
            );

            _globalKnowledge = new AIKnowledge(_globalSensors);
        }

        private void OnEnable()
        {
            Debug.Log(_commanderRegistry);
            if (_commanderRegistry != null)
            {
                _commanderRegistry.RegisterCommander(TeamKey, this);
            }
            // Подписываемся на события мира
            AIGlobalRegistry.OnOreSpawned += HandleOreSpawned;
            AIGlobalRegistry.OnOreDepleted += HandleOreDepleted;
            
            AIGlobalRegistry.OnStructureSpawned += HandleStructureSpawned;
            AIGlobalRegistry.OnStructureAttacked += HandleStructureAttacked;
            AIGlobalRegistry.OnStructureDestroyed += HandleStructureDestroyed;
        }

        private void OnDisable()
        {
            if (_commanderRegistry != null)
            {
                _commanderRegistry.UnregisterCommander(TeamKey);
            }
            // Отписываемся, чтобы не было утечек памяти
            AIGlobalRegistry.OnOreSpawned -= HandleOreSpawned;
            AIGlobalRegistry.OnOreDepleted -= HandleOreDepleted;
            
            AIGlobalRegistry.OnStructureSpawned -= HandleStructureSpawned;
            AIGlobalRegistry.OnStructureAttacked -= HandleStructureAttacked;
            AIGlobalRegistry.OnStructureDestroyed -= HandleStructureDestroyed;
        }

        public void RegisterAgent(AIAgent agent)
        {
            if (!_unassignedAgents.Contains(agent))
                _unassignedAgents.Add(agent);
        }

        private void Update()
        {
            _evaluationTimer += Time.deltaTime;
            if (_evaluationTimer >= EVALUATION_INTERVAL)
            {
                EvaluateGlobalStrategy();
                _evaluationTimer = 0f;
            }

            for (int i = _activeSquads.Count - 1; i >= 0; i--)
            {
                Squad squad = _activeSquads[i];
                if (squad.CurrentOrder == null || squad.CurrentOrder.IsOrderCompleted(_globalKnowledge))
                {
                    DisbandSquad(squad);
                }
            }
        }

        public void UnregisterAgent(AIAgent agent)
        {
            if (_subordinateAgents.Contains(agent))
                _subordinateAgents.Remove(agent);
        }

        private void EvaluateGlobalStrategy()
        {
            UpdateFriendlyStructuresState();

            // Дебаг: смотрим, сколько вообще фактов знает командир
            // Debug.Log($"Commander evaluating. Total global facts: {_globalKnowledge.GetAllFacts().Count()}");

            if (CheckAndProcessDefense())
            {
                Debug.Log("Commander: Defending base!");
                return;
            }

            ProcessMiningEconomy();
            ProcessAssaultStrategy();
        }

        private void ProcessAssaultStrategy()
        {
            var enemyTowers = _globalKnowledge.GetAllFactsByTag(GlobalKeys.ConditionTag.Spotted, GlobalKeys.WorldObject.EnemyTower).ToList();
    
            // Если башен нет - атакуем трон
            IObjectForAI targetToAttack = null;
            if (enemyTowers.Any())
            {
                targetToAttack = enemyTowers.First().Target;
            }
            else
            {
                var enemyThrone = _globalKnowledge.GetAllFactsByTag(GlobalKeys.ConditionTag.Spotted, GlobalKeys.WorldObject.EnemyThrone).FirstOrDefault();
                if (enemyThrone != null) targetToAttack = enemyThrone.Target;
            }

            if (targetToAttack != null)
            {
                if (!IsTargetAlreadyAssigned(targetToAttack))
                {
                    Squad assaultSquad = FormSquad(neededMelee: 4, neededRanged: 3, neededSupport: 1);
                    if (assaultSquad != null)
                    {
                        assaultSquad.AssignOrder(new AssaultOrder(targetToAttack, 90f));
                        _activeSquads.Add(assaultSquad);
                    }
                    else
                    {
                        Debug.LogWarning("Commander: Want to attack, but couldn't form squad (no suitable units).");
                    }
                }
            }
            else
            {
                // Командир не видит врагов
                // Debug.Log("Commander: No enemies spotted to assault."); 
            }
        }

        private void UpdateFriendlyStructuresState()
        {
            // Считаем сколько башен осталось
            int destroyedTowers = _globalKnowledge.GetAllFactsByTag("StructureDestroyed", "FriendlyTower").Count();
            _aliveTowersCount = 3 - destroyedTowers;
        }

        // Возвращает true, если пришлось сформировать отряд защиты
        private bool CheckAndProcessDefense()
        {
            // 1. Проверяем башни. Они защищаются всегда!
            var attackedTowers = _globalKnowledge.GetAllFactsByTag("UnderAttack", "FriendlyTower").ToList();
            if (attackedTowers.Any())
            {
                IObjectForAI tower = attackedTowers.First().Target;
                if (!IsTargetAlreadyAssigned(tower))
                {
                    Squad defenseSquad = FormSquad(neededMelee: 3, neededRanged: 2, neededSupport: 1);
                    if (defenseSquad != null)
                    {
                        defenseSquad.AssignOrder(new DefendOrder(tower, 100f)); // Высший приоритет
                        _activeSquads.Add(defenseSquad);
                        return true;
                    }
                }
            }

            // 2. Проверяем Трон. УСЛОВИЕ: защищаем, только если уничтожена хотя бы 1 башня!
            if (_aliveTowersCount < 3)
            {
                var attackedThrone =
                    _globalKnowledge.GetAllFactsByTag("UnderAttack", "FriendlyThrone").FirstOrDefault();
                if (attackedThrone != null && !IsTargetAlreadyAssigned(attackedThrone.Target))
                {
                    Squad defenseSquad =
                        FormSquad(neededMelee: 5, neededRanged: 5, neededSupport: 2); // Стягиваем всю армию!
                    if (defenseSquad != null)
                    {
                        defenseSquad.AssignOrder(new DefendOrder(attackedThrone.Target, 150f)); // Приоритет выше всего
                        _activeSquads.Add(defenseSquad);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ProcessMiningEconomy()
        {
            var detectedOres = _globalKnowledge.GetAllFactsByTag(GlobalKeys.ConditionTag.Spotted, GlobalKeys.WorldObject.OreNode).ToList();

            foreach (var oreFact in detectedOres)
            {
                if (!IsTargetAlreadyAssigned(oreFact.Target))
                {
                    // Нужны только рабочие
                    Squad workerSquad = FormSquad(neededWorkers: 2);
                    if (workerSquad != null)
                    {
                        workerSquad.AssignOrder(new MineResourceOrder(oreFact.Target, 80f));
                        _activeSquads.Add(workerSquad);
                    }
                }
            }
        }

        // Вспомогательный метод: проверяет, не отправляли ли мы уже отряд к этому объекту
        private bool IsTargetAlreadyAssigned(IObjectForAI target)
        {
            // Здесь должна быть логика проверки текущих целей у активных сквадов
            // Для упрощения предполагается, что в SquadOrder есть поле Target, доступное для чтения
            return _activeSquads.Any(s => s.CurrentOrder != null && s.CurrentOrder.Target == target);
        }

        private Squad FormSquad(int neededWorkers = 0, int neededMelee = 0, int neededRanged = 0, int neededSupport = 0)
        {
            // Упрощенная логика: просто берем любых свободных агентов (для начала, чтобы система заработала)
            int totalNeeded = neededWorkers + neededMelee + neededRanged + neededSupport;
    
            if (_unassignedAgents.Count < totalNeeded || totalNeeded == 0)
                return null; // Не хватает людей для приказа

            // Создаем объект отряда
            GameObject squadObj = Instantiate(_squadPrefab);
            Squad newSquad = squadObj.GetComponent<Squad>();
            newSquad.Initialize(_globalKnowledge);

            // Переводим агентов из резерва в отряд
            for (int i = 0; i < totalNeeded; i++)
            {
                AIAgent agent = _unassignedAgents[0];
                _unassignedAgents.RemoveAt(0);
        
                _subordinateAgents.Add(agent);
                newSquad.AddMember(agent);
            }

            return newSquad;
        }
        
        private void DisbandSquad(Squad squad)
        {
            // Возвращаем всех юнитов отряда обратно в резерв
            foreach (var agent in squad.Members)
            {
                agent.ClearOrder(); // Снимаем приказ
                _unassignedAgents.Add(agent); // Возвращаем в пул
            }

            _activeSquads.Remove(squad);
            Destroy(squad.gameObject); // Удаляем ГО отряда
        }
        
                private void HandleOreSpawned(IObjectForAI ore)
        {
            ((AIKnowledge)_globalKnowledge).AddFact(new Fact(ore, GlobalKeys.ConditionTag.Spotted, GlobalKeys.WorldObject.OreNode));
        }

        private void HandleOreDepleted(IObjectForAI ore)
        {
            // Удаляем факт о руде (сработает IsOrderCompleted у MineResourceOrder)
            ((AIKnowledge)_globalKnowledge).RemoveFactsByTarget(ore);
            ((AIKnowledge)_globalKnowledge).AddFact(new Fact(ore, GlobalKeys.ConditionTag.Depleted, GlobalKeys.WorldObject.OreNode));
        }

        private void HandleStructureSpawned(IObjectForAI structure, string team)
        {
            // Убираем string structureType = string.Join("", structure.GetTags());
            // Вместо этого считываем теги из объекта и используем GlobalKeys напрямую:
            bool isTower = structure.GetTags().Contains(GlobalKeys.WorldObject.Structure); // Зависит от того, как у вас определяются башни, предположим вы найдете способ различить их.
    
            // Пример логики с ключами:
            if (team == TeamKey) 
            {
                ((AIKnowledge)_globalKnowledge).AddFact(new Fact(structure, GlobalKeys.ConditionTag.Spotted, GlobalKeys.WorldObject.FriendlyTower));
            }
            else 
            {
                ((AIKnowledge)_globalKnowledge).AddFact(new Fact(structure, GlobalKeys.ConditionTag.Spotted, GlobalKeys.WorldObject.EnemyTower));
            }
        }

        private void HandleStructureAttacked(IObjectForAI structure, string team)
        {
            string factTag = "";
            foreach (string tag in structure.GetTags())
            {
                if (tag == nameof(StructureType.Tower))
                {
                    factTag = team == TeamKey? GlobalKeys.WorldObject.FriendlyTower : GlobalKeys.WorldObject.EnemyTower;
                    break;
                }

                if (tag == nameof(StructureType.Throne))
                {
                    factTag = team == TeamKey? GlobalKeys.WorldObject.FriendlyThrone : GlobalKeys.WorldObject.EnemyTower;
                    break;
                }
            }
            ((AIKnowledge)_globalKnowledge).AddFact(new Fact(structure, GlobalKeys.ConditionTag.UnderAttack, factTag));

        }

        private void HandleStructureDestroyed(IObjectForAI structure, string team)
        {
            // Здание уничтожено. Удаляем все факты, связанные с ним.
            ((AIKnowledge)_globalKnowledge).RemoveFactsByTarget(structure);
            
            // И добавляем новый факт, чтобы отменить приказы на защиту/атаку
            ((AIKnowledge)_globalKnowledge).AddFact(new Fact(structure, GlobalKeys.ConditionTag.StructureDestroyed, "Structure"));
        }
    }
}