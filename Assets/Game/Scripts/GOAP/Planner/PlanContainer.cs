using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Planner
{
    public class PlanContainer : IPlanContainer
    {
        private Queue<IGoapAction> _plan;
        private IObjectForFact _target;
        
        public void SetPlan(Queue<IGoapAction> plan)
        {
            _plan = plan;
            Debug.Log("SetNewPlan With : " + plan.Count);
            foreach (var action in plan)
            {
                Debug.Log(action.Name);
            }
        }

        public Queue<IGoapAction> GetPlan() => 
            _plan;

        public void ResetPlan()
        {
            _plan = new Queue<IGoapAction>();
            _target = null;
        }

        public IGoapAction Dequeue() => 
            _plan.Dequeue();

        public bool IsPlanValid() => 
            _plan != null && _plan.Count != 0;

        public void SetTarget(IObjectForFact target) => 
            _target = target;

        public IObjectForFact GetTarget() => 
            _target;
    }
}