using System.Collections.Generic;
using System.Linq;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using UnityEngine;

namespace GOAP.Action
{
    public abstract class GoapAction : IGoapAction
    {
        protected IPlanContainer PlanContainer;
        
        public abstract string Name { get; }
        public abstract float Cost { get; }
        public bool IsDone { get; protected set; }
        public bool IsFailed { get; protected set;}

        public abstract void OnEnter();
        public abstract void Perform();
        public abstract void OnExit();

        public IGoapAction Init(IPlanContainer planContainer)
        {
            PlanContainer = planContainer;
            return this;
        }

        public bool CheckProceduralPrecondition(IEnumerable<Fact> facts)
        {
            if (GetPreconditions() == null)
                return true;
                    
            foreach (var precondition in GetPreconditions())
            {
                var isFactContained = facts.Any(fact => fact.Tag == precondition.Tag);
                if (!isFactContained)
                    return false;
            }

            return true;
        }

        public abstract IEnumerable<FactWithCondition> GetEffects();

        public abstract IEnumerable<Fact> GetPreconditions();
        
        public virtual void ResetAction()
        {
        }
        
        protected bool TryGetTargetTransform(out Transform target)
        {
            var targetInContainer = PlanContainer.GetTarget();
            if (targetInContainer is IWorldObjectForFact worldObject)
            {
                target = worldObject.GetWorldTransform();
                return true;
            }
            target = null;
            return false;
        }
        
        protected bool TryGetTarget(out IObjectForFact target)
        {
            target = PlanContainer.GetTarget();
            return target != null;
        }
    }
}