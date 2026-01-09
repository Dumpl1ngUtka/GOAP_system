using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class KillEnemyGoal : GoalBase
    {
        private IObjectForAI _currentTarget;

        public KillEnemyGoal()
        {
            Name = "Kill Enemy";
        }

        public override bool ValidateAndCalculatePriority(IEnumerable<Fact> knowledgeBase)
        {
            _desiredState.Clear();
            _currentTarget = null;
            
            Fact enemyFact = knowledgeBase.FirstOrDefault(f => 
                f.ConditionTag == GlobalKeys.ConditionTag.Nearby && 
                f.ObjectTag ==  GlobalKeys.WorldObject.Enemy);

            if (enemyFact == null)
            {
                Priority = 0;
                return false; 
            }
            
            _currentTarget = enemyFact.Object;
            
            _desiredState.Add(new GoalCondition(
                GlobalKeys.ConditionTag.Nearby, 
                GlobalKeys.WorldObject.Enemy, 
                mustExist: false,
                specificObject: _currentTarget
            ));

            Priority = 100; 
            return true;
        }
    }
}