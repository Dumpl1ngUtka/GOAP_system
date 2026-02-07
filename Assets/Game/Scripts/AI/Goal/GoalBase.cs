using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal
{
    public abstract class GoalBase
    {
        public abstract IEnumerable<GoalCondition> GetDesiredState();
        
        public abstract float GetPriority(IEnumerable<Fact> knowledgeBase);
        
        public virtual void OnGoalActivated() { }
        
        public virtual void OnGoalDeactivated() { }
    }
}