using System.Collections.Generic;
using AI.Goal;
using AI.Knowledge;

namespace AI.Actions
{
    public abstract class ActionStrategy
    {
        public abstract float GetCost(IObjectForAI target);
        public abstract IEnumerable<GoalCondition> GetPreconditions(IObjectForAI target);
        public abstract IEnumerable<GoalCondition> GetEffects(IObjectForAI target);
        public abstract ActionBase CreateAction(IObjectForAI target);
    }
}