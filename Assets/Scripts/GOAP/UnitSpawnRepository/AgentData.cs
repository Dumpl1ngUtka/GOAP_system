using UnityEngine;

namespace GOAP.UnitSpawnRepository
{
    public class AgentData
    {
        public Agent Agent { get; }
        public float PositionPreference { get; }
        public bool IsLead { get; private set; }
        public int GroupID { get; private set; } = -1;
        
        public Vector3 Position => Agent.transform.position;

        public AgentData(Agent agent, float positionPreference)
        {
            Agent = agent;
            PositionPreference = positionPreference;
        }

        public void SetLead(bool isLead)
        {
            IsLead = isLead;
        }

        public AgentData WithGroupID(int groupID)
        {
            GroupID = groupID;
            return this;
        }
    }
}