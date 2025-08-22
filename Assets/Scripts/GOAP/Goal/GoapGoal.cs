using System.Collections.Generic;
using GOAP.KnowledgeBase;

namespace GOAP.Goal
{
    public abstract class GoapGoal : IGoapGoal
    {
        public abstract string Name { get; }
        public abstract float Priority { get; }
    
        private Dictionary<string, object> _desiredState = new Dictionary<string, object>();

        protected GoapGoal()
        {
            InitializeDesiredState();
        }

        protected abstract void InitializeDesiredState();

        public abstract bool IsValid(IGoapKnowledge knowledge);

        public virtual void OnGoalActivated() { }

        public virtual void OnGoalDeactivated() { }

        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>(_desiredState);
        }

        protected void AddDesiredEffect(string key, object value)
        {
            _desiredState[key] = value;
        }

        protected void ClearDesiredState()
        {
            _desiredState.Clear();
        }
    }
}