using System.Collections.Generic;
using GOAP.KnowledgeBase;
using Units.Mover;
using UnityEngine;

namespace GOAP.Action
{
    public class MoveToAction : GoapAction
    {
        private IAgentMover _agentMover;
        private Vector3 _targetPosition;
        
        public override string Name => "MoveToAction";
        public override float Cost => 1.0f;

        public MoveToAction(IAgentMover agentMover)
        {
            _agentMover = agentMover;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            if (Target != null)
            {
                var worldObjectForFact = (IWorldObjectForFact)Target;
                _agentMover.SetTargetPosition(worldObjectForFact.GetWorldTransform().position);
            }
            else
            {
                Debug.Log("TARGET IS NULL");
                IsFailed = true;
            }
        }

        public override void Perform()
        {
            if (!_agentMover.IsMoving)
                IsDone = true;
        }

        public override void OnExit()
        {
            _agentMover.Stop();
        }

        public override IEnumerable<FactWithCondition> GetEffects() =>
            new[]
            {
                new FactWithCondition(FactCondition.Include, new Fact(FactTag.Nearby, Target)),
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.Around, Target)),
            };

        public override IEnumerable<Fact> GetPreconditions()
        {
            return new[]
            {
                new Fact(FactTag.Around, Target),
            };
        }

        public override IGoapAction Clone() => new MoveToAction(_agentMover);
    }
}