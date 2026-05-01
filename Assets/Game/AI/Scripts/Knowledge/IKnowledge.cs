using System;
using System.Collections.Generic;

namespace AI.Knowledge
{
    public interface IKnowledge
    {
        event Action Changed;
        IEnumerable<Fact> GetAllFacts();
        void RemoveFactsByTarget(IObjectForAI target);
        bool ContainsFact(string conditionTag, params string[] objectTags);
        bool ContainsFact(string conditionTag, IEnumerable<string> objectTags);
        IEnumerable<Fact> GetAllFactsByTag(string conditionTag, params string[] objectTags);
        IEnumerable<Fact> GetAllFactsByTag(string conditionTag,IEnumerable<string> objectTags);
        void RemoveAllFacts();
        void AddFact(Fact fact);
    }
}