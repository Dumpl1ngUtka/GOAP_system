using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
using GOAP.Goal;
using GOAP.Goal.GoalList;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using OwnSystems.DamageSystem;
using Units;
using Units.Mover;
using UnityEngine;

namespace GOAP.Agent
{
    public class GoapAgent : MonoBehaviour, IGoapAgent, IDamageable, IWorldObjectForFact
    {
        private const float ReplanInterval = 2f;
        private const float UpdateKnowledgeInterval = 2f;

        [SerializeField] private AgentUI _ui;

        private int _teamId;
        private IGoapKnowledge _knowledge;
        private IGoapPlanner _planner;
        private IAgentMover _mover;
        private IHealth _health;
        private AgentsInfoHolder<GoapAgent> _enemyInfoHolder;
        private AgentsInfoHolder<GoapAgent> _alliesInfoHolder;
        private IGoapSensor[] _sensors;

        private List<IGoapGoal> _goals = new List<IGoapGoal>();
        private List<IGoapAction> _availableActions = new List<IGoapAction>();

        private IGoapAction _currentAction;
        private IGoapGoal _currentGoal;
        private IPlanContainer _planContainer;

        private float _replanTimer;
        private float _updateKnowledgeTimer;

        public IAgentMover Mover => _mover;
        public int TeamId => _teamId;
        public Transform GetTransform => transform;

        public void Initialize(
            int teamId,
            IGoapKnowledge knowledge,
            IGoapPlanner planner,
            IAgentMover mover,
            IPlanContainer planContainer,
            IHealth health,
            AgentsInfoHolder<GoapAgent> enemyInfoHolder,
            AgentsInfoHolder<GoapAgent> alliesInfoHolder,
            params IGoapSensor[] sensors)
        {
            _teamId = teamId;
            _knowledge = knowledge;
            _planner = planner;
            _sensors = sensors;
            _mover = mover;
            _health = health;
            _planContainer = planContainer;
            _enemyInfoHolder = enemyInfoHolder;
            _alliesInfoHolder = alliesInfoHolder;

            //_health.Changed += () => _ui.HealthChanged(_health.CurrentHealth, _health.MaxHealth);
            _health.Died += () => Die();

            AddGoal(new EmptyGoal().Init(planContainer));
            AddGoal(new SurviveGoal(_enemyInfoHolder).Init(planContainer));
            AddGoal(new PatrolGoal().Init(planContainer));
            AddGoal(new EliminateGoal(_enemyInfoHolder).Init(planContainer));

            AddAction(new IdleAction().Init(planContainer));
            AddAction(new MoveToAction(mover).Init(planContainer));
            AddAction(new HealAction(health).Init(planContainer));
            AddAction(new PatrolAction(mover).Init(planContainer));
            AddAction(new AttackAction().Init(planContainer));
            AddAction(new RunAwayAction(mover).Init(planContainer));

            PeriodicKnowledgeUpdate();
            Debug.Log("Agent init");
        }

        private void Die()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_knowledge == null || _planner == null) return;

            _replanTimer += Time.deltaTime;
            _updateKnowledgeTimer += Time.deltaTime;

            if (_replanTimer >= ReplanInterval)
            {
                _replanTimer = 0f;
                Replan();
            }

            if (_updateKnowledgeTimer >= UpdateKnowledgeInterval)
            {
                _updateKnowledgeTimer = 0f;
                PeriodicKnowledgeUpdate();
            }

