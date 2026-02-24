using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class FollowGoal : GoalBase
    {
        private readonly IObjectForAI _target;
        private readonly string _targetTag;

        public FollowGoal(IObjectForAI target, string targetTag)
        {
            _target = target;
            _targetTag = targetTag;
        }

        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.Nearby,
                    _targetTag,
                    mustExist: true,
                    specificObject: _target)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            return _target == null ? 0f : 50f;
        }
    }
}