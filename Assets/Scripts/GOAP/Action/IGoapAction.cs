using System.Collections.Generic;
using GOAP.KnowledgeBase;

namespace GOAP.Action
{
    public interface IGoapAction
    {
        string Name { get; }
        float Cost { get; }
        bool IsDone { get; }

        Dictionary<string, object> Preconditions { get; }
        Dictionary<string, object> Effects { get; }

        void OnEnter();
        bool Perform();
        void OnExit();
        bool CheckProceduralPrecondition(IGoapKnowledge knowledge);
        void ResetAction();
    }
}