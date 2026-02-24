using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class KillEnemyGoal : GoalBase
    {
        private readonly IObjectForAI _forcedTarget;

        public KillEnemyGoal(IObjectForAI targetEnemy = null)
        {
            _forcedTarget = targetEnemy;
        }

        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            var target = _forcedTarget ?? _forcedTarget;
            
            if (target == null)
                return Enumerable.Empty<GoalCondition>();

            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.Nearby,
                    GlobalKeys.WorldObject.Enemy,
                    mustExist: false,
                    specificObject: target)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            var currentTarget = _forcedTarget ?? FindTarget(knowledgeBase);

            if (currentTarget == null)
                return 0f;

            bool isLowHealth = knowledgeBase.Any(f => 
                f.Target == currentTarget && 
                f.ConditionTag == GlobalKeys.ConditionTag.IsLow && 
                f.ObjectTags.Contains(GlobalKeys.Resources.Health));

            if (isLowHealth)
                return 100f;

            var distanceFact = knowledgeBase.FirstOrDefault(f => 
                f.Target == currentTarget && 
                (f.ConditionTag == GlobalKeys.ConditionTag.Nearby || 
                 f.ConditionTag == GlobalKeys.ConditionTag.Around || 
                 f.ConditionTag == GlobalKeys.ConditionTag.InSight));
            
            if (distanceFact == null)
                return 0f;

            if (distanceFact.ConditionTag == GlobalKeys.ConditionTag.Nearby)
                return 80f;
            
            if (distanceFact.ConditionTag == GlobalKeys.ConditionTag.Around)
                return 60f;
            
            if (distanceFact.ConditionTag == GlobalKeys.ConditionTag.InSight)
                return 40f;

            return 0f;
        }

        private IObjectForAI FindTarget(IEnumerable<Fact> knowledgeBase)
        {
            var enemies = knowledgeBase
                .Where(f => f.ObjectTags.Contains(GlobalKeys.WorldObject.Enemy) && f.Target != null)
                .Select(f => f.Target)
                .Distinct()
                .ToList();

            if (!enemies.Any())
                return null;

            var lowHealthEnemy = enemies.FirstOrDefault(enemy => 
                knowledgeBase.Any(f => f.Target == enemy && 
                                       f.ConditionTag == GlobalKeys.ConditionTag.IsLow && 
                                       f.ObjectTags.Contains(GlobalKeys.Resources.Health)));
            
            if (lowHealthEnemy != null)
                return lowHealthEnemy;

            var nearbyEnemy = enemies.FirstOrDefault(enemy => 
                knowledgeBase.Any(f => f.Target == enemy && 
                                       f.ConditionTag == GlobalKeys.ConditionTag.Nearby));
            
            if (nearbyEnemy != null)
                return nearbyEnemy;

            return enemies.FirstOrDefault();
        }
    }
}