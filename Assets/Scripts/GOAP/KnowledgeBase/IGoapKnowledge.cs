using System.Collections.Generic;
using UnityEngine;

namespace GOAP.KnowledgeBase
{
    public interface IGoapKnowledge
    {
        void SetFact(Fact fact);
        Fact GetFact(FactTag key);
        IEnumerable<Fact> GetAllFacts();
        bool TryGetFact(FactTag key, out Fact value);
        bool ContainsFact(FactTag key);
        void RemoveAllFacts();
    }
}