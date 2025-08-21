using System.Collections.Generic;
using System.Linq;

namespace GOAP.UnitSpawnRepository
{
    public class AgentDataBase
    {
        private Dictionary<int, List<AgentData>> _teamsData;

        public List<Agent> GetGroupLeads(int teamID)
        {
            return !_teamsData.TryGetValue(teamID, out var teamData) 
                ? null 
                : (from agentData in teamData where agentData.IsLead select agentData.Agent).ToList();
        }

        public void Add(AgentData data)
        {
            var teamID = data.Agent.TeamID;
            if (!_teamsData.ContainsKey(teamID)) 
                _teamsData.Add(teamID, new List<AgentData>());
            
            _teamsData[teamID].Add(data);
        }
    }
}