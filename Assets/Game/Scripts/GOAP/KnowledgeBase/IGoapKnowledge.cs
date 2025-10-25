using System.Collections.Generic;
using GOAP.Action;
using UnityEngine;

namespace GOAP.KnowledgeBase
{
    public interface IGoapKnowledge
    {
        void SetFact(Fact fact);
        IEnumerable<Fact> GetFactsByTag(FactTag key);
        IEnumerable<Fact> GetAllFacts();
        bool TryGetFact(FactTag key, out Fact value);
        bool ContainsFact(FactTag key);
        bool ContainsFactWithObjectTag(FactTag factTag, ObjectForFactTag tag);
        IEnumerable<Fact> GetFactsWithObjectTag(FactTag factTag, ObjectForFactTag key);
        void RemoveAllFacts();
        bool CheckFactWithCondition(FactWithCondition factWithCondition);
    }
}