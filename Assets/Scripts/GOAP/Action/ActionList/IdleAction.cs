using System.Collections.Generic;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Action
{
    public class IdleAction : GoapAction
    {
        public override string Name => "Idle";
        public override float Cost => 0.1f;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Perform()
        {
            Debug.Log("Idle");
        }

        public override void OnExit()
        {
        }

        public override IEnumerable<FactWithCondition> GetEffects() => new List<FactWithCondition>();
        
        public override IEnumerable<Fact> GetPreconditions() => new List<Fact>();
        public override IGoapAction Clone() => new IdleAction();
    }
}