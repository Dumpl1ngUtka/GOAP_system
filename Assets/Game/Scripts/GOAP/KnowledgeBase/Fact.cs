using System;
using System.Collections.Generic;
using System.Linq;

namespace GOAP.KnowledgeBase
{
    public class Fact
    {
        public readonly FactTag Tag;
        public readonly IObjectForFact Object;
        public readonly List<ObjectForFactTag> ObjectTags;

        public Fact(FactTag tag, IObjectForFact obj = null)
        {
            Tag = tag;
            Object = obj;
            ObjectTags = obj == null? new List<ObjectForFactTag>() : obj.GetTags().ToList();
        }
        
        public Fact WithAdditionObjectForFactTags(params ObjectForFactTag[] facts)
        {
            ObjectTags.AddRange(facts);
            return this;
        }
        
        public Fact WithAdditionObjectForFactTags(IEnumerable<ObjectForFactTag> facts)
        {
            ObjectTags.AddRange(facts);
            return this;
        }
    }
}