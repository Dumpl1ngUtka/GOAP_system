using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class IdleGoal : GoalBase
    {
        public override IEnumerable<GoalCondition> GetDesiredState() => new GoalCondition[]{};

        public override float GetPriority(IEnumerable<Fact> knowledgeBase) => 1f;
    }
}