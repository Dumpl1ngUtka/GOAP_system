using System.Collections.Generic;
using GOAP.KnowledgeBase;
using Unit.Mover;
using UnityEngine;

namespace GOAP.Action
{
    public class PatrolAction : GoapAction
    {
        private IAgentMover _agentMover;
        private Vector3[] _patrolPoints;
        private int _currentPatrolPointIndex;
        public override string Name => "Patrol";
        public override float Cost => 1f;

        public PatrolAction(IAgentMover agentMover)
        {
            _agentMover = agentMover;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            if (_patrolPoints == null || _patrolPoints.Length == 0)
                _patrolPoints = GetNewPatrolPath();
            
            _currentPatrolPointIndex = 0;
        }

        private Vector3[] GetNewPatrolPath()
        {
            var selfPos = _agentMover.GetSelfTransform().position;
            var patrolPoints = new List<Vector3>
            {
                selfPos + Vector3.left * 10,
                selfPos + Vector3.left * -10,
                selfPos + Vector3.forward * 10,
                selfPos + Vector3.forward * -10
            };
            return patrolPoints.ToArray();
        }

        public override void Perform()
        {
            if (!_agentMover.IsMoving)
                _agentMover.SetTargetPosition(GetNextPatrolPoint());
        }

        public override void OnExit()
        {
            _agentMover.Stop();
        }

        public override IEnumerable<FactWithCondition> GetEffects()
        {
            return new[]
            {
                new FactWithCondition(FactCondition.Include, new Fact(FactTag.IsPatrolling)),
            };
        }

        public override IEnumerable<Fact> GetPreconditions() => new List<Fact>();
        public override IGoapAction Clone()
        {
            return new PatrolAction(_agentMover);
        }

        private Vector3 GetNextPatrolPoint()
        {
            _currentPatrolPointIndex += 1;
            if (_currentPatrolPointIndex >= _patrolPoints.Length)
                _currentPatrolPointIndex = 0;
            return _patrolPoints[_currentPatrolPointIndex];
        }
    }
}