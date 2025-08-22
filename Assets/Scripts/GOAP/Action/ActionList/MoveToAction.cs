using GOAP.KnowledgeBase;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP.Action
{
    public class MoveToAction : GoapAction
    {
        public override string Name => "MoveToAction";
        public override float Cost => 1.0f;

        private NavMeshAgent _agent;
        private Vector3 _targetPosition;
        private float _stoppingDistance = 0.5f;

        public MoveToAction(NavMeshAgent agent)
        {
            _agent = agent;
            AddPrecondition("hasTarget", true);
            AddEffect("isAtPosition", true);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (_agent != null)
            {
                _agent.isStopped = false;
            }
        }

        public override bool Perform()
        {
            if (_agent == null || !_agent.isActiveAndEnabled) return false;

            if (_agent.pathPending) return true;

            if (_agent.remainingDistance <= _agent.stoppingDistance + _stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    IsDone = true;
                    return false;
                }
            }

            return true;
        }

        public override void OnExit()
        {
            base.OnExit();
            if (_agent != null)
            {
                _agent.isStopped = true;
            }
        }

        public override bool CheckProceduralPrecondition(IGoapKnowledge knowledge)
        {
            if (!base.CheckProceduralPrecondition(knowledge))
                return false;

            if (knowledge.TryGetObject("moveTarget", out var target) && target != null)
            {
                _targetPosition = target.transform.position;
                _agent.SetDestination(_targetPosition);
                return true;
            }

            if (knowledge.TryGetFact<Vector3>("movePosition", out var position))
            {
                _targetPosition = position;
                _agent.SetDestination(_targetPosition);
                return true;
            }

            return false;
        }
    }
}