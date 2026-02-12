using System.Collections.Generic;
using AI.Goal;
using AI.Knowledge;
using UnityEngine;

namespace AI.Actions.Implementation
{
    public class AttackActionStrategy : ActionStrategy
    {
        private readonly Transform _selfTransform;
        private readonly float _attackCooldown;
        private readonly int _damage;

        public AttackActionStrategy(Transform selfTransform, float attackCooldown, int damage)
        {
            _selfTransform = selfTransform;
            _attackCooldown = attackCooldown;
            _damage = damage;
        }

        public override float GetCost(IObjectForAI target)
        {
            return 1.0f;
        }

        public override IEnumerable<GoalCondition> GetPreconditions(IObjectForAI target)
        {
            return new GoalCondition[]
            {
                new(GlobalKeys.ConditionTag.Nearby, "Enemy", true),
            };
        }

        public override IEnumerable<GoalCondition> GetEffects(IObjectForAI target)
        {
            return new GoalCondition[]
            {
                new(GlobalKeys.ConditionTag.Nearby, "Enemy", false),
            };
        }

        public override ActionBase CreateAction(IObjectForAI target)
        {
            return new AttackAction(_selfTransform, target, _attackCooldown, _damage);
        }
    }
}