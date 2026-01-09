using System.Collections.Generic;
using AI.Knowledge;

namespace AI.Goal
{
    public abstract class GoalBase
    {
        public string Name { get; protected set; }
        public float Priority { get; protected set; }

        protected List<GoalCondition> _desiredState = new();

        public IEnumerable<GoalCondition> GetDesiredState() => _desiredState;
        
        public abstract bool ValidateAndCalculatePriority(IEnumerable<Fact> knowledgeBase);
        
        public virtual void OnGoalActivated() { }
        
        public virtual void OnGoalDeactivated() 
        {
            _desiredState.Clear();
        }
    }
}