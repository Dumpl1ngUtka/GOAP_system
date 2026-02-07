using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;
using AI.Sensors;

namespace AI.Goal.Implementation
{
    public class KillEnemyGoal : GoalBase
    {
        private readonly IObjectForAI _currentTarget;
        private readonly VisualSensor _visualSensor; 

        public KillEnemyGoal(
            IObjectForAI currentTarget,
            VisualSensor visualSensor)
        {
            _currentTarget = currentTarget;
            _visualSensor = visualSensor;
        }

        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.Nearby,
                    GlobalKeys.WorldObject.Enemy,
                    mustExist: false,
                    specificObject: _currentTarget)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            if (_visualSensor == null)
                return 0;
            return 100;
        }
    }
}