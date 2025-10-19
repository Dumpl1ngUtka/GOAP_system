using System.Collections.Generic;
using GOAP.KnowledgeBase;
using GOAP.Planner;

namespace GOAP.Action
{
    public interface IGoapAction
    {
        string Name { get; }
        float Cost { get; }
        bool IsDone { get; }
        bool IsFailed { get; }
        IObjectForFact Target { get; set; }
        IGoapAction Init(IPlanContainer planContainer);
        void OnEnter();
        void Perform();
        void OnExit();
        bool CheckProceduralPrecondition(IEnumerable<Fact> facts);
        IEnumerable<FactWithCondition> GetEffects();
        IEnumerable<Fact> GetPreconditions();
        IGoapAction Clone();
        IGoapAction WithTarget(IObjectForFact target);
        void ResetAction();
    }

    public struct FactWithCondition
    {
        public readonly FactCondition Type;
        public readonly Fact Fact;

        public FactWithCondition(FactCondition type, Fact fact)
        {
            Type = type;
            Fact = fact;
        }
    }

    public enum FactCondition
    {
        None,
        Exclude,
        Include,
    }
}