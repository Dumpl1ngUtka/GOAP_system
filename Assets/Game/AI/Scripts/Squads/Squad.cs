using System.Collections.Generic;
using AI.Agent;
using AI.Knowledge;
using AI.Sensors;
using AI.Squads.Orders;
using UnityEngine;

namespace AI.Squads
{
    public class Squad : MonoBehaviour
    {
        private List<AIAgent> _members = new List<AIAgent>();
        private SquadOrder _currentOrder;
        
        private IKnowledge _squadKnowledge;
        
        public int MemberCount => _members.Count;
        public SquadOrder CurrentOrder => _currentOrder;
        public List<AIAgent> Members => new List<AIAgent>();

        public void Initialize(IKnowledge squadKnowledge)
        {
            _squadKnowledge = squadKnowledge;
        }

        public void AddMember(AIAgent agent)
        {
            if (!_members.Contains(agent))
            {
                _members.Add(agent);
                // Если у отряда уже есть приказ, сразу вводим новичка в курс дела
                if (_currentOrder != null)
                {
                    _currentOrder.DistributeGoals(new List<AIAgent> { agent });
                }
            }
        }

        public void RemoveMember(AIAgent agent)
        {
            if (_members.Contains(agent))
            {
                agent.ClearOrder(); 
                _members.Remove(agent);
            }
        }

        public void AssignOrder(SquadOrder newOrder)
        {
            _currentOrder = newOrder;
            Debug.Log($"<color=orange>Squad {gameObject.name} received order: {newOrder.GetType().Name}</color>");
            
            _currentOrder.DistributeGoals(_members);
        }

        private void Update()
        {
            if (_currentOrder != null)
            {
                if (_currentOrder.IsOrderCompleted(_squadKnowledge))
                {
                    Debug.Log($"<color=green>Squad {gameObject.name} completed order!</color>");
                    _currentOrder = null;
                    ClearAllMemberOrders();
                }
            }
        }

        private void ClearAllMemberOrders()
        {
            foreach (var member in _members)
            {
                member.ClearOrder();
            }
        }
    }
}