using System.Collections.Generic;
using System.Linq;
using GOAP.KnowledgeBase;
using Unity.VisualScripting;

namespace AI.Knowledge
{
    public class Fact
    {
        public readonly IObjectForAI Object;
        public readonly string ConditionTag;
        public readonly string ObjectTag;

        public Fact(string conditionTag, string objectTag, IObjectForAI aiObject = null)
        {
            ConditionTag = conditionTag;
            ObjectTag = objectTag;
            Object = aiObject;
        }
    }
}