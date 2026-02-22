using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class PickUpItemGoal : GoalBase
    {
        private readonly IObjectForAI _targetItem;

        public PickUpItemGoal(IObjectForAI targetItem = null)
        {
            _targetItem = targetItem;
        }

        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.Has,
                    GlobalKeys.WorldObject.DroppedItem,
                    mustExist: true,
                    specificObject: _targetItem)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            float priorityMultiplier = _targetItem == null ? 0.5f : 1f;
            
            Fact itemFact = _targetItem == null ? 
                knowledgeBase.FirstOrDefault(fact => fact.ObjectTags.Contains(GlobalKeys.WorldObject.DroppedItem)) 
                : knowledgeBase.FirstOrDefault(fact => fact.Target == _targetItem);
            
            if (itemFact == null)
                return 0f;

            if (!itemFact.ObjectTags.Contains(GlobalKeys.WorldObject.DroppedItem))
                return 0f;

            float priorityByDistance = itemFact.ConditionTag switch
            {
                GlobalKeys.ConditionTag.Nearby => 100f,
                GlobalKeys.ConditionTag.Around => 60f,
                GlobalKeys.ConditionTag.InSight => 30f,
                _ => 0f
            };

            return priorityByDistance * priorityMultiplier;
        }
    }
}