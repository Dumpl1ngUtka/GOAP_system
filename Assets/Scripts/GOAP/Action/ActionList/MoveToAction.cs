using System.Collections.Generic;
using GOAP.KnowledgeBase;
using Unit.Mover;
using UnityEngine;

namespace GOAP.Action
{
    public class MoveToAction : GoapAction
    {
        private IAgentMover _agentMover;
        private Vector3 _targetPosition;
        private float _stoppingDistance = 0.5f;
        private Transform _target;
        
        public override string Name => "MoveToAction";
        public override float Cost => 1.0f;
        
        public override void OnEnter()
        {
            if (TryGetTargetTransform(out _target))
                _agentMover.SetTargetPosition(_target.position);
            else
                IsFailed = true;
        }

        public override bool Perform()
        {
            return _agentMover.IsMoving;
        }

        public override void OnExit()
        {
            _agentMover.Stop();
        }

        public override IEnumerable<ActionWithFact> GetEffects() =>
            new[]
            {
                new ActionWithFact(ActionType.Add,new Fact(FactTag.Nearby, PlanContainer.GetTarget())),
            };

        public override IEnumerable<Fact> GetPreconditions() => null;
    }
}