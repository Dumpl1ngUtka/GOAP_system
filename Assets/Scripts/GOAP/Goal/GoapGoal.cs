using System.Collections.Generic;
using GOAP.KnowledgeBase;

namespace GOAP.Goal
{
    public abstract class GoapGoal : IGoapGoal
    {
        public abstract string Name { get; }
    
        private Dictionary<FactTag, object> _desiredState = new();

        protected GoapGoal()
        {
            InitializeDesiredState();
        }

        protected abstract void InitializeDesiredState();

        public abstract float GetPriority(IGoapKnowledge knowledge);

        public abstract bool IsValid(IGoapKnowledge knowledge);

        public virtual void OnGoalActivated() { }

        public virtual void OnGoalDeactivated() { }

        public IEnumerable<Fact> GetDesiredState() => null;

        protected void AddDesiredEffect(FactTag key, object value)
        {
            _desiredState[key] = value;
        }

        protected void ClearDesiredState()
        {
            _desiredState.Clear();
        }
    }
}