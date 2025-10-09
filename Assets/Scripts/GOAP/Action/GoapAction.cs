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
        public abstract bool Perform();
        public abstract void OnExit();

        public void Init(IPlanContainer planContainer)
        {
            PlanContainer = planContainer;
        }

        public bool CheckProceduralPrecondition(IEnumerable<Fact> facts)
        {
            foreach (var precondition in GetPreconditions())
            {
                var isFactContained = facts.Any(fact => fact.Tag == precondition.Tag);
                if (!isFactContained)
                    return false;
            }

            return true;
        }

        public abstract IEnumerable<ActionWithFact> GetEffects();

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