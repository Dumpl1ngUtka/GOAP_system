using System.Collections.Generic;
using GOAP.KnowledgeBase;
using NUnit.Framework;
using Unit.Mover;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP.Action
{
    public class MoveToAction : GoapAction
    {
        private IAgentMover _agentMover;
        private Vector3 _targetPosition;
        private float _stoppingDistance = 0.5f;
        private readonly Transform _target;
        
        public override string Name => "MoveToAction";
        public override float Cost => 1.0f;

        public override IEnumerable<Fact> Preconditions => null;

        public override IEnumerable<ActionWithFact> Effects => 
            new[] { ActionType.Add, new Fact(FactTag.Nearby, _target) };

        public MoveToAction(IAgentMover agentMover, IWorldObjectForFact target)
        {
            _agentMover = agentMover;
            _target = target.GetWorldTransform();
        }

        public override void OnEnter()
        {
            _agentMover.SetTargetPosition(_target.position);
        }

        public override bool Perform()
        {
            return _agentMover.IsMoving;
        }

        public override void OnExit()
        {
            _agentMover.Stop();
        }
    }
}