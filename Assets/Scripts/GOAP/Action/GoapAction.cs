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
        
        public IObjectForFact Target { get; set; }

        public virtual void OnEnter()
        {
            IsDone = false;
            IsFailed = false;
        }
        public abstract void Perform();
        public abstract void OnExit();

        public IGoapAction Init(IPlanContainer planContainer)
        {
            PlanContainer = planContainer;
            return this;
        }

        public bool CheckProceduralPrecondition(IEnumerable<Fact> facts)
        {
            if (!GetPreconditions().Any())
                return true;
            
            foreach (var precondition in GetPreconditions())
            {
                var isPreconditionContains = false;
                foreach (var fact in facts)
                {
                    if (fact.Tag == precondition.Tag)
                    {
                        var isRightFact = true;
                        foreach (var preconditionObjectTag in precondition.ObjectTags)
                        {
                            if (!fact.ObjectTags.Contains(preconditionObjectTag))
                            {
                                isRightFact = false;
                                break;
                            }
                        }

                        if (isRightFact)
                        {
                            isPreconditionContains = true;
                            break;
                        }
                    }
                }
                if (!isPreconditionContains)
                    return false;
            }

            return true;
        }

        public abstract IEnumerable<FactWithCondition> GetEffects();

        public abstract IEnumerable<Fact> GetPreconditions();
        public abstract IGoapAction Clone();
        public IGoapAction WithTarget(IObjectForFact target)
        {
            Target = target;
            Debug.Log("Set "+ target + " for " + Name);
            return this;
        }

        public virtual void ResetAction()
        {
        }
    }
}