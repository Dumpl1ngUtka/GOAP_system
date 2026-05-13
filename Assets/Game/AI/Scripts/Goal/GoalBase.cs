using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal
{
    public abstract class GoalBase
    {
        protected IObjectForAI _currentTarget;
        
        public abstract IEnumerable<GoalCondition> GetDesiredState();
        
        public abstract float GetPriority(IEnumerable<Fact> knowledgeBase);
        
        public virtual void OnGoalActivated() { }
        
        public virtual void OnGoalDeactivated()
        {
            _currentTarget = null;
        }
    }
}