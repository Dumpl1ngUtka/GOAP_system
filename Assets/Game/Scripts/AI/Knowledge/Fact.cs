using System.Collections.Generic;
using System.Linq;
using GOAP.KnowledgeBase;
using Unity.VisualScripting;

namespace AI.Knowledge
{
    public class Fact
    {
        public readonly string ConditionTag;
        public readonly string[] ObjectTags;

        public Fact(
            string conditionTag, 
            params string[] objectTags)
        {
            ConditionTag = conditionTag;
            ObjectTags = objectTags;
        }
    }
}