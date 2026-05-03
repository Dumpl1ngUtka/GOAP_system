using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class MineResourceGoal : GoalBase
    {
        private readonly IObjectForAI _targetNode;
        private readonly float _assignedPriority;

        public MineResourceGoal(IObjectForAI targetNode, float priority = 80f)
        {
            _targetNode = targetNode;
            _assignedPriority = priority;
        }

        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.IsMined, 
                    GlobalKeys.WorldObject.OreNode,
                    mustExist: true,
                    specificObject: _targetNode)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            if (_targetNode == null) return 0f;

            // Если инвентарь полон, приоритет падает в 0, чтобы сработала цель "Отнести на базу"
            bool inventoryFull = knowledgeBase.Any(f => 
                f.ConditionTag == GlobalKeys.ConditionTag.IsHigh && 
                f.ObjectTags.Contains(GlobalKeys.Resources.Inventory));        
            
            if (inventoryFull) return 0f;

            return _assignedPriority;
        }
    }
}