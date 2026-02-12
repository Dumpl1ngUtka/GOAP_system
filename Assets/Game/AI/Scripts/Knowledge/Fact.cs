using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Knowledge
{
    public class Fact
    {
        public readonly string ConditionTag;
        public readonly string[] ObjectTags;
        public readonly IObjectForAI Target;

        public Fact(
            string conditionTag, 
            params string[] objectTags)
        {
            ConditionTag = conditionTag;
            ObjectTags = objectTags;
            Target = null;
        }

        public Fact(
            IObjectForAI target,
            string conditionTag, 
            params string[] objectTags)
        {
            ConditionTag = conditionTag;
            ObjectTags = objectTags;
            Target = target;
        }
    }
}