using System.Collections.Generic;
using System.Linq;
using GOAP.KnowledgeBase;

namespace GOAP.Action
{
    public abstract class GoapAction : IGoapAction
    {
        public abstract string Name { get; }
        public abstract float Cost { get; }
        public bool IsDone { get; protected set; }
        
        public bool IsFailed { get; protected set;}

        public abstract IEnumerable<Fact> Preconditions { get; }
        public abstract IEnumerable<ActionWithFact> Effects { get; }

        public abstract void OnEnter();
        public abstract bool Perform();
        public abstract void OnExit();

        public bool CheckProceduralPrecondition(IEnumerable<Fact> facts)
        {
            foreach (var precondition in Preconditions)
            {
                var isFactContained = facts.Any(fact => fact.Tag == precondition.Tag);
                if (!isFactContained)
                    return false;
            }

            return true;
        }

        public virtual void ResetAction()
        {
        }
    }
}