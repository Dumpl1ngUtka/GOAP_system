using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class KillEnemyGoal : GoalBase
    {
        private readonly IObjectForAI _forcedTarget;
        private readonly float _assignedPriority;
        
        public KillEnemyGoal(IObjectForAI targetEnemy = null, float priority = 0f)
        {
            _forcedTarget = targetEnemy;
            _assignedPriority = priority;
        }

        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            var target = _forcedTarget ?? _currentTarget;
            
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
            if (_forcedTarget != null && _assignedPriority > 0f)
                return _assignedPriority;
            
            _currentTarget = _forcedTarget ?? FindTarget(knowledgeBase);
            if (_currentTarget == null) 
                return 0f;

            bool isLowHealth = knowledgeBase.Any(f => 
                f.Target == _currentTarget && 
                f.ConditionTag == GlobalKeys.ConditionTag.IsLow && 
                f.ObjectTags.Contains(GlobalKeys.Resources.Health));

            if (isLowHealth)
                return 100f;

            var distanceFact = knowledgeBase.FirstOrDefault(f => 
                f.Target == _currentTarget && 
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