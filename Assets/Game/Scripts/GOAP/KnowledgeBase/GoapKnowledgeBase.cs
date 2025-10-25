using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
using UnityEngine;

namespace GOAP.KnowledgeBase
{
    public class GoapKnowledgeBase : IGoapKnowledge
    {
        private readonly List<Fact> _facts = new();

        public void SetFact(Fact fact) =>
            _facts.Add(fact);

        public IEnumerable<Fact> GetFactsByTag(FactTag key) => 
            _facts.Where(fact => fact.Tag == key);

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

        public bool ContainsFactWithObjectTag(FactTag factTag, ObjectForFactTag tag) => 
            _facts.Any(fact => fact.Tag == factTag && fact.ObjectTags.Contains(tag));

        public IEnumerable<Fact> GetFactsWithObjectTag(FactTag factTag, ObjectForFactTag key) => 
            _facts.Where(fact => fact.Tag == factTag && fact.ObjectTags.Contains(key));

        public void RemoveAllFacts() => 
            _facts.Clear();

        public bool CheckFactWithCondition(FactWithCondition factWithCondition)
        {
            var isNeedToBeInclude = factWithCondition.Type == FactCondition.Include;
            var has = false;
            foreach (var fact in _facts)
            {
                if (fact.Tag != factWithCondition.Fact.Tag)
                    continue;

                var factHasAllTags = true;
                foreach (var factObjectTag in factWithCondition.Fact.ObjectTags)
                {
                    if (!fact.ObjectTags.Contains(factObjectTag))
                    {
                        factHasAllTags = false; 
                        break;
                    }
                }

                if (factHasAllTags)
                {
                    has = true;
                    break;
                }
            }
            return has && isNeedToBeInclude || !has && !isNeedToBeInclude;
        }
    }
}