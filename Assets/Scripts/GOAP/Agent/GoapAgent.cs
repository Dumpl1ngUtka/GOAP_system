using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GOAP.Action;
using GOAP.Goal;
using GOAP.Goal.GoalList;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using OwnSystems.DamageSystem;
using Unit;
using Unit.Mover;
using UnityEngine;

namespace GOAP.Agent
{
    public class GoapAgent : MonoBehaviour, IGoapAgent, IDamageable
    {
        private const float ReplanInterval = 2f;
        private const float UpdateKnowledgeInterval = 2f;
        
        [SerializeField] private AgentUI _ui;
        
        private IGoapKnowledge _knowledge;
        private IGoapPlanner _planner;
        private IAgentMover _mover;
        private IHealth _health;
        private IGoapSensor[] _sensors;

        private List<IGoapGoal> _goals = new List<IGoapGoal>();
        private List<IGoapAction> _availableActions = new List<IGoapAction>();
    
        private IGoapAction _currentAction;
        private IGoapGoal _currentGoal;
        private IPlanContainer _planContainer;
        private CancellationTokenSource _cancellationTokenSource;

        private float _replanTimer;
        private float _updateKnowledgeTimer;
        
        public IAgentMover Mover => _mover;

        public void Initialize(IGoapKnowledge knowledge, 
            IGoapPlanner planner, 
            IAgentMover mover, 
            IPlanContainer planContainer,
            IHealth health,
            Transform[] patrolPoints,
            params IGoapSensor[] sensors)
        {
            _knowledge = knowledge;
            _planner = planner;
            _sensors = sensors;
            _mover = mover;
            _health = health;
            _planContainer = planContainer;
            _cancellationTokenSource = new CancellationTokenSource();

            _health.Changed += () => _ui.HealthChanged(_health.CurrentHealth, _health.MaxHealth);
            
            AddGoal(new EmptyGoal().Init(planContainer));
            AddGoal(new SurviveGoal(health).Init(planContainer));
            AddGoal(new PatrolGoal().Init(planContainer));
            
            AddAction(new IdleAction().Init(planContainer));
            AddAction(new MoveToAction().Init(planContainer));
            AddAction(new HealAction(health).Init(planContainer));
            AddAction(new PatrolAction(patrolPoints, mover).Init(planContainer));
            
            PeriodicKnowledgeUpdate();
            Debug.Log("Agent init");
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
                Debug.Log("New best gaol: " + bestGoal.Name);
                if (TryCreatePlanForGoal(bestGoal, out var plan))
                {
                    if (bestGoal != _currentGoal)
                    {
                        Debug.Log("Applied");
                        AbortCurrentPlan();
                        _planContainer.SetPlan(plan);
                        SwitchGoal(bestGoal);
                    }
                    return;
                }
                else
                {
                    Debug.Log("Deny");
                    goals.Remove(bestGoal);
                }
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
            if (_currentAction == null) return;

            if (_currentAction.IsDone)
            {
                HandleActionCompletion();
                return;
            }

            if (!_currentAction.CheckProceduralPrecondition(_knowledge.GetAllFacts().ToList()))
            {
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
            {
                Debug.Log("Because goal is invalid");
                return false;
            }

            plan = _planner.Plan(_knowledge, goal, _availableActions);
            if (plan.Count == 0)
            {
                Debug.Log("Because plan is empty");
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
            Debug.Log("Starting periodic update");
            
            foreach (var sensor in _sensors)
                foreach (var fact in sensor.GetFacts())
                    _knowledge.SetFact(fact);
            
            if (_knowledge.GetAllFacts().Count() != 0)
            {
                Debug.Log("Sensors: ");
                foreach (var fact in  _knowledge.GetAllFacts())
                {
                    Debug.Log(fact.Tag);
                }
            }
        }

        private void SetNewCurrentAction()
        {
            _currentAction = _planContainer.Dequeue();
            _currentAction.OnEnter();
        }

        public void ApplyDamage(float damage)
        {
            var uDamage = (ushort)damage;
            _health.ApplyDamage(uDamage);
        }
    }
}