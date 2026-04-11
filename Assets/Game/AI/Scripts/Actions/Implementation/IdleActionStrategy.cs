using System.Collections.Generic;
using AI.Goal;
using AI.Knowledge;

namespace AI.Actions.Implementation
{
    public class IdleActionStrategy : ActionStrategy
    {
        public override float GetCost(IObjectForAI target)
        {
            return 0.1f;
        }

        public override IEnumerable<GoalCondition> GetPreconditions(IObjectForAI target)
        {
            return new GoalCondition[]{};
        }

        public override IEnumerable<GoalCondition> GetEffects(IObjectForAI target)
        {
            return new[]
            {
                new GoalCondition(GlobalKeys.ConditionTag.DoNothing, "", true)
            };
        }

        public override ActionBase CreateAction(IObjectForAI target)
        {
            return new IdleAction(target);
        }
    }
}