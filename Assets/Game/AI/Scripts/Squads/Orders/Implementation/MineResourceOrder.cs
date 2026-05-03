using System.Collections.Generic;
using AI.Agent;
using AI.Goal.Implementation;
using AI.Knowledge;
using Units.UnitClasses;

namespace AI.Squads.Orders
{
    public class MineResourceOrder : SquadOrder
    {
        private readonly float _priority;

        public MineResourceOrder(IObjectForAI oreNode, float priority)
        {
            Target = oreNode;
            _priority = priority;
        }

        public override void DistributeGoals(IEnumerable<AIAgent> squadMembers)
        {
            foreach (var agent in squadMembers)
            {
                if (agent.Role == UnitRole.Worker)
                {
                    agent.AssignOrder(new MineResourceGoal(Target, _priority));
                }
                else
                {
                    // Если в отряд попали бойцы, они охраняют рабочих
                    agent.AssignOrder(new DefendPositionGoal(Target, _priority - 10f));
                }
            }
        }

        public override bool IsOrderCompleted(IKnowledge squadKnowledge)
        {
            // Считаем выполненным, если руда истощена (факт NodeDepleted)
            return squadKnowledge.ContainsFact(GlobalKeys.ConditionTag.Depleted, GlobalKeys.WorldObject.OreNode);
        }
    }
}