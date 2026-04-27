using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;
using AI.Sensors;
using AI.Squads;
using AI.Squads.Orders;
using Units.UnitClasses;
using UnityEngine;

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
            _globalSensors?.Start();
        }

        private void OnDisable()
        {
            _globalSensors?.Stop();
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
        }

        public void UnregisterAgent(AIAgent agent)
        {
            if (_subordinateAgents.Contains(agent))
                _subordinateAgents.Remove(agent);
        }

        private void EvaluateGlobalStrategy()
        {
            // Обновляем инфу о наших башнях (предположим, сенсоры добавляют факты "TowerDestroyed")
            UpdateFriendlyStructuresState();

            // ПРИОРИТЕТ 1: ЗАЩИТА (Defense)
            if (CheckAndProcessDefense()) 
                return; // Если база в опасности, бросаем все силы на защиту, атака подождет

            // ПРИОРИТЕТ 2: ДОБЫЧА РУДЫ (Economy)
            ProcessMiningEconomy();

            // ПРИОРИТЕТ 3: АТАКА (Offense)
            ProcessAssaultStrategy();
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
                var attackedThrone = _globalKnowledge.GetAllFactsByTag("UnderAttack", "FriendlyThrone").FirstOrDefault();
                if (attackedThrone != null && !IsTargetAlreadyAssigned(attackedThrone.Target))
                {
                    Squad defenseSquad = FormSquad(neededMelee: 5, neededRanged: 5, neededSupport: 2); // Стягиваем всю армию!
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
            var detectedOres = _globalKnowledge.GetAllFactsByTag("OreSpotted").ToList();
            
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

        private void ProcessAssaultStrategy()
        {
            // Логика атаки: Сначала башни, потом трон.
            var enemyTowers = _globalKnowledge.GetAllFactsByTag("EnemyTowerSpotted").ToList();
            IObjectForAI targetToAttack = null;

            if (enemyTowers.Any())
            {
                targetToAttack = enemyTowers.First().Target; // Выбираем первую попавшуюся башню
            }
            else
            {
                var enemyThrone = _globalKnowledge.GetAllFactsByTag("EnemyThroneSpotted").FirstOrDefault();
                if (enemyThrone != null)
                    targetToAttack = enemyThrone.Target;
            }

            if (targetToAttack != null && !IsTargetAlreadyAssigned(targetToAttack))
            {
                // Формируем ударную группу
                Squad assaultSquad = FormSquad(neededMelee: 4, neededRanged: 3, neededSupport: 1);
                if (assaultSquad != null)
                {
                    assaultSquad.AssignOrder(new AssaultOrder(targetToAttack, 90f));
                    _activeSquads.Add(assaultSquad);
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
            // (Логика из предыдущего ответа) ...
            // Убеждаемся что хватает _unassignedAgents нужных ролей, создаем GameObject с Squad.cs, перекидываем туда агентов.
            return null; // Заглушка
        }
    }
}