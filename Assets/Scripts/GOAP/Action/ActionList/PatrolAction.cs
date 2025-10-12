using System.Collections.Generic;
using GOAP.KnowledgeBase;
using Unit.Mover;
using UnityEngine;

namespace GOAP.Action
{
    public class PatrolAction : GoapAction
    {
        private IAgentMover _agentMover;
        private Transform[] _patrolPoints;
        private int _currentPatrolPointIndex;
        public override string Name => "Patrol";
        public override float Cost => 1f;

        public PatrolAction(Transform[] points)
        {
            _patrolPoints = points;
        }
        
        public override void OnEnter()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
                IsFailed = true;
            
            _currentPatrolPointIndex = 0;
        }

        public override void Perform()
        {
            if (!_agentMover.IsMoving)
                _agentMover.SetTargetPosition(GetNextPatrolPoint().position);
        }

        public override void OnExit()
        {
        }

        public override IEnumerable<FactWithCondition> GetEffects()
        {
            return new[]
            {
                new FactWithCondition(FactCondition.Include, new Fact(FactTag.IsPatrolling)),
            };
        }

        public override IEnumerable<Fact> GetPreconditions() => new List<Fact>();

        private Transform GetNextPatrolPoint()
        {
            return _patrolPoints[_currentPatrolPointIndex++];
        }
    }
}