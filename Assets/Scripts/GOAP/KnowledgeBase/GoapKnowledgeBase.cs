using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP.KnowledgeBase
{
    public class GoapKnowledgeBase : IGoapKnowledge
    {
        private readonly List<Fact> _facts = new();

        public void SetFact(Fact fact) =>
            _facts.Add(fact);

        public Fact GetFact(FactTag key) =>
            _facts.First(fact => fact.Tag == key);

        public IEnumerable<Fact> GetAllFacts() => 
            _facts;

        public bool TryGetFact(FactTag key, out Fact value)
        {
            var fact = _facts.FirstOrDefault(fact => fact.Tag == key);
            value = fact; 
            return fact != null;
        }

        public bool ContainsFact(FactTag key) => 
            _facts.FirstOrDefault(fact => fact.Tag == key) != null;

        public void RemoveAllFacts() =>
            _facts.Clear();
    }
}