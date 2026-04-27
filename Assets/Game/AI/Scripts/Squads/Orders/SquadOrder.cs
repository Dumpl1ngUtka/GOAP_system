using System.Collections.Generic;
using AI.Agent;
using AI.Goal;
using AI.Knowledge;
using UnityEngine;

namespace AI.Squads.Orders
{
    public abstract class SquadOrder
    {
        public IObjectForAI Target { get; protected set; }
        
        // Метод, который Отряд вызывает для распределения целей
        public abstract void DistributeGoals(IEnumerable<AIAgent> squadMembers);
        
        // Опционально: проверка, выполнен ли приказ в целом
        public abstract bool IsOrderCompleted(IKnowledge squadKnowledge);
    }
}