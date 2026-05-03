using System.Collections.Generic;
using AI.Agent;
using AI.Goal.Implementation;
using AI.Knowledge;
using Units.UnitClasses;

namespace AI.Squads.Orders
{
    public class AssaultOrder : SquadOrder
    {
        private readonly float _priority;

        public AssaultOrder(IObjectForAI enemyStructure, float priority)
        {
            Target = enemyStructure;
            _priority = priority;
        }

        public override void DistributeGoals(IEnumerable<AIAgent> squadMembers)
        {
            foreach (var agent in squadMembers)
            {
                if (agent.Role == UnitRole.MeleeCombat || agent.Role == UnitRole.RangedCombat)
                {
                    agent.AssignOrder(new KillEnemyGoal(Target, _priority)); 
                }
                else if (agent.Role == UnitRole.Support)
                {
                    agent.AssignOrder(new HealAllyGoal());
                }
            }
        }

        public override bool IsOrderCompleted(IKnowledge squadKnowledge)
        {
            // Выполнено, когда здание врага уничтожено
            return squadKnowledge.ContainsFact("StructureDestroyed", Target.GetTags());
        }
    }
}