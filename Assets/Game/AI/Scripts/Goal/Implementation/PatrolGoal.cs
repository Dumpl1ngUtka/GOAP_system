using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class PatrolGoal : GoalBase
    {
        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            return new[]
            {
                new GoalCondition(
                    conditionTag: GlobalKeys.ConditionTag.IsPatrolling,
                    objectTag: "",
                    mustExist: true)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            return 20f; 
        }
    }
}