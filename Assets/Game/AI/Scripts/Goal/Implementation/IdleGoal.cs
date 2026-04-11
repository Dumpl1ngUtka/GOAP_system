using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class IdleGoal : GoalBase
    {
        public override IEnumerable<GoalCondition> GetDesiredState() => new GoalCondition[]
        {
            new GoalCondition(GlobalKeys.ConditionTag.DoNothing, "", true)
        };

        public override float GetPriority(IEnumerable<Fact> knowledgeBase) => 1f;
    }
}