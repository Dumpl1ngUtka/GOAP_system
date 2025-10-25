using System.Collections.Generic;
using GOAP.KnowledgeBase;
using Unit.Mover;

namespace GOAP.Action
{
    public class RunAwayAction : GoapAction
    {
        private IAgentMover _agentMover;
        private IWorldObjectForFact _target;
        
        public override string Name => "Run Away";
        public override float Cost => 1;

        public RunAwayAction(IAgentMover agentMover)
        {
            _agentMover = agentMover;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            if (Target is IWorldObjectForFact target)
            {
                _target = target;  
                return;
            }
            IsFailed = true;
        }

        public override void Perform()
        {
            if (!_agentMover.IsMoving)
            {
                var moveVector = _agentMover.GetSelfTransform().position - _target.GetWorldTransform().position;
                var newPosition = (_agentMover.GetSelfTransform().position + moveVector).normalized * 20;
                _agentMover.SetTargetPosition(newPosition);
            }
        }

        public override void OnExit()
        {
        }

        public override IEnumerable<FactWithCondition> GetEffects()
        {
            return new[]
            {
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.IsInDangerous)),
            };
        }

        public override IEnumerable<Fact> GetPreconditions()
        {
            return new[]
            {
                new Fact(FactTag.IsInDangerous),
            };
        }

        public override IGoapAction Clone()
        {
            return new RunAwayAction(_agentMover);
        }
    }
}