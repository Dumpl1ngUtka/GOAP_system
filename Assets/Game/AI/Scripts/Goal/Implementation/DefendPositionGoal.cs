using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class DefendPositionGoal : GoalBase
    {
        private readonly IObjectForAI _targetStructure;
        private readonly float _assignedPriority;

        public DefendPositionGoal(IObjectForAI targetStructure, float priority = 90f)
        {
            _targetStructure = targetStructure;
            _assignedPriority = priority;
        }

        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.Nearby,
                    GlobalKeys.WorldObject.Ally,
                    mustExist: true,
                    specificObject: _targetStructure)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            return _targetStructure == null ? 0f : _assignedPriority;
        }
    }
}