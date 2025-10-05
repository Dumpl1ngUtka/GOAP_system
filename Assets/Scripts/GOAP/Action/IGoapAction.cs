using System.Collections.Generic;
using GOAP.KnowledgeBase;

namespace GOAP.Action
{
    public interface IGoapAction
    {
        string Name { get; }
        float Cost { get; }
        bool IsDone { get; }
        bool IsFailed { get; }
        IEnumerable<Fact> Preconditions { get; }
        IEnumerable<ActionWithFact> Effects { get; }

        void OnEnter();
        bool Perform();
        void OnExit();
        bool CheckProceduralPrecondition(IEnumerable<Fact> facts);
        void ResetAction();
    }

    public struct ActionWithFact
    {
        public readonly ActionType Type;
        public readonly Fact Fact;

        public ActionWithFact(ActionType type, Fact fact)
        {
            Type = type;
            Fact = fact;
        }
    }

    public enum ActionType
    {
        None,
        Remove,
        Add,
    }
}