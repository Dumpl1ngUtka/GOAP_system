using System;
using System.Collections.Generic;
using System.Linq;

namespace GOAP.KnowledgeBase
{
    public class Fact
    {
        public readonly FactTag Tag;
        public readonly IObjectForFact Object;
        public readonly IEnumerable<ObjectForFactTag> ObjectTags;

        public Fact(FactTag tag, IObjectForFact obj = null)
        {
            Tag = tag;
            Object = obj;
            ObjectTags = obj == null? Array.Empty<ObjectForFactTag>() : obj.GetTags();
        }
        
        public Fact(FactTag tag, IEnumerable<ObjectForFactTag> objFacts)
        {
            Tag = tag;
            Object = null;
            ObjectTags = objFacts;
        }
        
        public Fact(FactTag tag, ObjectForFactTag objFact)
        {
            Tag = tag;
            Object = null;
            ObjectTags = new List<ObjectForFactTag>
            {
                objFact
            };
        }
    }
}