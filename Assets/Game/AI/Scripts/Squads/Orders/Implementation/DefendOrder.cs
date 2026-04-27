using System.Collections.Generic;
using AI.Agent;
using AI.Goal.Implementation;
using AI.Knowledge;
using Units.UnitClasses;

namespace AI.Squads.Orders
{
    public class DefendOrder : SquadOrder
    {
        private readonly float _priority;

        public DefendOrder(IObjectForAI structureToDefend, float priority)
        {
            Target = structureToDefend;
            _priority = priority;
        }

        public override void DistributeGoals(IEnumerable<AIAgent> squadMembers)
        {
            foreach (var agent in squadMembers)
            {
                switch (agent.Role)
                {
                    case UnitRole.MeleeCombat:
                    case UnitRole.RangedCombat:
                        agent.AssignOrder(new DefendPositionGoal(Target, _priority));
                        break;
                    case UnitRole.Support:
                        agent.AssignOrder(new HealAllyGoal()); // Саппорты лечат защитников
                        break;
                }
            }
        }

        public override bool IsOrderCompleted(IKnowledge squadKnowledge)
        {
            // Приказ отменяется, если здание разрушено
            return squadKnowledge.ContainsFact("StructureDestroyed", Target.GetTags());
        }
    }
}