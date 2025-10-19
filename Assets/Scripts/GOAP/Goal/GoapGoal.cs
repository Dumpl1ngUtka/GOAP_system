using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;
using GOAP.Planner;

namespace GOAP.Goal
{
    public abstract class GoapGoal : IGoapGoal
    {
        private IObjectForFact _target;
        
        protected IPlanContainer PlanContainer;
        
        public abstract string Name { get; }

        public abstract IObjectForFact GetTarget();

        public IGoapGoal Init(IPlanContainer planContainer)
        {
            PlanContainer = planContainer;
            return this;
        }

        public abstract float GetPriority(IGoapKnowledge knowledge);

        public abstract bool IsValid(IGoapKnowledge knowledge);

        public abstract void OnGoalActivated();

        public abstract void OnGoalDeactivated();

        public abstract IEnumerable<FactWithCondition> GetDesiredState();
    }
}