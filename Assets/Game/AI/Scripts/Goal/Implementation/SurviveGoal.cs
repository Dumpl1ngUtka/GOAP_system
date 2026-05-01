using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class SurviveGoal : GoalBase
    {
        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.IsHigh,
                    GlobalKeys.Resources.Health,
                    mustExist: true)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            bool isLowHealth = knowledgeBase.Any(f => 
                f.ConditionTag == GlobalKeys.ConditionTag.IsLow && 
                f.ObjectTags.Contains(GlobalKeys.Resources.Health) &&
                f.Target == null);

            if (isLowHealth)
                return 100f;

            bool isHighHealth = knowledgeBase.Any(f => 
                f.ConditionTag == GlobalKeys.ConditionTag.IsHigh && 
                f.ObjectTags.Contains(GlobalKeys.Resources.Health) &&
                f.Target == null);

            if (isHighHealth)
                return 0f;

            return 10f;
        }
    }
}