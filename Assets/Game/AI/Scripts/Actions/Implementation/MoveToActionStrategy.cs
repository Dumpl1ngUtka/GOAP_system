using System.Collections.Generic;
using AI.Goal;
using AI.Knowledge;
using Units.Mover;

namespace AI.Actions.Implementation
{
    public class MoveToActionStrategy : ActionStrategy
    {
        private readonly AgentMover _mover;

        public MoveToActionStrategy(AgentMover mover)
        {
            _mover = mover;
        }

        public override float GetCost(IObjectForAI target)
        {
            return 1.0f;
        }

        public override IEnumerable<GoalCondition> GetPreconditions(IObjectForAI target)
        {
            return new List<GoalCondition>();
        }

        public override IEnumerable<GoalCondition> GetEffects(IObjectForAI target)
        {
            List<GoalCondition> effects = new();
            foreach (string tag in target.GetTags())
            {
                effects.Add(new GoalCondition(GlobalKeys.ConditionTag.Nearby, tag, true, target));
            }
            return effects;
        }

        public override ActionBase CreateAction(IObjectForAI target)
        {
            return new MoveToAction(target, _mover);
        }
    }
}