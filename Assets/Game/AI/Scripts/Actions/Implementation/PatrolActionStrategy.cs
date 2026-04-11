using System.Collections.Generic;
using AI.Goal;
using AI.Knowledge;
using Units.Mover;
using UnityEngine;

namespace AI.Actions.Implementation
{
    public class PatrolActionStrategy : ActionStrategy
    {
        private readonly AgentMover _agentMover;
        private readonly Transform _centerTransform; // Вокруг кого патрулировать
        private readonly float _radius;

        public PatrolActionStrategy(AgentMover agentMover, Transform centerTransform, float radius)
        {
            _agentMover = agentMover;
            _centerTransform = centerTransform;
            _radius = radius;
        }

        public override float GetCost(IObjectForAI target) => 1f;

        public override IEnumerable<GoalCondition> GetPreconditions(IObjectForAI target)
        {
            return new List<GoalCondition>();
        }

        public override IEnumerable<GoalCondition> GetEffects(IObjectForAI target)
        {
            return new[] { new GoalCondition("IsPatrolling", "Self", true) };
        }

        public override ActionBase CreateAction(IObjectForAI target)
        {
            return new PatrolAction(target, _agentMover, _centerTransform, _radius);
        }
    }
}