            ExecuteCurrentPlan();
        }

        private void Replan()
        {
            var goals = _goals.ToList();
            while (goals.Count > 0)
            {
                var bestGoal = GetBestGoal(goals);
                if (bestGoal == _currentGoal)
                {
                    return;
                }

                if (TryCreatePlanForGoal(bestGoal, out var plan))
                {
                    AbortCurrentPlan();
                    _planContainer.SetPlan(plan);
                    SwitchGoal(bestGoal);
                    return;
                }

                goals.Remove(bestGoal);
            }
        }

        private IGoapGoal GetBestGoal(List<IGoapGoal> goals)
        {
            IGoapGoal bestGoal = null;
            var highestPriority = float.MinValue;

            foreach (var goal in goals)
            {
                if (goal.IsValid(_knowledge) && goal.GetPriority(_knowledge) > highestPriority)
                {
                    highestPriority = goal.GetPriority(_knowledge);
                    bestGoal = goal;
                }
            }

            return bestGoal;
        }

        private void SwitchGoal(IGoapGoal newGoal)
        {
            _currentGoal?.OnGoalDeactivated();
            _currentGoal = newGoal;
            _currentGoal?.OnGoalActivated();
            SetNewCurrentAction();
            Debug.Log("New Goal: " + _currentGoal);
        }

        private void ExecuteCurrentPlan()
        {
            if (_currentAction == null)
            {
                AbortCurrentPlan();
                return;
            }

            if (_currentAction.IsDone)
            {
                Debug.Log("Current action is Done");
                HandleActionCompletion();
                return;
            }

            if (_currentAction.IsFailed)
            {
                Debug.Log("Current action is failed");
                AbortCurrentPlan();
                return;
            }

            if (!_currentAction.CheckProceduralPrecondition(_knowledge.GetAllFacts()))
            {
                Debug.Log("Current action is not procedural precondition");
                AbortCurrentPlan();
                return;
            }

            _currentAction.Perform();
        }

        private void HandleActionCompletion()
        {
            _currentAction.OnExit();
            _currentAction = null;

            if (!_planContainer.IsPlanValid())
            {
                _planContainer.ResetPlan();
                return;
            }

            SetNewCurrentAction();
        }

        public void AddGoal(IGoapGoal goal)
        {
            if (!_goals.Contains(goal))
                _goals.Add(goal);
        }

        public void RemoveGoal(string goalName)
        {
            _goals.RemoveAll(g => g.Name == goalName);
        }

        public void AddAction(IGoapAction action)
        {
            _availableActions.Add(action);
        }

        private bool TryCreatePlanForGoal(IGoapGoal goal, out Queue<IGoapAction> plan)
        {
            plan = new Queue<IGoapAction>();

            if (goal == null || !goal.IsValid(_knowledge))
                return false;

            plan = _planner.Plan(_knowledge, goal, _availableActions);
            if (plan == null || plan.Count == 0)
            {
                Debug.Log("Not a plan for goal: " + goal.Name);
                return false;
            }

            return true;
        }

        private void AbortCurrentPlan()
        {
            if (_currentAction != null)
            {
                _currentAction.OnExit();
                _currentAction = null;
            }

            _currentGoal?.OnGoalDeactivated();
            _currentGoal = null;
            _planContainer.ResetPlan();
        }

        private void OnDestroy()
        {
            AbortCurrentPlan();
            _currentGoal?.OnGoalDeactivated();
        }

        private void PeriodicKnowledgeUpdate()
        {
            _knowledge.RemoveAllFacts();

            foreach (var sensor in _sensors)
            foreach (var fact in sensor.GetFacts())
                _knowledge.SetFact(fact);

            if (_knowledge.GetAllFacts().Count() != 0)
            {
                Debug.Log("Sensors: ");
                foreach (var fact in _knowledge.GetAllFacts())
                {
                    var s = fact.ObjectTags.Aggregate("", (current, objectTag) => current + (objectTag + " "));
                    Debug.Log(fact.Tag + " - tag with " + fact.ObjectTags.Count() + " count " + s);
                }
            }
        }

        private void SetNewCurrentAction()
        {
            _currentAction = _planContainer.Dequeue();
            _currentAction.OnEnter();
            Debug.Log("New current action = " + _currentAction);
        }

        public void ApplyDamage(float damage)
        {
            var uDamage = (ushort)damage;
            _health.ApplyDamage(uDamage);
        }

        public IEnumerable<ObjectForFactTag> GetTags()
        {
            return new List<ObjectForFactTag>
            {
                //ObjectForFactTag.Enemy
            };
        }

        public Transform GetWorldTransform() =>
            transform;
    }
}