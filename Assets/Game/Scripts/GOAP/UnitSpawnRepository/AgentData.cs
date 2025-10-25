using GOAP.Agent;
using UnityEngine;

namespace GOAP.UnitSpawnRepository
{
    public class AgentData
    {
        public GoapAgent GoapAgent { get; }
        public float PositionPreference { get; }
        public bool IsLead { get; private set; }
        public int GroupID { get; private set; } = -1;
        
        public Vector3 Position => GoapAgent.transform.position;

        public AgentData(GoapAgent goapAgent, float positionPreference)
        {
            GoapAgent = goapAgent;
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