// --- START OF FILE CommanderRegistry.cs ---
using System.Collections.Generic;
using AI.Agent;

namespace AI.Global
{
    public class CommanderRegistry
    {
        private readonly Dictionary<string, CommanderAgent> _commandersByTeam = new Dictionary<string, CommanderAgent>();

        public void RegisterCommander(string team, CommanderAgent commander)
        {
            if (!_commandersByTeam.ContainsKey(team))
            {
                _commandersByTeam.Add(team, commander);
            }
            else
            {
                _commandersByTeam[team] = commander; // Перезаписываем, если загрузили новый уровень
            }
        }

        public void UnregisterCommander(string team)
        {
            if (_commandersByTeam.ContainsKey(team))
            {
                _commandersByTeam.Remove(team);
            }
        }

        public bool TryGetCommander(string team, out CommanderAgent commander)
        {
            return _commandersByTeam.TryGetValue(team, out commander);
        }
    }
}