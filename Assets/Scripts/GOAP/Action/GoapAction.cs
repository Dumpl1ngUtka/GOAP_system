using System.Collections.Generic;
using GOAP.KnowledgeBase;

namespace GOAP.Action
{
    public abstract class GoapAction : IGoapAction
    {
        public abstract string Name { get; }
        public abstract float Cost { get; }
        public bool IsDone { get; protected set; }

        public Dictionary<string, object> Preconditions { get; protected set; }
        public Dictionary<string, object> Effects { get; protected set; }

        protected GoapAction()
        {
            Preconditions = new Dictionary<string, object>();
            Effects = new Dictionary<string, object>();
            IsDone = false;
        }

        public virtual void OnEnter()
        {
            IsDone = false;
        }

        public abstract bool Perform();

        public virtual void OnExit()
        {
            IsDone = false;
        }

        public virtual bool CheckProceduralPrecondition(IGoapKnowledge knowledge)
        {
            foreach (var precondition in Preconditions)
            {
                if (!knowledge.ContainsFact(precondition.Key))
                    return false;

                if (!knowledge.GetFact<object>(precondition.Key).Equals(precondition.Value))
                    return false;
            }
            return true;
        }

        public virtual void ResetAction()
        {
            IsDone = false;
        }

        protected void AddPrecondition(string key, object value)
        {
            Preconditions[key] = value;
        }

        protected void AddEffect(string key, object value)
        {
            Effects[key] = value;
        }
    }
}